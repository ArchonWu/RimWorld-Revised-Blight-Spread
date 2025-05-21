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
            Log.Message("TRN");
            GenRadial.ProcessEquidistantCells(center, 4f, cells =>
            {
                Log.Message(cells.Count);
                if (NoWalls(cells, map))
                {
                    // no walls detected, do original TryReproduceNow()
                    return true;
                } else
                {
                    // do patch version
                    Log.Message("PATCH VERSION ING");
                    
                }
                return false;
            }, map);

            return false;
        }

        // check for walls in all of the cells
        private static bool NoWalls(List<IntVec3> cells, Map map) 
        {

            Log.Message($"Checking for walls! Number of cells: {cells.Count}");
            foreach (IntVec3 cell in cells)
            {
                Log.Message($"Checking {cell}");
                Thing edifice = map.edificeGrid[cell];  // edifices checks for wall-like structures
                if (edifice!=null)
                {
                    Log.Message($"Wall detected at {cell}, {edifice.def.defName}");
                    return false;
                }
            }

            Log.Message("No walls detected!");

            return true;
        }


        // for each tile, check all 8 adjacent tiles for walls
        // if wall is found in one direction, that direction is considered blocked by wall
        private static bool IsBlockedByWall()
        {
            return false;
        }

    }
}



