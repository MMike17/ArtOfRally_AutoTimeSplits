using System;
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

        public TimeSplits(string file)
        {
            string[] frags = file.Split('\n');

            foreach (string frag in frags)
            {
                string key = frag.Split(':')[0].Trim();
                string value = frag.Split(':')[1].Trim();

                switch (key)
                {
                    case nameof(carClass):
                        carClass = (CarClass)Enum.Parse(typeof(CarClass), value);
                        break;

                    case nameof(stageName):
                        stageName = value;
                        break;

                    case nameof(stageWeather):
                        stageWeather = (Weather)Enum.Parse(typeof(Weather), value);
                        break;

                    case nameof(splits):
                        splits = new int[splitsCount];
                        string[] values = value.Split('[')[1].Split(']')[0].Split(',');

                        for (int i = 0; i < values.Length; i++)
                            splits[i] = int.Parse(values[i]);
                        break;
                }
            }
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

        public string FormatFile()
        {
            string contents = nameof(carClass) + " : " + carClass + "\n" +
                nameof(stageName) + " : " + stageName + "\n" +
                nameof(stageWeather) + " : " + stageWeather + "\n" +
                nameof(splits) + " : [";

            foreach (int split in splits)
                contents += split + ",";

            return contents.Split(']')[0];
        }
    }
}
