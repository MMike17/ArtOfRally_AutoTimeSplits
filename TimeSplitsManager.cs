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
            if (waypointIndex >= splitIndexes[nextSplitIndex])
            {
                string splitDisplay = timeSplits.GetSplit(timeMilis, nextSplitIndex);

                if (splitDisplay != null)
                {
                    // TODO : Show splits UI here
                }

                Main.Log(splitIndexes[nextSplitIndex] + " : " + splitDisplay);
                nextSplitIndex++;
            }
        }

        public static void SaveTimeSplits(TimeSplits timeSplits, int timeMilis, int index)
        {
            timeSplits.splits[index] = timeMilis;
            PlayerPrefs.SetString(timeSplits.GetSaveKey(), JsonUtility.ToJson(timeSplits));
        }

        private static TimeSplits GetTimeSplits(Stage stage, CarClass carClass)
        {
            TimeSplits splits = new TimeSplits(stage, carClass);

            if (PlayerPrefs.HasKey(splits.GetSaveKey()))
                splits = JsonUtility.FromJson<TimeSplits>(PlayerPrefs.GetString(splits.GetSaveKey()));

            Main.Log("Retrieved splits for " + stage.Name + " (" + stage.Area + ") " + carClass + " [" + splits.splits + "]");
            return splits;
        }
    }
}