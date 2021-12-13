using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using RimWorld;
using UnityEngine;
using System.Reflection;

namespace PowerView
{
    [StaticConstructorOnStartup]
    static class CustomPowerOverlay
    {
        public static bool enabled;

        public static readonly Graphic_CustomLinkedTransmitterOverlay LinkedOverlayGraphicInactive;
        public static readonly Graphic_CustomLinkedTransmitterOverlay LinkedOverlayGraphicPositive;
        public static readonly Graphic_CustomLinkedTransmitterOverlay LinkedOverlayGraphicNegative;

        private static readonly Color32[] qua_inactive = new Color32[4]
        {
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
        };

        private static readonly Color32[] qua_positive = new Color32[4]
        {
            new Color32(0,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(0,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(0,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(0,byte.MaxValue,byte.MaxValue,byte.MaxValue),
        };

        private static readonly Color32[] qua_negative = new Color32[4]
        {
            new Color32(byte.MaxValue,byte.MaxValue,0,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,0,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,0,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,0,byte.MaxValue),
        };

        static CustomPowerOverlay()
        {
            Graphic subGraphicInactive = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlayDesaturated);
            Graphic subGraphicPositive = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay);
            Graphic subGraphicNegative = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay);

            CustomPowerOverlay.LinkedOverlayGraphicInactive = new Graphic_CustomLinkedTransmitterOverlay(subGraphicInactive);
            CustomPowerOverlay.LinkedOverlayGraphicPositive = new Graphic_CustomLinkedTransmitterOverlay(subGraphicPositive);
            CustomPowerOverlay.LinkedOverlayGraphicNegative = new Graphic_CustomLinkedTransmitterOverlay(subGraphicNegative);

            subGraphicInactive.MatSingle.renderQueue = 3600;
            subGraphicPositive.MatSingle.renderQueue = 3600;
            subGraphicNegative.MatSingle.renderQueue = 3600;
        }

        private static void Print(PowerStatus status,SectionLayer layer, Thing thing)
        {
            switch (status)
            {
                case PowerStatus.inactive:
                    LinkedOverlayGraphicInactive.Print(layer, thing, qua_inactive);
                    break;
                case PowerStatus.positive:
                    LinkedOverlayGraphicPositive.Print(layer, thing, qua_positive);
                    break;
                case PowerStatus.negative:
                    LinkedOverlayGraphicNegative.Print(layer, thing, qua_negative);
                    break;
            }
        }
        static float lastUpdated = 0;
        public static void DoWhileOverlayOn()
        {
            OverlayDrawHandler.DrawPowerGridOverlayThisFrame();
            if(Time.realtimeSinceStartup - lastUpdated > 1)
            {
                Find.CurrentMap.powerNetManager.NotifyDrawersForWireUpdate(Find.CameraDriver.MapPosition);
                lastUpdated = Time.realtimeSinceStartup;
            }
        }

        public static void PrintPowerOverlayThing(SectionLayer layer, Thing parent)
        {
            PowerNet powerNet = ((Building)parent).PowerComp.PowerNet;

            float netPowerGain = powerNet.CurrentEnergyGainRate() / CompPower.WattsToWattDaysPerTick;
            int poweTraders = 0;
            foreach(CompPower comp in powerNet.connectors)
            {
                if (!comp.GetType().IsAssignableFrom(typeof(CompPowerTrader)) || comp.parent.IsBrokenDown()) continue;
                CompFlickable comp1 = comp.parent.TryGetComp<CompFlickable>();
                if (comp1 != null && !comp1.SwitchIsOn) continue;
                netPowerGain += ((CompPowerTrader)comp).PowerOutput;
                poweTraders++;
            }

            foreach (CompPower comp in powerNet.transmitters)
            {
                if (!comp.GetType().IsAssignableFrom(typeof(CompPowerPlant))) continue;
                netPowerGain += ((CompPowerPlant)comp).PowerOutput;
            }
            PowerStatus status = poweTraders > 0 ? (netPowerGain > 0 ? PowerStatus.positive : PowerStatus.negative) : PowerStatus.inactive;

            CustomPowerOverlay.Print(status, layer, parent);
        }

        enum PowerStatus : byte
        {
            inactive,
            positive,
            negative
        }
    }
}
