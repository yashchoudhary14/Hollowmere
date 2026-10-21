using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// The milestone-1 HUD, drawn with OnGUI so it needs no assets: crosshair, prompt and hold bar, inventory,
    /// stamina, flashlight, microphone, messages and the reading panel. It only reads state.
    /// </summary>
    public class Hud : MonoBehaviour
    {
        public PlayerAgent Player;
        public MatchDirector Director;
        public HollowAI Hollow;

        GUIStyle _text, _center, _small, _hint, _title, _body;
        Texture2D _pixel;
        float _scale;
        Vector2 _scroll;

        static readonly Color Ink = new Color(0.93f, 0.91f, 0.85f);
        static readonly Color Dim = new Color(0.65f, 0.63f, 0.58f);
        static readonly Color Panel = new Color(0.05f, 0.045f, 0.04f, 0.92f);

        void OnGUI()
        {
            if (Player == null) return;
            Setup();
            if (Director != null && Director.Outcome != MatchOutcome.Running)
            {
                DrawOutcome();
                return;
            }
            if (Player.PanelOpen)
            {
                DrawPanel();
                return;
            }
            if (!Player.IsAlive) return;
            if (Player.Condition == PlayerCondition.Grabbed)
            {
                DrawGrabbed();
                return;
            }
            if (Player.InHiding) DrawHidingView();
            DrawChant();
            DrawCrosshair();
            DrawPrompt();
            DrawBars();
            DrawInventory();
            DrawMessages();
            if (Player.Paused) DrawCentered("Paused. The night goes on. Click to return.", Screen.height * 0.35f);
        }

        // ------------------------------------------------------------ setup and helpers

        void Setup()
        {
            float s = Screen.height / 1080f;
            if (_text != null && Mathf.Approximately(s, _scale)) return;
            _scale = s;
            if (_pixel == null)
            {
                _pixel = new Texture2D(1, 1);
                _pixel.SetPixel(0, 0, Color.white);
                _pixel.Apply();
            }
            _text = new GUIStyle(GUI.skin.label) { fontSize = Px(22), wordWrap = false, richText = true };
            _text.normal.textColor = Ink;
            _center = new GUIStyle(_text) { alignment = TextAnchor.MiddleCenter };
            _small = new GUIStyle(_text) { fontSize = Px(17) };
            _hint = new GUIStyle(_center) { fontSize = Px(17) };
            _hint.normal.textColor = Dim;
            _title = new GUIStyle(_text) { fontSize = Px(30), fontStyle = FontStyle.Bold, wordWrap = true };
            _body = new GUIStyle(_text) { fontSize = Px(22), wordWrap = true };
        }

        int Px(float v) => Mathf.Max(8, Mathf.RoundToInt(v * _scale));

        public void Fill(Rect r, Color c)
        {
            var old = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, _pixel);
            GUI.color = old;
        }

        void Shadowed(Rect r, string text, GUIStyle style)
        {
            var c = style.normal.textColor;
            style.normal.textColor = new Color(0f, 0f, 0f, 0.85f);
            GUI.Label(new Rect(r.x + 2f * _scale, r.y + 2f * _scale, r.width, r.height), text, style);
            style.normal.textColor = c;
            GUI.Label(r, text, style);
        }

        void DrawCentered(string text, float y)
        {
            Shadowed(new Rect(0f, y, Screen.width, Px(40)), text, _center);
        }

        void Bar(Rect r, float value01, Color color)
        {
            Fill(r, new Color(0f, 0f, 0f, 0.55f));
            Fill(new Rect(r.x, r.y, r.width * Mathf.Clamp01(value01), r.height), color);
        }

        public static string ItemName(ItemKind kind)
        {
            switch (kind)
            {
                case ItemKind.WindingKey: return "Winding key";
                case ItemKind.Matches: return "Matches";
                case ItemKind.Battery: return "Battery";
                case ItemKind.RitualPart: return "Ritual part";
                default: return "—";
            }
        }

        // ------------------------------------------------------------ pieces

        void DrawCrosshair()
        {
            float d = Px(4);
            Fill(new Rect((Screen.width - d) * 0.5f, (Screen.height - d) * 0.5f, d, d), new Color(1f, 1f, 1f, 0.7f));
        }

        void DrawPrompt()
        {
            var target = Player.Interactor != null ? Player.Interactor.Target : null;
            if (target == null) return;
            string prompt = target.Prompt(Player);
            if (string.IsNullOrEmpty(prompt)) return;

            bool can = target.CanInteract(Player);
            float y = Screen.height * 0.5f + Px(36);
            var style = can ? _center : _hint;
            Shadowed(new Rect(0f, y, Screen.width, Px(32)), can ? $"<b>[E]</b>  {prompt}" : prompt, style);

            float hold = Player.Interactor.HoldProgress01;
            if (hold > 0f)
            {
                float w = Px(220), h = Px(6);
                Bar(new Rect((Screen.width - w) * 0.5f, y + Px(36), w, h), hold, Ink);
            }
        }

        void DrawBars()
        {
            float w = Px(200), h = Px(8), x = Screen.width - w - Px(28), y = Screen.height - Px(40);

            var mic = Player.Mic;
            string micText = mic == null ? "" : mic.Listening
                ? (mic.Radius > 0f ? $"Mic on · heard {mic.Radius:0} m away" : "Mic on · quiet")
                : "Mic off (M)";
            Shadowed(new Rect(x, y - Px(6), w, Px(26)), micText, _small);

            var light = Player.Flashlight;
            if (light != null)
            {
                y -= Px(42);
                Shadowed(new Rect(x, y - Px(24), w, Px(24)), light.IsOn ? "Flashlight (F)" : "Flashlight off (F)", _small);
                Bar(new Rect(x, y, w, h), light.Charge01, light.Charge01 < light.FlickerBelow ? new Color(0.85f, 0.35f, 0.25f) : new Color(0.95f, 0.85f, 0.55f));
            }

            var motor = Player.Motor;
            if (motor != null && (motor.Stamina01 < 0.999f || motor.IsSprinting))
            {
                y -= Px(42);
                Shadowed(new Rect(x, y - Px(24), w, Px(24)), Player.Inventory.HasPart ? "Breath (can't run while carrying)" : "Breath", _small);
                Bar(new Rect(x, y, w, h), motor.Stamina01, new Color(0.7f, 0.8f, 0.85f));
            }
        }

        void DrawInventory()
        {
            var inv = Player.Inventory;
            float x = Px(28), y = Screen.height - Px(40), line = Px(28);

            Shadowed(new Rect(x, y, Px(900), line),
                $"Matches ×{inv.Matches}    Spare batteries ×{inv.Batteries}    Keepsake: {inv.Keepsake}", _small);
            y -= line;
            if (inv.HasPart)
            {
                Shadowed(new Rect(x, y, Px(900), line), $"Ritual part: <b>{HouseCatalog.Houses[inv.RitualPart].Part}</b>   (G drops it)", _text);
                y -= line;
            }
            for (int i = Inventory.HandSlotCount - 1; i >= 0; i--)
            {
                string name = ItemName(inv.Hands[i]);
                string text = i == inv.Selected ? $"<b>[{i + 1}] {name}</b>" : $"[{i + 1}] {name}";
                Shadowed(new Rect(x, y, Px(600), line), text, i == inv.Selected ? _text : _small);
                y -= line;
            }
        }

        /// <summary>Inside the wardrobe: darkness with a slit of view through the doors, and the breath meter.</summary>
        void DrawHidingView()
        {
            float slit = Screen.width * 0.16f;
            float side = (Screen.width - slit) * 0.5f;
            var dark = new Color(0f, 0f, 0f, 0.96f);
            Fill(new Rect(0f, 0f, side, Screen.height), dark);
            Fill(new Rect(side + slit, 0f, side, Screen.height), dark);

            float w = Px(260), h = Px(8), y = Screen.height * 0.78f;
            string label = Player.HoldingBreath ? "Holding your breath" : "Hold Space to hold your breath";
            Shadowed(new Rect(0f, y - Px(30), Screen.width, Px(26)), label, _hint);
            if (Player.Breath01 > 0f)
                Bar(new Rect((Screen.width - w) * 0.5f, y, w, h), 1f - Player.Breath01, new Color(0.7f, 0.8f, 0.85f));
        }

        /// <summary>Held by the Hollow: five seconds for a teammate to save you (rescues come with multiplayer).</summary>
        void DrawGrabbed()
        {
            float held = Hollow != null ? Hollow.GrabProgress01(Player) : 0f;
            Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.25f, 0f, 0f, 0.25f + 0.5f * held));
            DrawCentered("It has you.", Screen.height * 0.42f);
            float w = Px(300);
            Bar(new Rect((Screen.width - w) * 0.5f, Screen.height * 0.42f + Px(48), w, Px(8)), 1f - held, new Color(0.8f, 0.2f, 0.15f));
        }

        void DrawMessages()
        {
            float y = Px(24);
            foreach (var (text, time) in Player.Messages)
            {
                float age = Time.time - time;
                if (age > 5f) continue;
                var c = _center.normal.textColor;
                _center.normal.textColor = new Color(c.r, c.g, c.b, Mathf.Clamp01(5f - age));
                Shadowed(new Rect(0f, y, Screen.width, Px(30)), text, _center);
                _center.normal.textColor = c;
                y += Px(30);
            }
        }

        readonly System.Collections.Generic.List<string> _results = new System.Collections.Generic.List<string>();

        /// <summary>Results screen (docs/DESIGN.md "Results screen"): fates, the Hollow's variant, each house's puzzle.</summary>
        void DrawOutcome()
        {
            Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.88f));
            float x = Screen.width * 0.18f, w = Screen.width * 0.64f, y = Screen.height * 0.14f;
            string heading = Director.Outcome == MatchOutcome.Won ? "You made it out." : "Hollowmere keeps you.";
            GUI.Label(new Rect(x, y, w, Px(50)), heading, _title);
            y += Px(60);
            float h = _body.CalcHeight(new GUIContent(Director.OutcomeText), w);
            GUI.Label(new Rect(x, y, w, h), Director.OutcomeText, _body);
            y += h + Px(30);

            _results.Clear();
            Director.AppendResults(_results);
            foreach (var line in _results)
            {
                float lh = string.IsNullOrEmpty(line) ? Px(12) : _small.CalcHeight(new GUIContent(line), w);
                GUI.Label(new Rect(x, y, w, lh), line, _small);
                y += lh + Px(4);
            }
            GUI.Label(new Rect(0f, Screen.height - Px(80), Screen.width, Px(30)), "Press R to play again", _hint);
        }

        void DrawChant()
        {
            var circle = Director != null ? Director.Circle : null;
            if (circle == null || !circle.Chant.Started || circle.Chant.Complete) return;
            string text;
            switch (circle.Status)
            {
                case ChantStatus.Paused: text = "The chant falters. Everyone into the circle."; break;
                case ChantStatus.Guttering: text = "Every candle is out. Relight one, quickly!"; break;
                case ChantStatus.Reset: text = "The chant died. Relight a candle to start again."; break;
                default: text = $"The chant: {Mathf.CeilToInt(circle.Chant.Seconds - circle.Chant.Progress)} s"; break;
            }
            float w = Px(360);
            Shadowed(new Rect(0f, Px(150), Screen.width, Px(30)), text, _center);
            Bar(new Rect((Screen.width - w) * 0.5f, Px(186), w, Px(8)), circle.Chant.Progress01, new Color(0.95f, 0.75f, 0.4f));
        }

        void DrawPanel()
        {
            float w = Mathf.Min(Screen.width * 0.6f, Px(900)), h = Screen.height * 0.72f;
            var r = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
            Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0f, 0f, 0f, 0.5f));
            Fill(r, Panel);

            float pad = Px(28);
            var inner = new Rect(r.x + pad, r.y + pad, r.width - 2f * pad, r.height - 2f * pad);
            GUI.Label(new Rect(inner.x, inner.y, inner.width, Px(44)), Player.PanelTitle ?? "", _title);

            var bodyRect = new Rect(inner.x, inner.y + Px(56), inner.width, inner.height - Px(96));
            float bodyHeight = _body.CalcHeight(new GUIContent(Player.PanelText), bodyRect.width - Px(20));
            _scroll = GUI.BeginScrollView(bodyRect, _scroll, new Rect(0f, 0f, bodyRect.width - Px(20), bodyHeight));
            GUI.Label(new Rect(0f, 0f, bodyRect.width - Px(20), bodyHeight), Player.PanelText, _body);
            GUI.EndScrollView();

            GUI.Label(new Rect(inner.x, inner.yMax - Px(30), inner.width, Px(30)), "E or Esc to close", _hint);
        }
    }
}
