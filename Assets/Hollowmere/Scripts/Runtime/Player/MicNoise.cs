using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Turns the player's microphone loudness into noise the Hollow can hear (M toggles it).
    /// Loudness to radius: NoiseSystem.VoiceRadius. Design: docs/DESIGN.md "Player systems", "Voice is noise".
    /// </summary>
    public class MicNoise : MonoBehaviour
    {
        public PlayerAgent Agent;
        [Tooltip("While speaking, one voice noise event this often (seconds).")]
        public float EmitInterval = 0.25f;

        public bool Listening { get; private set; }
        public float Rms { get; private set; }
        public float Radius { get; private set; }
        public string Device { get; private set; }

        const int SampleRate = 16000;
        const int Window = 1024;
        AudioClip _clip;
        readonly float[] _buffer = new float[Window];
        float[] _tail = new float[0];
        float _timer;

        void Update()
        {
            if (GameInput.Pressed(GameKey.MicToggle) && !Agent.PanelOpen) SetListening(!Listening);
            if (!Listening || _clip == null)
            {
                Rms = 0f;
                Radius = 0f;
                return;
            }

            Rms = ReadRms(Microphone.GetPosition(Device));
            Radius = Agent.IsAlive ? NoiseSystem.VoiceRadius(Rms) : 0f;
            _timer -= Time.deltaTime;
            if (Radius > 0f && _timer <= 0f)
            {
                _timer = EmitInterval;
                NoiseSystem.Emit(Agent.Head.position, Radius, NoiseKind.Voice, Agent.Index);
            }
        }

        public void SetListening(bool on)
        {
            if (on == Listening) return;
            if (on)
            {
                if (Microphone.devices.Length == 0)
                {
                    Agent.Notify("No microphone found.");
                    return;
                }
                Device = Microphone.devices[0];
                _clip = Microphone.Start(Device, true, 1, SampleRate);
                Listening = true;
                Agent.Notify("Microphone on. The Hollow can hear you.");
            }
            else
            {
                Microphone.End(Device);
                _clip = null;
                Listening = false;
                Agent.Notify("Microphone off.");
            }
        }

        /// <summary>RMS of the last Window samples before the write head of the looping mic clip.</summary>
        float ReadRms(int writeHead)
        {
            int samples = _clip.samples;
            int start = writeHead - Window;
            if (start < 0) start += samples;

            int first = Mathf.Min(Window, samples - start);
            if (first == Window)
            {
                _clip.GetData(_buffer, start);
            }
            else
            {
                // The window wraps past the end of the clip: read the tail, then the start.
                if (_tail.Length != first) _tail = new float[first];
                _clip.GetData(_tail, start);
                var head = new float[Window - first];
                _clip.GetData(head, 0);
                _tail.CopyTo(_buffer, 0);
                head.CopyTo(_buffer, first);
            }

            double sum = 0;
            for (int i = 0; i < Window; i++) sum += _buffer[i] * _buffer[i];
            return Mathf.Sqrt((float)(sum / Window));
        }

        void OnDisable()
        {
            if (Listening) Microphone.End(Device);
            Listening = false;
        }
    }
}
