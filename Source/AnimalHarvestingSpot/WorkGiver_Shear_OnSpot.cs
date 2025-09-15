using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class WorkGiver_Shear_OnSpot : WorkGiver_Shear
{
    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var job = base.JobOnThing(pawn, t, forced);
        if (job?.def != JobDefOf.Shear)
        {
            return job;
        }

        var spot = findSpot(pawn);
        if (spot == null
            || !pawn.CanReserveAndReach(spot, PathEndMode.InteractionCell, Danger.Some, 1, -1, null, forced)
            || pawn.Map.designationManager.DesignationOn(spot, DesignationDefOf.Deconstruct) != null)
        {
            return job;
        }

        if (job != null)
        {
            job.targetB = spot;
        }

        return job;
    }

    private static Thing findSpot(Pawn pawn)
    {
        var list = new List<Thing>();
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalHarvestingSpot));
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalShearingSpot));

        if (!list.Any())
        {
            return null;
        }

        return GenClosest.ClosestThing_Global_Reachable(
            pawn.Position, pawn.Map, list,
            PathEndMode.Touch, TraverseParms.For(pawn), 999f);
    }
}