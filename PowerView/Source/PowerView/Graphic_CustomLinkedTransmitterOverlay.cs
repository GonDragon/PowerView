using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Verse;
using RimWorld;

namespace PowerView
{
    public class Graphic_CustomLinkedTransmitterOverlay : Graphic_LinkedTransmitterOverlay
    {
        private static readonly Color32[] qua_default = new Color32[4]
        {
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
            new Color32(byte.MaxValue,byte.MaxValue,byte.MaxValue,byte.MaxValue),
        };

        public Graphic_CustomLinkedTransmitterOverlay(Graphic subGraphic) : base(subGraphic) { }

        public override void Print(SectionLayer layer, Thing parent, float extraRotation) => Print(layer, parent, qua_default);

        public void Print(SectionLayer layer, Thing parent, Color32[] quaternion)
        {
            foreach (IntVec3 cell in parent.OccupiedRect())
            {
                Vector3 shiftedWithAltitude = cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay);

                Printer_Plane.PrintPlane(layer, shiftedWithAltitude, new Vector2(1f, 1f), this.LinkedDrawMatFrom(parent,cell), colors: quaternion);
            }
        }

        protected override Material LinkedDrawMatFrom(Thing parent, IntVec3 cell)
        {
            int num1 = 0;
            int num2 = 1;
            for (int index = 0; index < 4; ++index)
            {
                if (this.ShouldLinkWith(cell + GenAdj.CardinalDirections[index], parent))
                    num1 += num2;
                num2 *= 2;
            }
            LinkDirections LinkSet = (LinkDirections)num1;
            return MaterialAtlasPool.SubMaterialFromAtlas(this.subGraphic.MatSingleFor(parent), LinkSet);
        }
    }
}
