using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace AutoTimeSplits
{
    [HarmonyPatch(typeof(OutOfBoundsManager), nameof(OutOfBoundsManager.Start))]
    static class SplitsInitializer
    {
        static void Postfix(OutOfBoundsManager __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() =>
            {
                int totalCount = __instance.GetWaypointList().Count;
                int step = Mathf.FloorToInt(totalCount / TimeSplits.splitsCount);
                int[] splitsIndexes = new int[TimeSplits.splitsCount - 1];

                for (int i = 0; i < splitsIndexes.Length; i++)
                    splitsIndexes[i] = step * (i + 1);

                TimeSplitsManager.Init(splitsIndexes);
                TimeSplitsManager.Prime(
                    GameModeManager.GetRallyDataCurrentGameMode().GetCurrentStage(),
                    GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.carClass
                );

                Main.Log("Initialized splits system (splits : " +
                    splitsIndexes[0] + ", " +
                    splitsIndexes[1] + ", " +
                    splitsIndexes[2] + " / " +
                    totalCount + ")"
                );
            });
        }
    }

    [HarmonyPatch(typeof(OutOfBoundsManager), "FixedUpdate")]
    static class SplitsUpdater
    {
        public static StageTimerManager timerManager;

        static void Postfix(OutOfBoundsManager __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() =>
            {
                if (timerManager == null)
                {
                    timerManager = StageTimerManager.FindObjectOfType<StageTimerManager>();

                    if (timerManager != null)
                        TimeSplitsUI.Init(timerManager);
                }
                else
                    TimeSplitsManager.Update(timerManager.GetStageTimeMSInt(), __instance.GetCurrentWaypointIndex());
            });
        }
    }

    [HarmonyPatch(typeof(EventManager), nameof(EventManager.StartEvent))]
    public class SplitsReseter
    {
        static void Postfix()
        {
            if (!Main.enabled)
                return;

            TimeSplitsManager.Prime(
                GameModeManager.GetRallyDataCurrentGameMode().GetCurrentStage(),
                GameModeManager.GetSeasonDataCurrentGameMode().SelectedCar.carClass
            );
        }
    }

    [HarmonyPatch(typeof(StageTimerManager), nameof(StageTimerManager.OnStageOver))]
    static class SplitsCloser
    {
        static void Postfix(StageTimerManager __instance)
        {
            if (!Main.enabled)
                return;

            Main.Try(() =>
            {
                TimeSplitsManager.CheckFinishingTime(__instance.GetStageTimeMSInt());
                Main.Log("Finish time : " + Main.GetField<Text, StageTimerManager>(__instance, "TimeDisplay", System.Reflection.BindingFlags.Instance).text);
            });
        }
    }
}
