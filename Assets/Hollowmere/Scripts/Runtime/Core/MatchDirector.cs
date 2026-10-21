using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hollowmere
{
    public enum MatchOutcome { Running, Won, Lost }

    /// <summary>
    /// The match authority. Generates the MatchPlan, applies it to the village, runs the clock through
    /// Arrival → Day → Dusk → Night (three acts) → dawn, and fires the timed events: the dusk tolls and latching,
    /// the midnight bell and unlatching, the minute-17 power cut, and the dawn loss. Milestone 2 runs this on the
    /// host only. Design: docs/DESIGN.md "Core loop", "Dusk transition", "Night phase".
    /// </summary>
    public class MatchDirector : MonoBehaviour
    {
        [Header("Match setup (debug)")]
        public bool RandomSeed = true;
        public int Seed;
        [Tooltip("One person plays this many players' houses.")]
        [Range(1, 6)] public int SimulatedPlayers = 1;
        [Tooltip("Speeds up the match clock only; movement stays normal.")]
        public float ClockSpeed = 1f;

        public TimelineSettings Timeline = new TimelineSettings();

        [Header("Starter kit, DESIGN.md \"Core loop\" (the match count is a placeholder)")]
        public int StarterMatches = 5;

        [Header("Scene")]
        public House[] Houses;              // sorted by House.Index
        public PlayerAgent[] Players;
        public VillageLayout Layout;
        public NoticeBoard Board;
        public Readable RitualBook;
        public BellTower Bell;
        public PhaseLighting Lighting;
        public VillageGate Gate;

        public MatchPlan Plan { get; private set; }
        public float Clock { get; private set; }
        public TimelinePoint Now => MatchTimeline.At(Timeline, Clock);
        public MatchOutcome Outcome { get; private set; }
        public string OutcomeText { get; private set; } = "";

        /// <summary>Stops the clock, e.g. once the gate is open and dawn no longer matters.</summary>
        public bool ClockStopped { get; set; }

        /// <summary>House puzzles only work at night (docs/DESIGN.md "Decisions": puzzles by day).</summary>
        public bool PuzzlesActive => Now.Phase == MatchPhase.Night;

        public event Action NightStarted, MidnightBell, Act3Started, PowerCut;

        static readonly string[] TravellerNames = { "Alex", "Sam", "Robin", "Jo", "Kit", "Lee" };
        static readonly string[] Keepsakes = { "a pocket watch", "a photograph", "a ring", "a lighter", "a locket", "a harmonica" };

        bool _gateShut;

        void Awake()
        {
            if (MatchLaunch.Next.HasValue)
            {
                var next = MatchLaunch.Next.Value;
                MatchLaunch.Next = null;
                RandomSeed = next.Seed < 0;
                Seed = next.Seed;
                SimulatedPlayers = next.Players;
                ClockSpeed = next.ClockSpeed;
            }
            if (RandomSeed) Seed = Environment.TickCount & 0x7fffffff; // the seed source; MatchSetup does the rolling
        }

        void Start() => BeginMatch();

        void BeginMatch()
        {
            int count = Mathf.Clamp(SimulatedPlayers, 1, 6);
            var names = TravellerNames.Take(count).ToArray();
            Plan = MatchSetup.Generate(Seed, count, names);
            ApplyPlan();

            foreach (var p in Players)
            {
                p.DisplayName = names[Mathf.Min(p.Index, count - 1)];
                p.Inventory = new Inventory { Matches = StarterMatches, Keepsake = Keepsakes[p.Index % Keepsakes.Length] };
                p.Motor.Teleport(Layout.PlayerSpawn.position, Layout.PlayerSpawn.eulerAngles.y);
            }

            Gate.SetOpen(true, instant: true);
            Lighting.Crossing = false;
            Lighting.Apply(Now);
            NotifyAll("Your car died at the broken bridge. The only road leads into Hollowmere.");
        }

        /// <summary>Allotted houses, shrines, hearths, letters and the Ritual Book (docs/DESIGN.md "Replayability").</summary>
        void ApplyPlan()
        {
            foreach (var h in Houses)
            {
                bool lived = Plan.IsAllotted(h.Index) || Plan.IsShrine(h.Index);
                h.Hearth.SetLit(lived);              // guests' houses and shrine houses keep their hearths lit
                h.ShrineMarker.SetActive(Plan.IsShrine(h.Index));
                h.FuseBox.SetPowered(true);
                h.SetLightsOn(lived);                // hollow houses stand dark
            }
            Board.Post(Plan);
            RitualBook.Set("The Ritual Book", Plan.RitualBookText(), "Read the Ritual Book");
        }

        void Update()
        {
            if (Outcome == MatchOutcome.Running && !ClockStopped)
            {
                float before = Clock;
                Clock += Time.deltaTime * Mathf.Max(0f, ClockSpeed);
                Fire(before, Clock);
            }
            UpdateArrival();
            Lighting.Apply(Now);

            if (Outcome != MatchOutcome.Running && GameInput.Pressed(GameKey.Restart))
                Restart(-1, Plan.PlayerCount, ClockSpeed);
        }

        void Fire(float before, float after)
        {
            var s = Timeline;
            bool Crossed(float moment) => MatchTimeline.Crossed(before, after, moment);

            if (Crossed(MatchTimeline.DayStart(s)))
                NotifyAll("Day. One letter on the notice board has your name on it.");
            if (Crossed(MatchTimeline.WarningTollAt(s)))
            {
                Bell.Toll(0.5f);
                NotifyAll("A single toll from the bell tower. The day is nearly gone.");
            }
            if (Crossed(MatchTimeline.DuskStart(s)))
                NotifyAll("Dusk. Be inside your house before the last toll.");
            for (int k = 0; k < s.DuskTolls; k++)
                if (Crossed(MatchTimeline.TollTime(s, k)))
                    Bell.Toll(Mathf.Lerp(0.35f, 1f, k / (float)Mathf.Max(1, s.DuskTolls - 1))); // each toll louder than the last
            if (Crossed(MatchTimeline.NightStart(s))) StartNight();
            if (Crossed(MatchTimeline.Act2Start(s))) StartAct2();
            if (Crossed(MatchTimeline.Act3Start(s))) StartAct3();
            if (Crossed(MatchTimeline.PowerCutAt(s))) CutPower();
            if (Crossed(MatchTimeline.DawnAt(s)))
            {
                Bell.Toll(3, 1f);
                End(MatchOutcome.Lost, "The dawn bell rang before the gate opened. The village keeps its guests.");
            }
        }

        /// <summary>The guests walk in and the gate swings shut behind them (docs/DESIGN.md "Core loop", Arrival).</summary>
        void UpdateArrival()
        {
            if (_gateShut || Players.Length == 0) return;
            bool allInside = Players.All(p => p.transform.position.z < Layout.GateLineZ - 1.5f);
            if (!allInside && Clock < MatchTimeline.DayStart(Timeline)) return;

            foreach (var p in Players)
                if (p.transform.position.z >= Layout.GateLineZ - 1.5f)
                    p.Motor.Teleport(new Vector3(p.transform.position.x * 0.3f, 0.1f, Layout.GateLineZ - 4f), 180f);
            Gate.SetOpen(false);
            _gateShut = true;
            NotifyAll("The gate swings shut behind you.");
        }

        void StartNight()
        {
            foreach (var h in Houses) h.Door.CloseAndLatch();
            NotifyAll("The last toll fades. Every door in the village swings shut and latches.");
            foreach (var p in Players)
                if (p.IsAlive && HouseAt(p.transform.position) == null)
                    p.Notify("You're outside. It's coming. Find a door.");
            NightStarted?.Invoke();
        }

        void StartAct2()
        {
            int living = Players.Count(p => p.IsAlive);
            Bell.Toll(Mathf.Max(1, living), 1f); // once per living player (docs/DESIGN.md "Night phase", Act 2)
            foreach (var h in Houses) h.Door.Unlatch();
            NotifyAll("Midnight. The bell counts the living. The doors unlatch.");
            MidnightBell?.Invoke();
        }

        void StartAct3()
        {
            NotifyAll("The fog thickens.");
            Act3Started?.Invoke();
        }

        void CutPower()
        {
            foreach (var h in Houses) h.FuseBox.Trip();
            NotifyAll("Every light in the village dies at once.");
            PowerCut?.Invoke();
        }

        public void End(MatchOutcome outcome, string text)
        {
            if (Outcome != MatchOutcome.Running) return;
            Outcome = outcome;
            OutcomeText = text;
            ClockStopped = true;
            foreach (var p in Players)
            {
                p.Frozen = true;
                p.ResultsCapture = true;
            }
        }

        // ------------------------------------------------------------ queries

        public House HouseAt(Vector3 worldPos)
        {
            foreach (var h in Houses)
                if (h.Contains(worldPos)) return h;
            return null;
        }

        /// <summary>The house a player's letter describes. With simulated players, the first allotted house is yours.</summary>
        public House HomeOf(PlayerAgent player)
        {
            if (Plan == null) return null;
            return Houses[Plan.AllottedHouses[Mathf.Min(player.Index, Plan.PlayerCount - 1)]];
        }

        public void NotifyAll(string text)
        {
            foreach (var p in Players) p.Notify(text);
        }

        // ------------------------------------------------------------ debug

        public void SkipToNextPhase()
        {
            if (Outcome != MatchOutcome.Running) return;
            float before = Clock;
            Clock = MatchTimeline.NextBoundary(Timeline, Clock);
            Fire(before, Clock);
        }

        public void TeleportHome(PlayerAgent player)
        {
            var home = HomeOf(player);
            if (home == null || !player.IsAlive || player.InHiding) return;
            var hall = home.Rooms[0].Bounds;
            player.Motor.Teleport(new Vector3(hall.center.x, home.transform.position.y + 0.1f, hall.center.z), 180f);
        }

        /// <summary>Reloads the scene with new settings. A negative seed picks a new one.</summary>
        public void Restart(int seed, int players, float clockSpeed)
        {
            MatchLaunch.Next = new MatchLaunch.Settings { Seed = seed, Players = players, ClockSpeed = clockSpeed };
            SceneManager.LoadScene(gameObject.scene.name);
        }

        public static string FormatClock(float seconds)
            => $"{(int)(seconds / 60f):00}:{(int)(seconds % 60f):00}";

        public string AnswerText(int house)
        {
            var s = Plan.Solutions[house];
            switch (HouseCatalog.Houses[house].Puzzle)
            {
                case PuzzleKind.CodeLock: return "code " + string.Concat(s.Code);
                case PuzzleKind.Clock: return $"clock at {s.ClockHour}";
                default: return string.Join(" ", s.Sequence.Select(i => HouseCatalog.SequenceSymbols[i]));
            }
        }

        public void AppendDebug(List<string> lines)
        {
            if (Plan == null) return;
            var now = Now;
            string act = now.Act > 0 ? $" act {now.Act}" : "";
            lines.Add($"{now.Phase}{act}   clock {FormatClock(Clock)}   {FormatClock(now.PhaseRemaining)} left   ×{ClockSpeed:0.#}");
            lines.Add($"Seed {Seed}   players {Plan.PlayerCount}   Hollow: {Plan.Variant}");
            for (int p = 0; p < Plan.PlayerCount; p++)
            {
                var h = Houses[Plan.AllottedHouses[p]];
                lines.Add($"{Plan.PlayerNames[p]}: {h.Def.Name} · {h.Def.Part} · slot {Plan.SlotFor(h.Index) + 1} · {AnswerText(h.Index)}");
            }
            lines.Add("Shrines: " + string.Join(", ", Plan.ShrineHouses.Select(i => Houses[i].Def.Name)));
        }
    }
}
