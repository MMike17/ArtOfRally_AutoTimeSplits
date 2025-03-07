﻿using System;
using UnityEngine;

using static Car;
using static ConditionTypes;

namespace AutoTimeSplits
{
    public class TimeSplits
    {
        const int splitsCount = 4;

        public CarClass carClass;
        public string stageName;
        public Weather stageWeather;
        public int[] splits;

        // use GameModeManager.GetRallyDataCurrentGameMode().GetCurrentStage() to get current Stage
        // use GameEntryPoint.EventManager.playerManager to get current car class
        public TimeSplits(Stage stage, CarClass carClass)
        {
            this.carClass = carClass;
            stageName = stage.Name;
            stageWeather = stage.Weather;

            splits = new int[splitsCount];

            for (int i = 0; i < splits.Length; i++)
                splits[i] = -1;
        }

        public string GetSplit(int timeMilisecs, int index)
        {
            int targetSplit = splits[index];

            if (targetSplit < 0 || timeMilisecs < targetSplit)
                TimeSplitsManager.SaveTimeSplits(this, timeMilisecs, index);

            int diff = Mathf.Abs(timeMilisecs - targetSplit);
            int minutes = Mathf.RoundToInt(diff / 60000);
            int seconds = Mathf.RoundToInt(diff / 1000 - minutes * 60);
            int fractions = Mathf.RoundToInt((diff - (minutes * 60 + seconds) * 1000) / 10);

            return (diff > 0 ? "+" : "-") +
                (minutes < 10 ? "0" : "") + minutes + ":" +
                (seconds < 10 ? "0" : "") + seconds + ":" +
                (fractions < 10 ? "0" : "") + fractions;
        }

        public string GetSaveKey() => carClass + "_" + stageName + "_" + stageWeather;
    }
}
