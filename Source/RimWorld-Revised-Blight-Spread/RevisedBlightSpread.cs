using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace RevisedBlightSpread
{
    [StaticConstructorOnStartup]
    public static class RevisedBlightSpread
    {
        static RevisedBlightSpread()
        {
            Log.Message("Static RevisedBlightSpread Class Loaded");
            Harmony.DEBUG = true;
            Harmony harmony = new Harmony("rimworld.archonwu.revisedblightspread");
            harmony.PatchAll();
            Harmony.DEBUG = false;
        }
    }

    [HarmonyPatch]
    public static class TryReproduceNow_Patch
    {
        [HarmonyPatch(typeof(Blight), nameof(Blight.TryReproduceNow))]
        [HarmonyPrefix]
        static bool Prefix(Blight __instance)
        {
            IntVec3 center = __instance.Position;
            Map map = __instance.Map;
            bool wallsFound = false;
            Log.Message("TRN");

            // go through the nearby cells radially once first, to see if there's any walls within
            GenRadial.ProcessEquidistantCells(center, 4f, cells =>
            {
                Log.Message($"{center}, {cells.Count}");
                foreach (IntVec3 cell in cells)
                {
                    Thing edifice = map.edificeGrid[cell];  // edifices checks for wall-like structures
                    if (edifice != null)
                    {
                        Log.Message($"Wall detected at {cell}, {edifice.def.defName}");
                        wallsFound = true;
                    }
                }
                return false;
            }, map);

            if (!wallsFound)
            {
                return true; // run original method
            }

            // my logic
            if (wallsFound)
            {
                RevisedBlightSpreadAlgorithm(__instance);                
            }

            return false;   // skip original method
        }


        private static void RevisedBlightSpreadAlgorithm(Blight __instance)
        {
            IntVec3 center = __instance.Position;
            Map map = __instance.Map;
            GenRadial.ProcessEquidistantCells(center, 4f, cells =>
            {
               
                if (cells.Where((IntVec3 x) => BlightUtility.GetFirstBlightableNowPlant(x, map) != null).TryRandomElement(out var result))
                {
                    bool hasLineOfSight = GenSight.LineOfSight(center, result, map);
                    if (hasLineOfSight)     // if there's LOS, no impassable blocks is in between, proceed to blight plant
                    {
                        BlightUtility.GetFirstBlightableNowPlant(result, map).CropBlighted();
                        return true;
                    }
                }
                return false;
            }, map);
        }
    }
}



