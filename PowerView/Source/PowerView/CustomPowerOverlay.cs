using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PowerView
{
    [StaticConstructorOnStartup]
    internal static class CustomPowerOverlay
    {
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicInactive;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicNegative;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicOnBattery;
        public static readonly Graphic_LinkedTransmitterOverlay LinkedOverlayGraphicPositive;
        public static readonly Texture2D toggleIcon = ContentFinder<Texture2D>.Get("UI/Playsettings/gd-powerview");
        public static bool enabled;
        private static readonly ColorInt inactive = new ColorInt(200, 200, 200, 100);
        private static readonly ColorInt negative = new ColorInt(250, 90, 90, 200);
        private static readonly ColorInt onBattery = new ColorInt(225, 225, 0, 200);
        private static readonly ColorInt positive = new ColorInt(200, 200, 255, 200);
        private static readonly Dictionary<int, PowerStatus> powernetStatusCache = new Dictionary<int, PowerStatus>();

        private static float lastUpdated = 0;

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
        private enum PowerStatus : byte
        {
            inactive,
            positive,
            onBattery,
            negative
        }

        public static void DoWhileOverlayOn()
        {
            OverlayDrawHandler.DrawPowerGridOverlayThisFrame();
            if (Time.realtimeSinceStartup - lastUpdated > 1)
            {
                powernetStatusCache.Clear();
                Find.CurrentMap.powerNetManager.NotifyDrawersForWireUpdate(Find.CameraDriver.MapPosition);
                lastUpdated = Time.realtimeSinceStartup;
            }
        }

        public static void PrintPowerOverlayThing(SectionLayer layer, Thing parent)
        {
            PowerNet powerNet = ((Building)parent).PowerComp.PowerNet;
            int powernetHash = powerNet.GetHashCode();

            if (!powernetStatusCache.ContainsKey(powernetHash))
            {
                //The way count the powergain was simplified using the "PowerMetrics" function of Pustalorc Active-Circuits github as an example
                // https://github.com/Pustalorc/Active-Circuits/blob/main/Source/ActiveCircuitsBase.cs

                float produced = 0;
                float consumed = 0;

                foreach (CompPowerTrader comp in powerNet.powerComps)
                {
                    if (comp.PowerOutput > 0)
                    {
                        produced += comp.PowerOutput;
                    }
                    else
                    {
                        if (comp.parent.IsBrokenDown()) continue;

                        CompFlickable comp1 = comp.parent.TryGetComp<CompFlickable>();
                        if (comp1 != null && !comp1.SwitchIsOn) continue;

                        CompSchedule comp2 = comp.parent.TryGetComp<CompSchedule>();
                        if (comp2 != null && !comp2.Allowed) continue;

                        consumed += comp.PowerOutput;
                    }
                }

                float netPowerGain = produced + consumed;

                powernetStatusCache[powernetHash] = consumed != 0 ? (netPowerGain > 0 ? PowerStatus.positive : (powerNet.CurrentStoredEnergy() > 0 ? PowerStatus.onBattery : PowerStatus.negative)) : PowerStatus.inactive;
            }

            CustomPowerOverlay.Print(powernetStatusCache[powernetHash], layer, parent);
        }

        private static void Print(PowerStatus status, SectionLayer layer, Thing thing)
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
    }
}