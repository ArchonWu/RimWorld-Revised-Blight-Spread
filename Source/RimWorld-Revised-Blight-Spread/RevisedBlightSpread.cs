using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;

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

            return !wallsFound;
        }


        private static void RevisedSpreadBlightAlgorithm()
        {

        }



        // for each tile, check all 8 adjacent tiles for walls
        // if wall is found in one direction, that direction is considered blocked by wall
        private static bool IsBlockedByWall()
        {
            return false;
        }

    }
}



