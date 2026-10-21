using System.Collections.Generic;
using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// F1 debug overlay: phase and clock, the match plan with every answer, and controls for clock speed, seed and
    /// simulated player count. F2 skips to the next phase or act, F4 teleports you into your house.
    /// </summary>
    public class DebugOverlay : MonoBehaviour
    {
        public MatchDirector Director;
        public PlayerAgent Player;
        public HollowAI Hollow;
        public bool Visible;

        /// <summary>Other systems (the Hollow) add their own lines.</summary>
        public System.Action<List<string>> ExtraLines;

        static readonly float[] Speeds = { 1f, 5f, 20f, 60f };
        static readonly string[] SpeedLabels = { "1×", "5×", "20×", "60×" };
        static readonly string[] PlayerLabels = { "1", "2", "3", "4", "5", "6" };

        readonly List<string> _lines = new List<string>();
        string _seed = "";
        int _players = 1;
        int _speed;
        GUIStyle _style;

        void Update()
        {
            if (GameInput.Pressed(GameKey.DebugOverlay))
            {
                Visible = !Visible;
                if (Visible) SyncFields();
            }
            Player.DebugCapture = Visible;
            if (GameInput.Pressed(GameKey.DebugSkip)) Director.SkipToNextPhase();
            if (GameInput.Pressed(GameKey.DebugTeleportHome)) Director.TeleportHome(Player);
            if (GameInput.Pressed(GameKey.DebugFreezeMonster) && Hollow != null)
            {
                Hollow.Frozen = !Hollow.Frozen;
                Player.Notify(Hollow.Frozen ? "Debug: the Hollow is frozen." : "Debug: the Hollow moves again.");
            }
        }

        void SyncFields()
        {
            _seed = Director.Seed.ToString();
            _players = Director.Plan != null ? Director.Plan.PlayerCount : Director.SimulatedPlayers;
            _speed = 0;
            for (int i = 0; i < Speeds.Length; i++)
                if (Mathf.Abs(Speeds[i] - Director.ClockSpeed) < Mathf.Abs(Speeds[_speed] - Director.ClockSpeed)) _speed = i;
        }

        void OnGUI()
        {
            if (!Visible) return;
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(11, Mathf.RoundToInt(16f * Screen.height / 1080f)), richText = true, wordWrap = true };
                _style.normal.textColor = new Color(0.95f, 0.93f, 0.85f);
            }

            _lines.Clear();
            Director.AppendDebug(_lines);
            if (Hollow != null) Hollow.AppendDebug(_lines);
            ExtraLines?.Invoke(_lines);

            GUILayout.BeginArea(new Rect(16f, 16f, Mathf.Max(420f, Screen.width * 0.42f), Screen.height - 32f), GUI.skin.box);
            foreach (var line in _lines) GUILayout.Label(line, _style);

            GUILayout.Space(10f);
            GUILayout.Label("Clock speed (match clock only)", _style);
            int speed = GUILayout.Toolbar(_speed, SpeedLabels);
            if (speed != _speed)
            {
                _speed = speed;
                Director.ClockSpeed = Speeds[speed];
            }

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Seed", _style, GUILayout.Width(50f));
            _seed = GUILayout.TextField(_seed, GUILayout.Width(140f));
            if (GUILayout.Button("Random", GUILayout.Width(90f))) _seed = "";
            GUILayout.EndHorizontal();
            GUILayout.Label("Simulated players (you play every one of their houses)", _style);
            _players = GUILayout.Toolbar(_players - 1, PlayerLabels) + 1;
            if (GUILayout.Button("Restart with this seed and player count"))
                Director.Restart(int.TryParse(_seed, out var seed) && seed >= 0 ? seed : -1, _players, Speeds[_speed]);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Skip phase (F2)")) Director.SkipToNextPhase();
            if (GUILayout.Button("Teleport home (F4)")) Director.TeleportHome(Player);
            GUILayout.EndHorizontal();
            GUILayout.Label("F1 closes this panel. F3 freezes the Hollow.", _style);
            GUILayout.EndArea();
        }
    }
}
