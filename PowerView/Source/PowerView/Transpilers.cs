using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using HarmonyLib;
using System.Reflection.Emit;
using System.Reflection;

namespace PowerView
{
    [HarmonyPatch(typeof(MainTabWindow_Architect), "WindowUpdate")]
    internal class MainTabWindow_Architect_Transpiler
    {
        private static MethodInfo DoWhileOverlayOn = AccessTools.Method(typeof(CustomPowerOverlay), nameof(CustomPowerOverlay.DoWhileOverlayOn));

        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool patched = false;
            int num = 0;

            foreach(CodeInstruction instruction in instructions)
            {
                if(!patched && instruction.opcode == OpCodes.Call)
                {
                    num++;
                    if(num == 3)
                    {
                        instruction.operand = DoWhileOverlayOn;
                        patched = true;
                    }

                }
                yield return instruction;
            }
        }
    }

    [HarmonyPatch(typeof(BuildDesignatorUtility), "TryDrawPowerGridAndAnticipatedConnection")]
    internal class BuildDesignatorUtility_Transpiler
    {
        private static MethodInfo DoWhileOverlayOn = AccessTools.Method(typeof(CustomPowerOverlay), nameof(CustomPowerOverlay.DoWhileOverlayOn));

        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instructions)
        {
            bool patched = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!patched && instruction.opcode == OpCodes.Call)
                {
                    instruction.operand = DoWhileOverlayOn;
                    patched = true;
                }
                yield return instruction;
            }
        }
    }
}
