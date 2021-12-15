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
        public static readonly Texture2D toggleIcon = ContentFinder<Texture2D>.Get("UI/Playsettings/gd-powerview");

        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicInactive;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicPositive;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicOnBattery;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicNegative;

        private static readonly ColorInt inactive = new ColorInt(200, 200, 200, 100);
        private static readonly ColorInt positive = new ColorInt(210, 230, 255, 190);
        private static readonly ColorInt onBattery = new ColorInt(225, 225, 0, 190);
        private static readonly ColorInt negative = new ColorInt(250, 90, 90, 190);

        static CustomPowerOverlay()
        {
            Graphic subGraphicInactive = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay, Vector2.one, inactive.ToColor);
            Graphic subGraphicPositive = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay, Vector2.one, positive.ToColor);
            Graphic subGraphicOnBattery = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay, Vector2.one, onBattery.ToColor);
            Graphic subGraphicNegative = GraphicDatabase.Get<Graphic_Single>("Things/Special/Power/TransmitterAtlas", ShaderDatabase.MetaOverlay, Vector2.one, negative.ToColor);

            CustomPowerOverlay.LinkedOverlayGraphicInactive = new Graphic_LinkedTransmitterOverlay(subGraphicInactive);
            CustomPowerOverlay.LinkedOverlayGraphicPositive = new Graphic_LinkedTransmitterOverlay(subGraphicPositive);
            CustomPowerOverlay.LinkedOverlayGraphicOnBattery = new Graphic_LinkedTransmitterOverlay(subGraphicOnBattery);
            CustomPowerOverlay.LinkedOverlayGraphicNegative = new Graphic_LinkedTransmitterOverlay(subGraphicNegative);

            subGraphicInactive.MatSingle.renderQueue = 3600;
            subGraphicPositive.MatSingle.renderQueue = 3600;
            subGraphicOnBattery.MatSingle.renderQueue = 3600;
            subGraphicNegative.MatSingle.renderQueue = 3600;
        }

        private static void Print(PowerStatus status,SectionLayer layer, Thing thing)
        {
            switch (status)
            {
                case PowerStatus.inactive:
                    LinkedOverlayGraphicInactive.Print(layer, thing, 0);
                    break;
                case PowerStatus.positive:
                    LinkedOverlayGraphicPositive.Print(layer, thing, 0);
                    break;
                case PowerStatus.onBattery:
                    LinkedOverlayGraphicOnBattery.Print(layer, thing, 0);
                    break;
                case PowerStatus.negative:
                    LinkedOverlayGraphicNegative.Print(layer, thing, 0);
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

            PowerStatus status = poweTraders > 0 ? (netPowerGain > 0 ? PowerStatus.positive : (powerNet.CurrentStoredEnergy() > 0 ? PowerStatus.onBattery :PowerStatus.negative)) : PowerStatus.inactive;

            CustomPowerOverlay.Print(status, layer, parent);
        }

        enum PowerStatus : byte
        {
            inactive,
            positive,
            onBattery,
            negative
        }
    }
}
