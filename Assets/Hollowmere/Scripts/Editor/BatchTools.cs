using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Entry points for tools/unity.ps1 (Unity batch mode). Each method ends the editor itself
    /// with EditorApplication.Exit, so the wrapper never needs -quit for these.
    /// Custom arguments use the -hm prefix, e.g. -hmScene Assets/.../Greybox.unity
    /// </summary>
    public static class BatchTools
    {
        const string Tag = "[Hollowmere]";
        const string ClaudeLogDir = "Logs/claude";

        /// <summary>Packages the game needs. Added by name so Unity picks the version that matches the editor.</summary>
        static readonly string[] RequiredPackages =
        {
            "com.unity.render-pipelines.universal",
            "com.unity.inputsystem",
            "com.unity.ai.navigation",
            "com.unity.netcode.gameobjects",
            "com.unity.test-framework",
        };

        static readonly string[] ProjectFolders =
        {
            "Assets/Hollowmere/Art",
            "Assets/Hollowmere/Audio",
            "Assets/Hollowmere/Materials",
            "Assets/Hollowmere/Prefabs",
            "Assets/Hollowmere/Scenes",
            "Assets/Hollowmere/Settings",
            "Assets/Hollowmere/Scripts/Editor/Builders",
        };

        // ---------------------------------------------------------------- check

        /// <summary>Reaching this method at all means every assembly compiled.</summary>
        public static void CompileCheck()
        {
            Debug.Log($"{Tag} COMPILE OK (Unity {Application.unityVersion})");
            EditorApplication.Exit(0);
        }

        // ---------------------------------------------------------------- info

        public static void Info()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Unity version: {Application.unityVersion}");
            sb.AppendLine($"Product: {PlayerSettings.productName}");
            var rp = GraphicsSettings.defaultRenderPipeline;
            sb.AppendLine($"Render pipeline: {(rp != null ? rp.GetType().Name + " (" + rp.name + ")" : "Built-in")}");
            sb.AppendLine("Scenes in build settings:");
            foreach (var s in EditorBuildSettings.scenes)
                sb.AppendLine($"  [{(s.enabled ? "x" : " ")}] {s.path}");
            sb.AppendLine("Packages/manifest.json dependencies:");
            var manifestPath = "Packages/manifest.json";
            if (File.Exists(manifestPath))
            {
                foreach (var line in File.ReadAllLines(manifestPath))
                {
                    var t = line.Trim();
                    if (t.StartsWith("\"com.")) sb.AppendLine("  " + t.TrimEnd(','));
                }
            }

            Directory.CreateDirectory(ClaudeLogDir);
            File.WriteAllText(Path.Combine(ClaudeLogDir, "info.txt"), sb.ToString());
            Debug.Log($"{Tag} INFO\n{sb}");
            EditorApplication.Exit(0);
        }

        // ---------------------------------------------------------------- setup

        static AddAndRemoveRequest s_packageRequest;

        /// <summary>Creates the folder layout, sets player settings and adds any missing packages.</summary>
        public static void Setup()
        {
            foreach (var folder in ProjectFolders)
                Directory.CreateDirectory(folder);
            AssetDatabase.Refresh();

            PlayerSettings.productName = "Hollowmere";
            PlayerSettings.runInBackground = true; // needed when testing two game instances on one PC

            var manifest = File.Exists("Packages/manifest.json") ? File.ReadAllText("Packages/manifest.json") : "";
            var missing = RequiredPackages.Where(p => !manifest.Contains("\"" + p + "\"")).ToArray();
            if (missing.Length == 0)
            {
                Debug.Log($"{Tag} SETUP OK: folders ready, all required packages already installed.");
                EditorApplication.Exit(0);
                return;
            }

            Debug.Log($"{Tag} Adding packages: {string.Join(", ", missing)}");
            s_packageRequest = Client.AddAndRemove(missing, null);
            EditorApplication.update += WaitForPackages;
        }

        static void WaitForPackages()
        {
            if (s_packageRequest == null || !s_packageRequest.IsCompleted) return;
            EditorApplication.update -= WaitForPackages;

            if (s_packageRequest.Status == StatusCode.Success)
            {
                Debug.Log($"{Tag} SETUP OK: packages added. Run 'check' next; the first import after this can take a few minutes.");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"{Tag} SETUP FAILED: {s_packageRequest.Error?.message}");
                EditorApplication.Exit(1);
            }
        }

        // ---------------------------------------------------------------- build

        public static void BuildWindows()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length == 0)
            {
                Debug.LogError($"{Tag} BUILD FAILED: no enabled scenes in Build Settings.");
                EditorApplication.Exit(1);
                return;
            }

            var outPath = Arg("-hmOut") ?? "Builds/Windows/Hollowmere.exe";
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            });

            var summary = report.summary;
            bool ok = summary.result == BuildResult.Succeeded;
            Debug.Log($"{Tag} BUILD {(ok ? "OK" : "FAILED")}: {summary.result}, {summary.totalErrors} errors, " +
                      $"{summary.totalSize / (1024 * 1024)} MB, {summary.totalTime.TotalSeconds:F0} s -> {outPath}");
            EditorApplication.Exit(ok ? 0 : 1);
        }

        // ---------------------------------------------------------------- screenshot

        /// <summary>
        /// Renders a scene's main camera to a PNG so Claude Code can look at the result.
        /// Args: -hmScene path/to/scene.unity (optional), -hmOut path.png (optional),
        ///       -hmCamera GameObjectName (optional, defaults to the MainCamera).
        /// Must run WITHOUT -nographics.
        /// </summary>
        public static void CaptureScreenshot()
        {
            try
            {
                var scenePath = Arg("-hmScene");
                if (!string.IsNullOrEmpty(scenePath))
                    EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

                Camera cam = null;
                var camName = Arg("-hmCamera");
                if (!string.IsNullOrEmpty(camName))
                {
                    var go = GameObject.Find(camName);
                    if (go != null) cam = go.GetComponentInChildren<Camera>();
                }
                if (cam == null) cam = Camera.main;
                if (cam == null) cam = UnityEngine.Object.FindAnyObjectByType<Camera>();
                if (cam == null)
                {
                    Debug.LogError($"{Tag} SHOT FAILED: no camera in the scene.");
                    EditorApplication.Exit(1);
                    return;
                }

                var outPath = Arg("-hmOut") ?? Path.Combine(ClaudeLogDir, "screenshot.png");
                RenderToPng(cam, outPath);
                Debug.Log($"{Tag} SHOT OK: {outPath} (camera '{cam.name}')");
                EditorApplication.Exit(0);
            }
            catch (Exception e)
            {
                Debug.LogError($"{Tag} SHOT FAILED: {e}");
                EditorApplication.Exit(1);
            }
        }

        /// <summary>Renders one camera to a 1600x900 PNG. Needs graphics (not -nographics).</summary>
        public static void RenderToPng(Camera cam, string outPath)
        {
            const int w = 1600, h = 900;
            var rt = new RenderTexture(w, h, 24, RenderTextureFormat.ARGB32);
            var previousTarget = cam.targetTexture;
            cam.targetTexture = rt;
            cam.Render();

            var previousActive = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
            tex.Apply();
            RenderTexture.active = previousActive;
            cam.targetTexture = previousTarget;

            var dir = Path.GetDirectoryName(outPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            File.WriteAllBytes(outPath, tex.EncodeToPNG());

            UnityEngine.Object.DestroyImmediate(tex);
            rt.Release();
            UnityEngine.Object.DestroyImmediate(rt);
        }

        // ---------------------------------------------------------------- helpers

        /// <summary>Reads a custom command-line argument value, e.g. Arg("-hmScene").</summary>
        public static string Arg(string name)
        {
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            return null;
        }
    }
}
