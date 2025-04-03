using System;
using UnityEngine;

using static Car;

namespace AutoTimeSplits
{
    static class TimeSplitsManager
    {
        private static int nextSplitIndex;
        private static int[] splitIndexes;
        private static TimeSplits timeSplits;

        public static void Init(int[] splitIndexes) => TimeSplitsManager.splitIndexes = splitIndexes;

        public static void Prime(Stage stage, CarClass carClass)
        {
            timeSplits = GetTimeSplits(stage, carClass);
            nextSplitIndex = 0;

            TimeSplitsUI.UpdateBest();
        }

        public static void Update(int timeMilis, int waypointIndex)
        {
            if (nextSplitIndex < splitIndexes.Length && waypointIndex >= splitIndexes[nextSplitIndex])
            {
                string splitDisplay = timeSplits.GetSplit(timeMilis, nextSplitIndex);
                timeSplits.splits[nextSplitIndex] = timeMilis;

                if (splitDisplay != null)
                    TimeSplitsUI.ShowSplits(splitDisplay);

                Main.Log("Waypoint " + splitIndexes[nextSplitIndex] + " : " + splitDisplay);
                nextSplitIndex++;
            }
        }

        private static TimeSplits GetTimeSplits(Stage stage, CarClass carClass)
        {
            TimeSplits splits = new TimeSplits(stage, carClass);
            string key = splits.GetCorrectedSaveKey();

            if (PlayerPrefs.HasKey(splits.GetSaveKey()))
            {
                key = splits.GetSaveKey();
                splits = JsonUtility.FromJson<TimeSplits>(PlayerPrefs.GetString(key));

                if (splits.needsDoubleCheck)
                {
                    Main.Log("Detected time split fixing.");
                    Main.Log("Previous key : " + splits.GetSaveKey() + " / New key : " + splits.GetCorrectedSaveKey());

                    SaveTimeSplits(splits, splits.splits[0], 0);
                    PlayerPrefs.DeleteKey(splits.GetSaveKey());
                }
            }
            else if (splits.needsDoubleCheck && PlayerPrefs.HasKey(splits.GetCorrectedSaveKey()))
                splits = JsonUtility.FromJson<TimeSplits>(PlayerPrefs.GetString(splits.GetCorrectedSaveKey()));

            Main.Log("Retrieved splits for " + stage.Name + " (" + key + ")");
            return splits;
        }

        public static void SaveTimeSplits(TimeSplits timeSplits, int timeMilis, int index)
        {
            timeSplits.splits[index] = timeMilis;
            PlayerPrefs.SetString(timeSplits.GetCorrectedSaveKey(), JsonUtility.ToJson(timeSplits));
            Main.Log("Saved time splits for key : " + timeSplits.GetCorrectedSaveKey());
        }

        public static void CheckFinishingTime(int timeMilis)
        {
            int targetSplit = timeSplits.splits[timeSplits.splits.Length - 1];

            if (targetSplit <= 0 || timeMilis < targetSplit)
                SaveTimeSplits(timeSplits, timeMilis, timeSplits.splits.Length - 1);
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
                    DeleteKey(timeSplits.GetSaveKey());
                    DeleteKey(timeSplits.GetCorrectedSaveKey());
                }
            }

            void DeleteKey(string key)
            {
                if (PlayerPrefs.HasKey(key))
                    PlayerPrefs.DeleteKey(key);
            }

            Main.Log("Time splits have been reset");
        }
    }
}