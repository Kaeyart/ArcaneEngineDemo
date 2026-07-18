using UnityEngine;

namespace ArcaneEngine
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            if (Object.FindAnyObjectByType<GameWorld>() != null) return;
            GameObject root = new GameObject("Arcane Engine Demo");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<ArcaneInputPump>();
            root.AddComponent<GameWorld>();
            root.AddComponent<RunDirector>();
            root.AddComponent<DemoUI>();
            root.AddComponent<V21ProductUI>();
            root.AddComponent<ModernCombatHUD>();
            root.AddComponent<WorldInteractionController>();
            root.AddComponent<RunStartScreen>();
            root.AddComponent<RunStatistics>();
            root.AddComponent<V21TrainingAnalytics>();
            root.AddComponent<DemoV05Director>();
            root.AddComponent<V21AudioDirector>();
            root.AddComponent<AdaptiveAudioSystem>();
            root.AddComponent<HomeBaseController>();
            root.AddComponent<TutorialDirector>();
            root.AddComponent<AttackCoordinator>();
            root.AddComponent<V1PerformanceBudget>();
            root.AddComponent<V1Diagnostics>();
            root.AddComponent<VisualDiagnosticsOverlay>();
            root.AddComponent<VisualStressScenarioRunner>();
            root.AddComponent<VisualComparisonMatrixRunner>();
            root.AddComponent<V12SystemsDirector>();
            root.AddComponent<V1TitleScreen>();
            root.AddComponent<V1GameDirector>();
            root.AddComponent<NarrativeDirector>();
        }
    }
}
