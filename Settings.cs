using UnityEngine;
using UnityModManagerNet;

using static UnityModManagerNet.UnityModManager;

namespace AutoTimeSplits
{
    public class Settings : ModSettings, IDrawable
    {
        //[Draw(DrawType.)]

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
        }
    }
}
