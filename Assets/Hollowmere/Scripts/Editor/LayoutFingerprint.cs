using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace Hollowmere.EditorTools
{
    /// <summary>
    /// A text snapshot of everything in the Greybox scene that gameplay touches: every collider (shape, trigger,
    /// enabled), every NavMesh obstacle, and the transform of every Hollowmere component. Art passes (M1.5, M7) must
    /// leave it unchanged; GreyboxSceneTests compares the scene against the snapshot recorded before the look pass.
    /// Renderers, meshes, materials and lights are deliberately left out.
    /// </summary>
    public static class LayoutFingerprint
    {
        public const string FixturePath = "Assets/Hollowmere/Tests/EditMode/GreyboxLayout.txt";

        /// <summary>Writes the open scene's fingerprint as the reference. Only for a deliberate layout change.</summary>
        public static void Record()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(GreyboxVillageBuilder.ScenePath, OpenSceneMode.Single);
            var lines = Compute();
            File.WriteAllText(FixturePath, string.Join("\n", lines) + "\n");
            AssetDatabase.ImportAsset(FixturePath);
            Debug.Log($"[Hollowmere] LAYOUT RECORDED: {lines.Count} lines -> {FixturePath}");
        }

        /// <summary>Sorted lines describing the gameplay layout of the open scene.</summary>
        public static List<string> Compute()
        {
            var lines = new List<string>();
            foreach (var root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            foreach (var c in root.GetComponentsInChildren<Component>(true))
            {
                switch (c)
                {
                    case BoxCollider b:
                        lines.Add($"{Pose(b)}|Box|{V(b.center)}|{V(b.size)}|{Flags(b)}");
                        break;
                    case SphereCollider s:
                        lines.Add($"{Pose(s)}|Sphere|{V(s.center)}|{F(s.radius)}|{Flags(s)}");
                        break;
                    case CapsuleCollider k:
                        lines.Add($"{Pose(k)}|Capsule|{V(k.center)}|{F(k.radius)}|{F(k.height)}|{k.direction}|{Flags(k)}");
                        break;
                    case CharacterController cc:
                        lines.Add($"{Pose(cc)}|CharacterController|{V(cc.center)}|{F(cc.radius)}|{F(cc.height)}|{Flags(cc)}");
                        break;
                    case MeshCollider m:
                        lines.Add($"{Pose(m)}|Mesh|{(m.sharedMesh ? m.sharedMesh.name : "none")}|{m.convex}|{Flags(m)}");
                        break;
                    case NavMeshObstacle o:
                        lines.Add($"{Pose(o)}|Obstacle|{o.shape}|{V(o.center)}|{V(o.size)}|{o.carving}|{o.enabled}|{o.gameObject.activeSelf}");
                        break;
                    case MonoBehaviour mb when mb.GetType().Namespace == "Hollowmere":
                        lines.Add($"{Pose(mb)}|{mb.GetType().Name}|{mb.enabled}|{mb.gameObject.activeSelf}");
                        break;
                }
            }
            lines.Sort(System.StringComparer.Ordinal);
            return lines;
        }

        static string Flags(Collider c) => $"{c.isTrigger}|{c.enabled}|{c.gameObject.activeSelf}";

        static string Pose(Component c)
        {
            var t = c.transform;
            var q = t.rotation;
            if (q.w < 0f || (q.w == 0f && q.y < 0f)) q = new Quaternion(-q.x, -q.y, -q.z, -q.w);
            return $"{Path(t)}|{V(t.position)}|{Q(q)}|{V(t.lossyScale)}";
        }

        static string Path(Transform t)
        {
            var names = new List<string>();
            for (; t != null; t = t.parent) names.Add(t.name);
            names.Reverse();
            return string.Join("/", names);
        }

        static string F(float v) => (Mathf.Round(v * 1000f) / 1000f + 0f).ToString("0.000", CultureInfo.InvariantCulture);
        static string V(Vector3 v) => $"{F(v.x)},{F(v.y)},{F(v.z)}";
        static string Q(Quaternion q)
        {
            string R(float v) => (Mathf.Round(v * 10000f) / 10000f + 0f).ToString("0.0000", CultureInfo.InvariantCulture);
            return $"{R(q.x)},{R(q.y)},{R(q.z)},{R(q.w)}";
        }
    }
}
