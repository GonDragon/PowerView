using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace PowerView
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    internal class DoPlaySettingsGlobalControlsPatch
    {
        public static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView)
            {
            }
            else
            {
                row.ToggleableIcon(ref CustomPowerOverlay.enabled, Textures.toggleIcon, "Powerview.Playsetting.Tooltip".Translate(), SoundDefOf.Mouseover_ButtonToggle);

                if (CustomPowerOverlay.enabled && Event.current.type == EventType.Repaint) OverlayDrawHandler.DrawPowerGridOverlayThisFrame();
            }
        }
    }
}
