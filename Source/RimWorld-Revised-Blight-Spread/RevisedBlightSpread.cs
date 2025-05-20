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
        static bool Prefix(Blight _instance)
        {
            IntVec3 center = _instance.Position;
            Map map = _instance.Map;

            GenRadial.ProcessEquidistantCells(center, 4f, cells =>
            {
                if (NoWalls(cells))
                {
                    // no walls detected, do original
                    return true;
                } else
                {
                    // do patch version
                    
                }
                return false;
            }, map);

            return false;
        }

        // check for walls in all of the cells
        private static bool NoWalls(List<IntVec3> cells) 
        {
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



