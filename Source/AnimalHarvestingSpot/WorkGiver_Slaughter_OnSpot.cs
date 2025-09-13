using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class WorkGiver_Slaughter_OnSpot : WorkGiver_Slaughter
{
    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        Job job = base.JobOnThing(pawn, t, forced);
        if( job?.def == JobDefOf.Slaughter )
        {
            Thing spot = FindSpot(pawn);
            if (spot != null
                && pawn.CanReserveAndReach(spot, PathEndMode.InteractionCell, Danger.Some, 1, -1, null, forced)
                && pawn.Map.designationManager.DesignationOn(spot, DesignationDefOf.Deconstruct) == null)
            {
                job.targetB = spot;
            }
        }
        return job;
    }

    private static Thing FindSpot(Pawn pawn)
    {
        var list = new List<Thing>();
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalHarvestingSpot));
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalSlaughteringSpot));

        if (!list.Any())
            return null;

        return GenClosest.ClosestThing_Global_Reachable(
            pawn.Position, pawn.Map, list,
            PathEndMode.Touch, TraverseParms.For(pawn), 999f);
    }
}
