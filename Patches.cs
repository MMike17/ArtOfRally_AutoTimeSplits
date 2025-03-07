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
        static void Postfix(OutOfBoundsManager _instance)
        {
            int totalCount = _instance.GetWaypointList().Count;
            int step = Mathf.FloorToInt(totalCount / TimeSplits.splitsCount);
            int[] splitsIndexes = new int[TimeSplits.splitsCount];

            for (int i = 0; i < splitsIndexes.Length; i++)
                splitsIndexes[i] = step * (i + 1);

            SplitsUpdater.timerManager = StageTimerManager.FindObjectOfType<StageTimerManager>();

            TimeSplitsManager.Init(
                GameModeManager.GetRallyDataCurrentGameMode().GetCurrentStage(),
                GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.carClass,
                splitsIndexes
            );

            Main.Log("Initialized splits system");
        }
    }

    [HarmonyPatch(nameof(OutOfBoundsManager), "FixedUpdate")]
    static class SplitsUpdater
    {
        public static StageTimerManager timerManager;

        static void Postfix(int __CurrentWaypointIndex)
        {
            TimeSplitsManager.Update(timerManager.GetStageTimeMSInt(), __CurrentWaypointIndex);
        }
    }
}
