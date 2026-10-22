using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// Renders the Greybox scene under runtime lighting that `shot` can't show, since PhaseLighting only runs in play.
    /// Run with: tools/unity.ps1 run -Method Hollowmere.EditorTools.LookTools.NightShots
    /// Writes Logs/claude/night_*.png.
    /// </summary>
    public static class LookTools
    {
        public static void NightShots()
        {
            EditorSceneManager.OpenScene(GreyboxVillageBuilder.ScenePath, OpenSceneMode.Single);
            var lighting = Object.FindAnyObjectByType<PhaseLighting>();
            var timeline = new TimelineSettings();
            lighting.Apply(MatchTimeline.At(timeline, MatchTimeline.NightStart(timeline) + 30f));
            RenderSettings.fogDensity = lighting.NightFogDensity; // Apply eases fog in over time; jump straight there

            foreach (var name in new[] { "Cam_Square", "Cam_House", "Cam_Interior", "Cam_Road" })
            {
                var go = GameObject.Find(name);
                if (go == null) continue;
                var cam = go.GetComponent<Camera>();
                cam.backgroundColor = RenderSettings.fogColor;
                BatchTools.RenderToPng(cam, Path.Combine("Logs/claude", $"night_{name}.png"));
            }

            // A player's eye on the street outside the first row of houses, flashlight on.
            var player = Object.FindAnyObjectByType<PlayerAgent>();
            player.transform.SetPositionAndRotation(new Vector3(-6f, 0.1f, 74f), Quaternion.Euler(0f, 200f, 0f));
            player.Flashlight.Beam.enabled = true;
            player.Flashlight.Beam.range = player.Flashlight.Range;
            player.Flashlight.Beam.intensity = player.Flashlight.Intensity;
            var eye = player.Head.GetComponent<Camera>();
            eye.backgroundColor = RenderSettings.fogColor;
            BatchTools.RenderToPng(eye, "Logs/claude/night_PlayerFlashlight.png");

            Debug.Log("[Hollowmere] NIGHT SHOTS OK: Logs/claude/night_*.png");
        }

        static readonly string[] LookCameras = { "Cam_Square", "Cam_House", "Cam_FrontYard", "Cam_Garden", "Cam_Road", "Cam_Forest", "Cam_Chapel", "Cam_Hall", "Cam_Fireside", "Cam_Clock" };

        /// <summary>
        /// The look pass's review set (M1.5): every look camera at midday, late golden day, mid-dusk and night (Act 1),
        /// with house 7's hearth lit and its flames running, plus a player's flashlight view at night.
        /// Run with: tools/unity.ps1 run -Method Hollowmere.EditorTools.LookTools.PhaseShots
        /// Writes Logs/claude/look_{phase}_{camera}.png.
        /// </summary>
        public static void PhaseShots()
        {
            EditorSceneManager.OpenScene(GreyboxVillageBuilder.ScenePath, OpenSceneMode.Single);
            var lighting = Object.FindAnyObjectByType<PhaseLighting>();
            var s = new TimelineSettings();
            var house = System.Array.Find(Object.FindObjectsByType<House>(FindObjectsSortMode.None), h => h.Index == 7);
            house.Hearth.SetLit(true);
            foreach (var ps in house.Hearth.Fire.GetComponentsInChildren<ParticleSystem>())
            {
                ps.Simulate(2f, true, true);
                Debug.Log($"[Hollowmere] hearth flames: {ps.particleCount} particles");
            }
            var interior = Object.FindAnyObjectByType<InteriorGrade>();

            var phases = new (string name, float clock)[]
            {
                ("day", MatchTimeline.DayStart(s) + s.DaySeconds * 0.3f),
                ("golden", MatchTimeline.DayStart(s) + s.DaySeconds * 0.9f),
                ("dusk", MatchTimeline.DuskStart(s) + s.DuskSeconds * 0.5f),
                ("night", MatchTimeline.NightStart(s) + 30f),
            };
            foreach (var (phase, clock) in phases)
            {
                var now = MatchTimeline.At(s, clock);
                lighting.Apply(now);
                if (now.Phase != MatchPhase.Day && now.Phase != MatchPhase.Dusk)
                    RenderSettings.fogDensity = lighting.NightFogDensity; // Apply eases night fog in over time; jump there
                foreach (var name in LookCameras)
                {
                    var go = GameObject.Find(name);
                    if (go == null) continue;
                    var cam = go.GetComponent<Camera>();
                    lighting.ApplyToCamera(cam);
                    if (interior != null) interior.Snap(cam.transform.position);
                    BatchTools.RenderToPng(cam, Path.Combine("Logs/claude", $"look_{phase}_{name}.png"));
                }
            }

            // Night, on the street outside the first row of houses, flashlight on.
            var player = Object.FindAnyObjectByType<PlayerAgent>();
            player.transform.SetPositionAndRotation(new Vector3(-6f, 0.1f, 74f), Quaternion.Euler(0f, 200f, 0f));
            player.Flashlight.Beam.enabled = true;
            player.Flashlight.Beam.range = player.Flashlight.Range;
            player.Flashlight.Beam.intensity = player.Flashlight.Intensity;
            var eye = player.Head.GetComponent<Camera>();
            lighting.ApplyToCamera(eye);
            if (interior != null) interior.Snap(eye.transform.position);
            BatchTools.RenderToPng(eye, "Logs/claude/look_night_PlayerFlashlight.png");
            Debug.Log("[Hollowmere] PHASE SHOTS OK: Logs/claude/look_*.png");
        }
    }
}
