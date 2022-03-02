using Verse;
using RimWorld;
using UnityEngine;

namespace PowerView
{
    internal class DoPlaySettingsGlobalControlsPatch
    {
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
            {
            }
            else
            {
                row.ToggleableIcon(ref CustomPowerOverlay.enabled, CustomPowerOverlay.toggleIcon, "Powerview.Playsetting.Tooltip".Translate(), SoundDefOf.Mouseover_ButtonToggle);

                if (CustomPowerOverlay.enabled && Event.current.type == EventType.Repaint) CustomPowerOverlay.DoWhileOverlayOn();
            }
        }
    }
}
