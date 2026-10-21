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
    }
}
