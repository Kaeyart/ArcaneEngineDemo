using System;
using UnityEngine;

namespace ArcaneEngine
{
    public static class GameBootstrap
    {
        private const string RootName = "Arcane Engine Demo";

        // This list is intentionally ordered. AddComponent invokes Awake immediately.
        private static readonly Type[] RuntimeComponents =
        {
            typeof(ArcaneInputPump),
            typeof(GameWorld),
            typeof(RunDirector),
            typeof(DemoUI),
            typeof(V21ProductUI),
            typeof(ModernCombatHUD),
            typeof(WorldInteractionController),
            typeof(RunStartScreen),
            typeof(RunStatistics),
            typeof(V21TrainingAnalytics),
            typeof(DemoV05Director),
            typeof(V21AudioDirector),
            typeof(AdaptiveAudioSystem),
            typeof(HomeBaseController),
            typeof(TutorialDirector),
            typeof(AttackCoordinator),
            typeof(V1PerformanceBudget),
            typeof(V1Diagnostics),
            typeof(VisualDiagnosticsOverlay),
            typeof(VisualStressScenarioRunner),
            typeof(VisualComparisonMatrixRunner),
            typeof(V12SystemsDirector),
            typeof(V1TitleScreen),
            typeof(V1GameDirector),
            typeof(NarrativeDirector)
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            // Required when Enter Play Mode Options disables domain reload.
            ServiceLocator.Reset();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            if (UnityEngine.Object.FindAnyObjectByType<GameWorld>() != null)
                return;

            GameObject root = new GameObject(RootName);
            UnityEngine.Object.DontDestroyOnLoad(root);

            try
            {
                foreach (Type componentType in RuntimeComponents)
                {
                    if (root.GetComponent(componentType) == null)
                        root.AddComponent(componentType);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                UnityEngine.Object.Destroy(root);
                throw;
            }
        }
    }
}
