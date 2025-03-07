﻿using UnityEngine;
using static Car;

// TODO : Should I save in file ?
// TODO : Export file ?

namespace AutoTimeSplits
{
    static class TimeSplitsManager
    {
        public static void SaveTimeSplits(TimeSplits timeSplits, int timeMilis, int index)
        {
            timeSplits.splits[index] = timeMilis;
            PlayerPrefs.SetString(timeSplits.GetSaveKey(), JsonUtility.ToJson(timeSplits));
        }

        public static TimeSplits GetTimeSplits(Stage stage, CarClass carClass)
        {
            TimeSplits splits = new TimeSplits(stage, carClass);

            if (PlayerPrefs.HasKey(splits.GetSaveKey()))
                splits = JsonUtility.FromJson<TimeSplits>(PlayerPrefs.GetString(splits.GetSaveKey()));

            return splits;
        }
    }
}