using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace AutoTimeSplits
{
    public class Settings : ModSettings, IDrawable
    {
        readonly static Color Brown = new Color(0.75f, 0.5f, 0.25f);

        public enum ColorTag
        {
            White,
            Grey,
            Black,
            Red,
            Green,
            Blue,
            Yellow,
            Magenta,
            Cyan,
            Brown
        }

        [Draw(DrawType.Auto)]
        public ColorTag goodSplitColor = ColorTag.Green;
        [Draw(DrawType.Auto)]
        public ColorTag badSplitColor = ColorTag.Red;
        [Space]
        [Draw(DrawType.Slider, Min = -100, Max = -50)]
        public int bestTimePanelHeight = -60;
        [Draw(DrawType.Slider, Min = 100, Max = 500)]
        public int timeSplitsPanelHeight = 250;
        [Draw(DrawType.Slider, Min = 1, Max = 3)]
        public float timeSplitsPanelSize = 1.5f;

        [Header("Debug")]
        [Draw(DrawType.Toggle)]
        public bool disableInfoLogs = true;
        [Draw(DrawType.Toggle, Label = "Reset time splits")]
        public bool resetTimeSplits;

        public override void Save(ModEntry modEntry) => Save(this, modEntry);

        public void OnChange()
        {
            if (resetTimeSplits)
            {
                TimeSplitsManager.ResetTimeSplits();
                resetTimeSplits = false;
            }

            TimeSplitsUI.SetBestHeight(bestTimePanelHeight);
            TimeSplitsUI.SetSplitsHeight(timeSplitsPanelHeight);
            TimeSplitsUI.SetSplitsSize(timeSplitsPanelSize);
        }

        public static Color GetColor(ColorTag tag)
        {
            switch (tag)
            {
                case ColorTag.White:
                    return Color.white;
                case ColorTag.Grey:
                    return Color.grey;
                case ColorTag.Black:
                    return Color.black;
                case ColorTag.Red:
                    return Color.red;
                case ColorTag.Green:
                    return Color.green;
                case ColorTag.Blue:
                    return Color.blue;
                case ColorTag.Yellow:
                    return Color.yellow;
                case ColorTag.Magenta:
                    return Color.magenta;
                case ColorTag.Cyan:
                    return Color.cyan;
                case ColorTag.Brown:
                    return Brown;
                default:
                    return Color.clear;
            }
        }
    }
}
