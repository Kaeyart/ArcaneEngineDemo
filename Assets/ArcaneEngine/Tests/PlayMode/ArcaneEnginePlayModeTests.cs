using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ArcaneEngine.Tests
{
    public sealed class ArcaneEnginePlayModeTests
    {
        [UnityTest]
        public IEnumerator BootstrapCreatesOneAuthoritativeMainCamera()
        {
            yield return null;
            Assert.That(GameWorld.Instance, Is.Not.Null);
            Camera[] main = Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)
                .Where(value => value != null && value.CompareTag("MainCamera")).ToArray();
            Assert.That(main.Length, Is.EqualTo(1));
            Assert.That(Camera.main, Is.SameAs(main[0]));
        }

        [UnityTest]
        public IEnumerator ProductPanelsOpenAndCloseWithoutLeakingPause()
        {
            yield return null;
            Assert.That(V21ProductUI.Instance, Is.Not.Null);
            V21ProductUI.Instance.OpenWorkshop();
            yield return null;
            Assert.That(V21ProductUI.Instance.IsOpen, Is.True);
            V21ProductUI.Instance.Close();
            yield return null;
            Assert.That(V21ProductUI.Instance.IsOpen, Is.False);
            Assert.That(Time.timeScale, Is.EqualTo(1f));
        }

        [UnityTest]
        public IEnumerator NewRunCreatesPlayerAndDeterministicRoomState()
        {
            yield return null;
            RunDirector run = GameWorld.Instance.GetComponent<RunDirector>();
            Assert.That(run, Is.Not.Null);
            run.BeginSeededRun(210021);
            yield return null;
            Assert.That(GameWorld.Instance.RunActive, Is.True);
            Assert.That(GameWorld.Instance.Player, Is.Not.Null);
            Assert.That(run.CurrentSeed, Is.EqualTo(210021));
            Assert.That(GameWorld.Instance.CurrentRoom, Is.Not.Null);
        }
    }
}
