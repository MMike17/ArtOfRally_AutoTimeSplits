using HarmonyLib;
using UnityEngine;

// Duplicate timer for best time (StageTimerManager / GetStageTimeMSInt)
// Duplicate timer for splits panel
// Add feature disabling

namespace AutoTimeSplits
{
    [HarmonyPatch(nameof(OutOfBoundsManager), nameof(OutOfBoundsManager.Start))]
    static class SplitsInitializer
    {
        static void Postfix(OutOfBoundsManager __instance)
        {
            Main.Try(() =>
            {
                int totalCount = __instance.GetWaypointList().Count;
                int step = Mathf.FloorToInt(totalCount / TimeSplits.splitsCount);
                int[] splitsIndexes = new int[TimeSplits.splitsCount];

                for (int i = 0; i < splitsIndexes.Length; i++)
                    splitsIndexes[i] = step * (i + 1);

                for (int i = 0; i < splitsIndexes.Length; i++)
                    Main.Log("Split index [" + i + "] : " + splitsIndexes[i]);

                TimeSplitsManager.Init(
                    GameModeManager.GetRallyDataCurrentGameMode().GetCurrentStage(),
                    GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.carClass,
                    splitsIndexes
                );

                Main.Log("Initialized splits system");
            });
        }
    }

    [HarmonyPatch(nameof(OutOfBoundsManager), "FixedUpdate")]
    static class SplitsUpdater
    {
        public static StageTimerManager timerManager;

        static void Postfix(OutOfBoundsManager __instance)
        {
            Main.Try(() =>
            {
                if (timerManager == null)
                    timerManager = StageTimerManager.FindObjectOfType<StageTimerManager>();
                else
                    TimeSplitsManager.Update(timerManager.GetStageTimeMSInt(), __instance.GetCurrentWaypointIndex());
            });
        }
    }
}
