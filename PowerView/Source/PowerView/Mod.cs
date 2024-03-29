﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using HarmonyLib;
using System.Reflection;
using RimWorld;

namespace PowerView
{
    [StaticConstructorOnStartup]
    public static class Mod
    {
        public static string Author => "GonDragon";
        public static string Name => Assembly.GetName().Name;
        public static string Id => Author + "." + Name;

        public static string Version => Assembly.GetName().Version.ToString();

        private static Assembly Assembly
        {
            get
            {
                return Assembly.GetAssembly(typeof(Mod));
            }
        }

        public static readonly Harmony Harmony;

        static Mod()
        {
#if DEBUG
            Mod.Log("Mod Loaded");
#endif
            Harmony = new Harmony(Id);
            Harmony.PatchAll();

            if (!LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Vanilla UI Expanded"))
            {
                Harmony.Patch(AccessTools.Method(typeof(PlaySettings), "DoPlaySettingsGlobalControls"), new HarmonyMethod(typeof(DoPlaySettingsGlobalControlsPatch).GetMethod("Postfix")));
            }
        }

        public static void Log(string message) => Verse.Log.Message(PrefixMessage(message));

        public static void Warning(string message) => Verse.Log.Warning(PrefixMessage(message));

        public static void Error(string message) => Verse.Log.Error(PrefixMessage(message));

        public static void ErrorOnce(string message, string key) => Verse.Log.ErrorOnce(PrefixMessage(message), key.GetHashCode());

        public static void Message(string message) => Messages.Message(message, MessageTypeDefOf.TaskCompletion, false);

        private static string PrefixMessage(string message) => $"[{Name} v{Version}] {message}";
    }
}
