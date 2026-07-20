using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ArcaneEngine
{
    public sealed class Patch210AutomatedCapture21 : MonoBehaviour
    {
        private static Patch210AutomatedCapture21 _instance;
        private bool _running;

        public static bool Running { get { return _instance != null && _instance._running; } }

        public static void Begin()
        {
            if (_instance == null)
            {
                GameObject root = new GameObject("Patch 2.1 Automated Visual Capture");
                DontDestroyOnLoad(root);
                _instance = root.AddComponent<Patch210AutomatedCapture21>();
            }
            if (!_instance._running)
                _instance.StartCoroutine(_instance.CaptureRoutine());
        }

        private IEnumerator CaptureRoutine()
        {
            _running = true;
            Patch210ReferenceRuntime21 reference = UnityEngine.Object.FindAnyObjectByType<Patch210ReferenceRuntime21>();
            if (reference == null)
                reference = MorphologyPresentationDirector21.Instance.gameObject.GetComponent<Patch210ReferenceRuntime21>();
            if (reference != null) reference.BuildReferencePreview();
            yield return null;
            yield return null;

            string directory = Path.Combine(Application.dataPath, "../Patch210Captures");
            Directory.CreateDirectory(directory);
            string session = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string sessionDirectory = Path.Combine(directory, session);
            Directory.CreateDirectory(sessionDirectory);

            SpellPhase21[] phases =
            {
                SpellPhase21.Charge,
                SpellPhase21.Release,
                SpellPhase21.Travel,
                SpellPhase21.Contact,
                SpellPhase21.Resolve,
                SpellPhase21.Persist,
                SpellPhase21.Return,
                SpellPhase21.Expire
            };

            List<string> report = new List<string>();
            report.Add("Arcane Engine Patch 2.1 automated visual capture");
            report.Add("Session: " + session);
            report.Add("Unity: " + Application.unityVersion);
            report.Add("Quality: " + Patch200PresentationSettings.Quality);
            report.Add(string.Empty);

            for (int phaseIndex = 0; phaseIndex < phases.Length; phaseIndex++)
            {
                SpellPhase21 phase = phases[phaseIndex];
                GeneratedSpellMorphologyHost21[] hosts = UnityEngine.Object.FindObjectsByType<GeneratedSpellMorphologyHost21>();
                for (int i = 0; i < hosts.Length; i++)
                    if (hosts[i] != null) hosts[i].SetDebugPhase(phase, PhaseProgress(phase));

                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                string filename = phaseIndex.ToString("00") + "-" + phase + ".png";
                ScreenCapture.CaptureScreenshot(Path.Combine(sessionDirectory, filename), 1);
                report.Add(phase + ": hosts=" + hosts.Length +
                           " particles=" + PresentationParticlePool2.ActiveCount +
                           " geometry=" + PresentationGeometry2.ActiveCount +
                           " audio=" + ProceduralSpellAudio21.ActiveVoices);
                yield return new WaitForSecondsRealtime(0.18f);
            }

            GeneratedSpellMorphologyHost21[] finalHosts = UnityEngine.Object.FindObjectsByType<GeneratedSpellMorphologyHost21>();
            for (int i = 0; i < finalHosts.Length; i++)
                if (finalHosts[i] != null) finalHosts[i].ClearDebugPhase();

            SpellVisualContract21 contract = SpellMorphologyPresentation21.LastContract;
            if (contract != null)
            {
                report.Add(string.Empty);
                report.Add(contract.DebugSummary);
                report.Add("Warnings: " + contract.validationWarnings.Count);
                for (int i = 0; i < contract.validationWarnings.Count; i++)
                    report.Add("- " + contract.validationWarnings[i]);
            }

            File.WriteAllLines(Path.Combine(sessionDirectory, "capture-report.txt"), report.ToArray());
            Debug.Log("Patch 2.1 automated capture complete: " + sessionDirectory);
            _running = false;
        }

        private static float PhaseProgress(SpellPhase21 phase)
        {
            switch (phase)
            {
                case SpellPhase21.Charge: return 0.16f;
                case SpellPhase21.Release: return 0.27f;
                case SpellPhase21.Travel: return 0.52f;
                case SpellPhase21.Contact: return 0.72f;
                case SpellPhase21.Resolve: return 0.80f;
                case SpellPhase21.Persist: return 0.68f;
                case SpellPhase21.Return: return 0.78f;
                case SpellPhase21.Expire: return 0.96f;
                default: return 0.5f;
            }
        }
    }
}
