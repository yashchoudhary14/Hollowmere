using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Hollowmere
{
    public enum HollowState { Dormant, Roam, Investigate, TestHouse, Hunt, Grab, Ritual, Banished }

    /// <summary>
    /// The Hollow, version 1: one NavMesh agent that sleeps through the day, appears at the chapel when night falls,
    /// roams, hears through NoiseSystem (walls muffle), sees only what is lit or very close, tests houses by the rules
    /// of each act, hunts, grabs, snuffs candles during the chant, and is banished when the gate opens.
    /// Runs on the host only from milestone 2. Design: docs/DESIGN.md "The monster" and "Night phase".
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class HollowAI : MonoBehaviour
    {
        public MatchDirector Director;
        public VillageLayout Layout;
        public Transform Eyes;
        public GameObject Visual;

        [Header("Speeds, DESIGN.md \"The monster\"")]
        public float WalkSpeed = 1.4f;
        public float HuntSpeed = 5.5f;
        [Tooltip("Not in DESIGN.md yet: hurrying towards a sound or a house.")]
        public float InvestigateSpeed = 2.5f;

        [Header("Senses, DESIGN.md \"The monster\"")]
        public float HearingOutdoors = 40f;
        public float HearingThroughWalls = 15f;
        public float SightRange = 20f;
        [Tooltip("Full darkness hides you beyond this range.")]
        public float DarkSightRange = 3f;
        [Tooltip("Not in DESIGN.md yet: field of view in degrees for anything beyond DarkSightRange.")]
        public float SightAngle = 140f;
        [Tooltip("Monster variants (milestone 3) scale these.")]
        public float HearingMultiplier = 1f;
        public float SightMultiplier = 1f;

        [Header("Attack")]
        public float GrabRange = 1.3f;
        [Tooltip("DESIGN.md \"The monster\": it grabs; teammates have 5 seconds to save you.")]
        public float GrabSeconds = 5f;
        [Tooltip("Not in DESIGN.md yet: seconds it keeps chasing after losing sight.")]
        public float HuntMemorySeconds = 6f;
        [Tooltip("Not in DESIGN.md yet: seconds it listens at a closed door after knocking.")]
        public float ListenAtDoorSeconds = 6f;
        [Tooltip("Not in DESIGN.md yet: seconds between candles it goes for during the chant.")]
        public float SnuffInterval = 12f;
        [Tooltip("DESIGN.md \"Dusk transition\": someone caught outside has about 30 seconds to reach a door.")]
        public float CaughtOutsideSeconds = 30f;

        public HollowState State { get; private set; } = HollowState.Dormant;
        public PlayerAgent Target { get; private set; }
        public House TargetHouse { get; private set; }
        public string LastHeard { get; private set; } = "nothing";

        /// <summary>Debug (F3): stands still and does nothing.</summary>
        public bool Frozen;

        const int IgnoreRaycastLayer = 2;
        static readonly int SightMask = Physics.DefaultRaycastLayers & ~(1 << IgnoreRaycastLayer);

        NavMeshAgent _agent;
        System.Random _rng;
        float _stateTime, _lastSeen, _grabTimer, _nextSnuff, _enterAt, _nextStep, _nextBreath, _trackUntil;
        Vector3 _lastKnown, _investigateAt;
        bool _heardInside;
        int _houseStep;                      // TestHouse: 0 approach, 1 listen at the door, 2 opening, 3 searching
        readonly Queue<Vector3> _searchPoints = new Queue<Vector3>();
        Candle _candle;

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        void OnEnable()
        {
            NoiseSystem.OnNoise += OnNoise;
            if (Director == null) return;
            Director.NightStarted += OnNightStarted;
            Director.ChantStarted += OnChantStarted;
            Director.GateOpened += Banish;
            Director.HollowCalled += OnCalled;
        }

        void OnDisable()
        {
            NoiseSystem.OnNoise -= OnNoise;
            if (Director == null) return;
            Director.NightStarted -= OnNightStarted;
            Director.ChantStarted -= OnChantStarted;
            Director.GateOpened -= Banish;
            Director.HollowCalled -= OnCalled;
        }

        void Start() => Hide(HollowState.Dormant);

        // ------------------------------------------------------------ appearing and leaving

        void Hide(HollowState state)
        {
            State = state;
            Target = null;
            TargetHouse = null;
            if (Visual != null) Visual.SetActive(false);
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
            if (_agent.enabled) _agent.enabled = false;
        }

        void Appear(Vector3 at)
        {
            if (Visual != null) Visual.SetActive(true);
            foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = true;
            if (NavMesh.SamplePosition(at, out var hit, 5f, NavMesh.AllAreas)) at = hit.position;
            transform.position = at;
            _agent.enabled = true;
            _agent.Warp(at);
        }

        /// <summary>DESIGN.md "Map": the chapel is where the Hollow first appears each night.</summary>
        void OnNightStarted()
        {
            _rng = new System.Random(Director.Plan.Seed ^ 0x48_4f_4c); // its own choices, repeatable per match seed
            Appear(Layout.HollowNightSpawn.position);
            BeginRoam();
            // Caught outside: a player outside when the bell ends is hunted immediately (DESIGN.md "Dusk transition").
            foreach (var p in Director.Players)
                if (CanTarget(p) && Director.HouseAt(p.transform.position) == null)
                {
                    StartHunt(p);
                    _trackUntil = Time.time + CaughtOutsideSeconds; // it knows where they are for a while
                    break;
                }
        }

        void OnChantStarted()
        {
            if (State == HollowState.Dormant || State == HollowState.Banished) return;
            BeginRitual();
        }

        /// <summary>The gate opens and the Hollow is thrown back into the forest (DESIGN.md "The Ritual of Passage").</summary>
        void Banish()
        {
            if (State == HollowState.Dormant) return;
            if (State == HollowState.Grab && Target != null && Target.Condition == PlayerCondition.Grabbed)
                Target.SetCondition(PlayerCondition.Alive);
            Hide(HollowState.Banished);
            transform.position = Layout.ForestSpawns.Length > 0 ? Layout.ForestSpawns[0].position : transform.position;
        }

        /// <summary>A part in the wrong altar slot calls it straight to the square.</summary>
        void OnCalled(Vector3 where)
        {
            if (!IsActive || State == HollowState.Grab || State == HollowState.Hunt) return;
            BeginInvestigate(where, HuntSpeed);
        }

        bool IsActive => State != HollowState.Dormant && State != HollowState.Banished;

        // ------------------------------------------------------------ the loop

        void Update()
        {
            if (!IsActive || !_agent.enabled) return;
            bool stop = Frozen || Director.Outcome != MatchOutcome.Running;
            _agent.isStopped = stop;
            if (stop) return;

            _stateTime += Time.deltaTime;
            if (State != HollowState.Grab) LookForPlayers();

            switch (State)
            {
                case HollowState.Roam: UpdateRoam(); break;
                case HollowState.Investigate: UpdateInvestigate(); break;
                case HollowState.TestHouse: UpdateTestHouse(); break;
                case HollowState.Hunt: UpdateHunt(); break;
                case HollowState.Grab: UpdateGrab(); break;
                case HollowState.Ritual: UpdateRitual(); break;
            }
            MakeItsOwnNoise();
        }

        void SetState(HollowState state, float speed)
        {
            State = state;
            _stateTime = 0f;
            _agent.speed = speed;
        }

        /// <summary>Back to roaming, or to the candles if the chant is still going.</summary>
        void BeginRoam()
        {
            Target = null;
            TargetHouse = null;
            if (Director.Circle != null && Director.Circle.Chant.Started && !Director.Circle.Chant.Complete)
            {
                BeginRitual();
                return;
            }
            SetState(HollowState.Roam, WalkSpeed);
            PickNextRoam();
        }

        void UpdateRoam()
        {
            if (Arrived(1.5f) || _stateTime > 45f) PickNextRoam();
        }

        void PickNextRoam()
        {
            _stateTime = 0f;
            int act = Director.Now.Act;
            // Act 3, The Hunt: it goes into houses after whoever is inside (DESIGN.md "Night phase").
            if (act >= 3)
            {
                foreach (var p in Director.Players)
                {
                    var house = CanTarget(p) ? Director.HouseAt(p.transform.position) : null;
                    if (house != null) { BeginTestHouse(house); return; }
                }
            }
            // Otherwise it tests the house nearest to it now and then, knocking in Act 1.
            if (_rng.NextDouble() < 0.45)
            {
                var house = NearestOccupiedOrAllottedHouse();
                if (house != null) { BeginTestHouse(house); return; }
            }
            var points = Layout.RoamPoints;
            SetDestination(points[_rng.Next(points.Length)].position);
        }

        House NearestOccupiedOrAllottedHouse()
        {
            House best = null;
            float bestDistance = 140f;
            foreach (var h in Director.Houses)
            {
                bool interesting = Director.Plan.IsAllotted(h.Index);
                foreach (var p in Director.Players)
                    if (CanTarget(p) && h.Contains(p.transform.position)) interesting = true;
                if (!interesting) continue;
                float d = Vector3.Distance(transform.position, h.transform.position);
                if (d < bestDistance) { bestDistance = d; best = h; }
            }
            return best;
        }

        // ------------------------------------------------------------ senses

        bool CanTarget(PlayerAgent p) => p != null && p.Condition == PlayerCondition.Alive;

        /// <summary>Lit hearth rooms always; the stone circle while the chant lasts.</summary>
        bool Sheltered(Vector3 pos)
        {
            foreach (var h in Director.Houses)
                if (h.Hearth != null && h.Hearth.Protects(pos)) return true;
            return Director.Circle != null && Director.Circle.Shelters(pos);
        }

        void LookForPlayers()
        {
            foreach (var p in Director.Players)
            {
                if (!CanTarget(p) || p.InHiding || Sheltered(p.transform.position)) continue;
                if (CanSee(p)) StartHunt(p);
            }
        }

        /// <summary>Sight: 20 m with line of sight, but only lit targets, or anyone within 3 m in the dark.</summary>
        public bool CanSee(PlayerAgent p)
        {
            var eye = Eyes.position;
            var head = p.Head.position;
            var toTarget = head - eye;
            float distance = toTarget.magnitude;
            if (distance > SightRange * SightMultiplier) return false;
            bool close = distance <= DarkSightRange;
            if (!close)
            {
                var flat = Vector3.ProjectOnPlane(toTarget, Vector3.up);
                if (Vector3.Angle(transform.forward, flat) > SightAngle * 0.5f) return false;
                if (!IsLit(p)) return false;
            }
            return HasLineOfSight(eye, head, p);
        }

        bool IsLit(PlayerAgent p)
        {
            var phase = Director.Now.Phase;
            if (phase != MatchPhase.Night && phase != MatchPhase.Dawn) return true;
            if (p.Flashlight != null && p.Flashlight.IsOn) return true;
            var house = Director.HouseAt(p.transform.position);
            return house != null && house.IsLitAt(p.transform.position);
        }

        static bool HasLineOfSight(Vector3 from, Vector3 to, PlayerAgent target)
        {
            if (!Physics.Linecast(from, to, out var hit, SightMask, QueryTriggerInteraction.Ignore)) return true;
            return hit.collider.GetComponentInParent<PlayerAgent>() == target;
        }

        /// <summary>Hearing: the sound's own radius, capped at 40 m in the open and about 15 m through walls.</summary>
        void OnNoise(NoiseEvent e)
        {
            if (!IsActive || Frozen || State == HollowState.Grab || e.SourcePlayer < 0) return;
            float distance = Vector3.Distance(Eyes.position, e.Position);
            bool muffled = Physics.Linecast(e.Position + Vector3.up * 0.5f, Eyes.position, SightMask, QueryTriggerInteraction.Ignore);
            float reach = Mathf.Min(e.Radius, muffled ? HearingThroughWalls : HearingOutdoors) * HearingMultiplier;
            if (distance > reach) return;
            LastHeard = $"{e.Kind} {distance:0} m{(muffled ? " through a wall" : "")}";

            var house = Director.HouseAt(e.Position);
            if (State == HollowState.TestHouse && house == TargetHouse) _heardInside = true;
            if (State == HollowState.Hunt)
            {
                if (Target != null && e.SourcePlayer == Target.Index) _lastKnown = e.Position;
                return;
            }
            if (State == HollowState.Ritual || State == HollowState.TestHouse) return;
            if (house != null) BeginTestHouse(house);
            else BeginInvestigate(e.Position, InvestigateSpeed);
        }

        // ------------------------------------------------------------ investigate

        void BeginInvestigate(Vector3 where, float speed)
        {
            SetState(HollowState.Investigate, speed);
            _investigateAt = where;
            SetDestination(where);
        }

        void UpdateInvestigate()
        {
            if (!Arrived(1.5f) && _stateTime < 30f) return;
            if (_stateTime < 3f) return; // stands still and listens
            var house = Director.HouseAt(_investigateAt) ?? HouseNear(_investigateAt, 12f);
            if (house != null) BeginTestHouse(house);
            else BeginRoam();
        }

        House HouseNear(Vector3 pos, float within)
        {
            House best = null;
            foreach (var h in Director.Houses)
            {
                float d = Vector3.Distance(pos, h.Interior.ClosestPoint(pos));
                if (d < within) { within = d; best = h; }
            }
            return best;
        }

        // ------------------------------------------------------------ testing a house

        void BeginTestHouse(House house)
        {
            SetState(HollowState.TestHouse, InvestigateSpeed);
            TargetHouse = house;
            _houseStep = 0;
            _heardInside = false;
            SetDestination(DoorStep(house));
        }

        static Vector3 DoorStep(House h) => h.Door.transform.position + h.transform.forward * 1.4f + h.transform.right * 0.8f;

        /// <summary>Act 1 never; from Act 2 it may come in (DESIGN.md "The monster", aggression by act).</summary>
        bool MayEnter() => Director.Now.Act >= 2;

        /// <summary>Act 3 enters any house; from Act 2 hollow houses with cold hearths are open to it.</summary>
        bool WalksInFreely(House h) => Director.Now.Act >= 3 || (Director.Now.Act >= 2 && !h.Hearth.Lit);

        void UpdateTestHouse()
        {
            var h = TargetHouse;
            switch (_houseStep)
            {
                case 0: // walking to the door
                    if (!Arrived(1.0f) && _stateTime < 40f) return;
                    _stateTime = 0f;
                    if (WalksInFreely(h) || (h.Door.IsOpen && MayEnter()))
                    {
                        OpenAndEnter(h);
                    }
                    else
                    {
                        SoundBank.PlayAt(SoundBank.Knock, h.Door.transform.position + Vector3.up * 1.2f, 1f, 30f);
                        _heardInside = false;
                        _houseStep = 1;
                    }
                    break;

                case 1: // listening at the closed door
                    if (_heardInside && MayEnter()) OpenAndEnter(h);
                    else if (_stateTime > ListenAtDoorSeconds) BeginRoam(); // a quiet house sends it away
                    break;

                case 2: // letting the opened door's carving clear
                    if (Time.time < _enterAt) return;
                    _houseStep = 3;
                    _stateTime = 0f;
                    NextSearchPoint();
                    break;

                case 3: // walking room to room
                    FindHiddenBreathing(h);
                    if (State != HollowState.TestHouse) return;
                    if (Arrived(1.0f) || _stateTime > 10f) NextSearchPoint();
                    break;
            }
        }

        void OpenAndEnter(House h)
        {
            if (!h.Door.IsOpen) h.Door.ForceOpen();
            _houseStep = 2;
            _enterAt = Time.time + 1.2f;
            _searchPoints.Clear();
            foreach (var room in h.Rooms)
                if (!(room.IsHearthRoom && h.Hearth.Lit))
                    _searchPoints.Enqueue(new Vector3(room.Bounds.center.x, h.transform.position.y, room.Bounds.center.z));
            _searchPoints.Enqueue(DoorStep(h));
        }

        void NextSearchPoint()
        {
            _stateTime = 0f;
            if (_searchPoints.Count == 0)
            {
                BeginRoam();
                return;
            }
            SetDestination(_searchPoints.Dequeue());
        }

        /// <summary>Hiding is safe unless your breathing is heard within 3 m (DESIGN.md "The monster").</summary>
        void FindHiddenBreathing(House h)
        {
            foreach (var p in Director.Players)
            {
                if (!CanTarget(p) || !p.InHiding || p.Hiding == null) continue;
                if (!p.Hiding.BreathingAudibleFrom(Eyes.position)) continue;
                p.Hiding.Leave();
                p.Notify("It heard you breathing.");
                BeginGrab(p);
                return;
            }
        }

        // ------------------------------------------------------------ hunt and grab

        void StartHunt(PlayerAgent p)
        {
            _lastSeen = Time.time;
            _lastKnown = p.transform.position;
            if (State == HollowState.Hunt && Target == p) return;
            SetState(HollowState.Hunt, HuntSpeed);
            Target = p;
            SoundBank.PlayAt(SoundBank.Breath, Eyes.position, 1f, 35f, 0.8f);
        }

        void UpdateHunt()
        {
            var p = Target;
            if (!CanTarget(p))
            {
                BeginRoam();
                return;
            }

            if ((!p.InHiding && CanSee(p)) || (Time.time < _trackUntil && Director.HouseAt(p.transform.position) == null))
            {
                _lastSeen = Time.time;
                _lastKnown = p.transform.position;
            }

            if (Sheltered(p.transform.position))
            {
                if (Time.time - _lastSeen > 2f) BeginRoam(); // it waits at the edge, then loses interest
                SetDestination(_lastKnown);
                return;
            }

            // A house it may not enter yet: it stops at the door and tests it instead.
            var theirHouse = Director.HouseAt(_lastKnown);
            var ourHouse = Director.HouseAt(transform.position);
            if (theirHouse != null && theirHouse != ourHouse)
            {
                bool canGoIn = WalksInFreely(theirHouse) || (theirHouse.Door.IsOpen && MayEnter());
                if (!canGoIn)
                {
                    BeginTestHouse(theirHouse);
                    return;
                }
                if (!theirHouse.Door.IsOpen && Vector3.Distance(transform.position, DoorStep(theirHouse)) < 2.5f)
                    theirHouse.Door.ForceOpen();
            }

            SetDestination(_lastKnown);

            if (p.InHiding)
            {
                if (p.Hiding.BreathingAudibleFrom(Eyes.position))
                {
                    p.Hiding.Leave();
                    p.Notify("It heard you breathing.");
                    BeginGrab(p);
                }
            }
            else if (Vector3.Distance(transform.position, p.transform.position) <= GrabRange)
            {
                BeginGrab(p);
                return;
            }

            if (Time.time - _lastSeen > HuntMemorySeconds) BeginInvestigate(_lastKnown, InvestigateSpeed);
        }

        void BeginGrab(PlayerAgent p)
        {
            SetState(HollowState.Grab, 0f);
            Target = p;
            _grabTimer = 0f;
            _agent.ResetPath();
            p.SetCondition(PlayerCondition.Grabbed);
            var toHollow = Eyes.position - p.Head.position;
            p.Motor.SetLook(Mathf.Atan2(toHollow.x, toHollow.z) * Mathf.Rad2Deg, -Mathf.Asin(Mathf.Clamp(toHollow.normalized.y, -1f, 1f)) * Mathf.Rad2Deg);
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(-toHollow, Vector3.up));
            p.Notify("It has you.");
            SoundBank.PlayAt(SoundBank.Breath, Eyes.position, 1f, 20f, 0.7f);
        }

        void UpdateGrab()
        {
            var p = Target;
            if (p == null || p.Condition != PlayerCondition.Grabbed)
            {
                BeginRoam();
                return;
            }
            _grabTimer += Time.deltaTime;
            if (_grabTimer < GrabSeconds) return;

            var house = Director.HouseAt(p.transform.position);
            string where = house != null ? $"in {house.Def.Name}" : "in the streets";
            p.SetCondition(PlayerCondition.Dead, $"Taken by the Hollow {where} at {MatchDirector.FormatClock(Director.Clock)}.");
            p.Frozen = true;
            BeginRoam();
        }

        /// <summary>Grab progress for the HUD (0..1), or 0 if nobody is held.</summary>
        public float GrabProgress01(PlayerAgent p) => State == HollowState.Grab && Target == p ? Mathf.Clamp01(_grabTimer / GrabSeconds) : 0f;

        // ------------------------------------------------------------ the chant

        void BeginRitual()
        {
            SetState(HollowState.Ritual, InvestigateSpeed);
            Target = null;
            _candle = null;
            _nextSnuff = Time.time + SnuffInterval * 0.5f;
        }

        /// <summary>It circles the square and puts candles out (DESIGN.md "The Ritual of Passage").</summary>
        void UpdateRitual()
        {
            var circle = Director.Circle;
            if (!circle.Chant.Started || circle.Chant.Complete)
            {
                BeginRoam();
                return;
            }

            if (_candle == null && Time.time >= _nextSnuff)
            {
                float best = float.MaxValue;
                foreach (var c in circle.Candles)
                {
                    if (!c.InUse || !c.Lit) continue;
                    float d = Vector3.Distance(transform.position, c.transform.position);
                    if (d < best) { best = d; _candle = c; }
                }
            }

            var center = circle.transform.position;
            if (_candle != null)
            {
                if (!_candle.Lit)
                {
                    _candle = null;
                    return;
                }
                var outward = Vector3.ProjectOnPlane(_candle.transform.position - center, Vector3.up).normalized;
                SetDestination(center + outward * (circle.Radius + 0.9f));
                if (Vector3.Distance(Flat(transform.position), Flat(_candle.transform.position)) <= 2.4f)
                {
                    _candle.Snuff();
                    SoundBank.PlayAt(SoundBank.Breath, _candle.transform.position, 0.8f, 20f, 1.4f);
                    _candle = null;
                    _nextSnuff = Time.time + SnuffInterval;
                }
            }
            else
            {
                float angle = Time.time * 0.12f;
                SetDestination(center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * (circle.Radius + 5f));
            }
        }

        static Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);

        // ------------------------------------------------------------ helpers

        void SetDestination(Vector3 target)
        {
            if (!_agent.enabled || !_agent.isOnNavMesh) return;
            if (NavMesh.SamplePosition(target, out var hit, 4f, NavMesh.AllAreas)) target = hit.position;
            _agent.SetDestination(target);
        }

        bool Arrived(float within) => _agent.enabled && !_agent.pathPending && _agent.remainingDistance <= within;

        /// <summary>Heard far more often than seen: footsteps while it moves, and breathing now and then.</summary>
        void MakeItsOwnNoise()
        {
            float speed = _agent.velocity.magnitude;
            if (speed > 0.3f && Time.time >= _nextStep)
            {
                SoundBank.PlayAt(SoundBank.Step, transform.position, State == HollowState.Hunt ? 1f : 0.6f, 25f, 0.55f);
                _nextStep = Time.time + Mathf.Clamp(1.1f / speed, 0.25f, 0.9f);
            }
            if (Time.time >= _nextBreath)
            {
                SoundBank.PlayAt(SoundBank.Breath, Eyes.position, 0.5f, 15f);
                _nextBreath = Time.time + 6f + 4f * Mathf.PerlinNoise(Time.time * 0.1f, 0.9f);
            }
        }

        public void AppendDebug(List<string> lines)
        {
            var player = Director.Players.Length > 0 ? Director.Players[0] : null;
            string distance = player != null && IsActive ? $" · {Vector3.Distance(transform.position, player.transform.position):0} m away" : "";
            string target = Target != null ? $" · target {Target.DisplayName}" : "";
            string house = TargetHouse != null ? $" · house {TargetHouse.Def.Name}" : "";
            lines.Add($"Hollow: {State}{(Frozen ? " (frozen, F3)" : "")}{target}{house}{distance}");
            lines.Add($"   last heard: {LastHeard}");
        }
    }
}
