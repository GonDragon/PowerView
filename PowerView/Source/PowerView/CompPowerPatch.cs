using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using Verse;
using RimWorld;

namespace PowerView
{
    [HarmonyPatch(typeof(CompPower), "CompPrintForPowerGrid")]
    internal class CompPowerPatch
    {
        static internal bool Prefix(SectionLayer layer, CompPower __instance)
        {
            if (__instance.TransmitsPowerNow)
                CustomPowerOverlay.PrintPowerOverlayThing(layer, (Thing)__instance.parent);
            if (__instance.parent.def.ConnectToPower)
                PowerNetGraphics.PrintOverlayConnectorBaseFor(layer, (Thing)__instance.parent);
            if (__instance.connectParent == null)
                return false;
            PowerNetGraphics.PrintWirePieceConnecting(layer, (Thing)__instance.parent, (Thing)__instance.connectParent.parent, true);
            return false;
        }
    }
}
