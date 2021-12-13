using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;
using UnityEngine;

namespace PowerView
{
    [StaticConstructorOnStartup]
    class Textures
    {
        public static readonly Texture2D toggleIcon = ContentFinder<Texture2D>.Get("UI/Playsettings/gd-powerview");
    }
}
