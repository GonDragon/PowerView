using Verse;
using RimWorld;
using UnityEngine;
using Verse.Sound;

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
                CheckKeyBindingToggle(KeybindingDefOf.ToggleElectricGrid, ref CustomPowerOverlay.enabled);
                if (CustomPowerOverlay.enabled && Event.current.type == EventType.Repaint) CustomPowerOverlay.DoWhileOverlayOn();
            }
        }

        private static void CheckKeyBindingToggle(KeyBindingDef keyBinding, ref bool value)
        {
            if (!keyBinding.KeyDownEvent)
                return;
            value = !value;
            if (value)
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
            else
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
        }
    }
}
