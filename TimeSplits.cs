using UnityEngine;

using static Car;
using static ConditionTypes;

namespace AutoTimeSplits
{
    public class TimeSplits
    {
        public const int splitsCount = 4;

        public CarClass carClass;
        public string stageName;
        public Weather stageWeather;
        public int[] splits;

        public bool needsDoubleCheck => stageWeather < Weather.Night && stageWeather > Weather.None;

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

            if (targetSplit < 0)
            {
                TimeSplitsManager.SaveTimeSplits(this, timeMilisecs, index);
                return null;
            }

            return FormatSplit(timeMilisecs - targetSplit);
        }

        public string FormatSplit(int timeMilisecs)
        {
            bool isNegative = timeMilisecs < 0;
            timeMilisecs = Mathf.Abs(timeMilisecs);

            int minutes = Mathf.RoundToInt(timeMilisecs / 60000);
            int seconds = Mathf.RoundToInt(timeMilisecs / 1000 - minutes * 60);
            int fractions = Mathf.RoundToInt((timeMilisecs - (minutes * 60 + seconds) * 1000) / 10);
            string trailingZeroes = string.Empty;

            while (trailingZeroes.Length < 3 - fractions.ToString().Length)
                trailingZeroes += "0";

            return (isNegative ? "-" : "+") +
                (minutes < 10 ? "0" : "") + minutes + ":" +
                (seconds < 10 ? "0" : "") + seconds + "." +
                trailingZeroes + fractions;
        }

        // used for most cases
        public string GetSaveKey() => carClass + "_" + stageName + "_" + stageWeather;

        // used for fixing
        public string GetCorrectedSaveKey() => carClass + "_" + stageName + "_" + (needsDoubleCheck ? Weather.None : stageWeather);
    }
}
