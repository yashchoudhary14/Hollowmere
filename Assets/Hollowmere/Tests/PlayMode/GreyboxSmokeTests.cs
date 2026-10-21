using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Hollowmere.Tests
{
    /// <summary>
    /// Plays the generated Greybox scene for a few seconds. Any exception or error log fails the test,
    /// which catches broken wiring that EditMode tests can't see.
    /// </summary>
    public class GreyboxSmokeTests
    {
        [UnityTest]
        public IEnumerator Greybox_PlaysForAFewSeconds_WithoutErrors()
        {
            SceneManager.LoadScene("Greybox");
            yield return null;
            yield return null;

            var player = Object.FindAnyObjectByType<PlayerAgent>();
            Assert.That(player, Is.Not.Null, "No PlayerAgent in the scene.");
            Assert.That(Object.FindAnyObjectByType<Hud>(), Is.Not.Null, "No HUD in the scene.");

            float until = Time.time + 3f;
            while (Time.time < until) yield return null;

            Assert.That(player.IsAlive, Is.True);
        }
    }
}
