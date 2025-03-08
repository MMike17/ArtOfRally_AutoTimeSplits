using System;
using UnityEngine;

using static Car;

// TODO : Should I save in file ?
// TODO : Export file ?

namespace AutoTimeSplits
{
    static class TimeSplitsManager
    {
        private static int nextSplitIndex;
        private static int[] splitIndexes;
        private static TimeSplits timeSplits;

        public static void Init(Stage stage, CarClass carClass, int[] splitIndexes)
        {
            TimeSplitsManager.splitIndexes = splitIndexes;
            timeSplits = GetTimeSplits(stage, carClass);
        }

        public static void Update(int timeMilis, int waypointIndex)
        {
            if (nextSplitIndex < splitIndexes.Length && waypointIndex >= splitIndexes[nextSplitIndex])
            {
                string splitDisplay = timeSplits.GetSplit(timeMilis, nextSplitIndex);

                if (splitDisplay != null)
                    TimeSplitsUI.ShowSplits(splitDisplay);

                Main.Log(splitIndexes[nextSplitIndex] + " : " + splitDisplay);
                nextSplitIndex++;
            }
        }

        private static TimeSplits GetTimeSplits(Stage stage, CarClass carClass)
        {
            TimeSplits splits = new TimeSplits(stage, carClass);

            if (PlayerPrefs.HasKey(splits.GetSaveKey()))
                splits = JsonUtility.FromJson<TimeSplits>(PlayerPrefs.GetString(splits.GetSaveKey()));

            Main.Log("Retrieved splits for " + stage.Name + " (" + stage.Area + ") " + carClass);
            return splits;
        }

        public static void SaveTimeSplits(TimeSplits timeSplits, int timeMilis, int index)
        {
            timeSplits.splits[index] = timeMilis;
            PlayerPrefs.SetString(timeSplits.GetSaveKey(), JsonUtility.ToJson(timeSplits));
            Main.Log("Saved time splits for key " + timeSplits.GetSaveKey());
        }

        public static void CheckFinishingTime(int timeMilis)
        {
            int targetSplit = timeSplits.splits[timeSplits.splits.Length - 1];

            if (targetSplit < 0 || timeMilis < targetSplit)
            {
                timeSplits.splits[timeSplits.splits.Length - 1] = timeMilis;
                SaveTimeSplits(timeSplits, timeMilis, timeSplits.splits.Length - 1);
            }
        }

        public static string GetBestTime()
        {
            int split = timeSplits.splits[timeSplits.splits.Length - 1];

            if (split < 0)
                return null;
            else
                return timeSplits.FormatSplit(split).Replace("+", "");
        }

        public static void ResetTimeSplits()
        {
            foreach (Stage stage in GameModeManager.GetRallyDataCurrentGameMode().StageList)
            {
                foreach (CarClass carClass in Enum.GetValues(typeof(CarClass)))
                {
                    TimeSplits timeSplits = new TimeSplits(stage, carClass);
                    string key = timeSplits.GetSaveKey();

                    if (PlayerPrefs.HasKey(key))
                        PlayerPrefs.DeleteKey(key);
                }
            }

            Main.Log("Time splits have been reset");
        }
    }
}