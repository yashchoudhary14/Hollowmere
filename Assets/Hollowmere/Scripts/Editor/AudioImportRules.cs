using UnityEditor;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Import rules for the recorded sound in Assets/Hollowmere/Audio and Assets/ThirdParty, so nothing is set by hand.
    /// The sources stay 48 kHz WAV in the project; every clip is compressed to Vorbis on import.
    /// - Ambience and ritual loops are long stereo beds: streamed from disk, loaded in the background.
    /// - Everything else (footsteps, doors, bells, voices) is a short one-shot played in 3D: mono, decompressed on
    ///   load so it starts without delay.
    /// </summary>
    public class AudioImportRules : AssetPostprocessor
    {
        public const string AudioRoot = "Assets/Hollowmere/Audio/";

        public static bool Applies(string path)
        {
            path = path.Replace('\\', '/');
            return path.StartsWith(AudioRoot) || ThirdPartyImport.IsThirdParty(path);
        }

        public static bool IsLoop(string path)
        {
            path = path.Replace('\\', '/');
            return path.Contains("/Ambience/") || path.Contains("/Ritual/");
        }

        void OnPreprocessAudio()
        {
            if (!Applies(assetPath)) return;
            var ai = (AudioImporter)assetImporter;
            bool loop = IsLoop(assetPath);
            ai.forceToMono = !loop;
            ai.loadInBackground = loop;
            var s = ai.defaultSampleSettings;
            s.compressionFormat = AudioCompressionFormat.Vorbis;
            s.quality = loop ? 0.5f : 0.7f;
            s.loadType = loop ? AudioClipLoadType.Streaming : AudioClipLoadType.DecompressOnLoad;
            s.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
            s.preloadAudioData = !loop;
            ai.defaultSampleSettings = s;
            ai.ClearSampleSettingOverride("Standalone");
        }
    }
}
