using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class JobDriver_Shear_OnSpot : AnimalHarvestingSpot
{
    protected override float WorkTotal => 1700f;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var list = new List<Thing>();
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalHarvestingSpot));
        list.AddRange(pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
            DefOfs.AnimalShearingSpot));

        if (!list.Any())
        {
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        if (TargetA.Thing is Pawn target)
        {
            Spot = GenClosest.ClosestThing_Global_Reachable(
                target.Position, target.Map, list,
                PathEndMode.Touch, TraverseParms.For(target), 999f);

            if (Spot != null && CanTarget(target))
            {
                return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
            }
        }

        Spot = null;
        return base.TryMakePreToilReservations(errorOnFailed);
    }

    protected override CompHasGatherableBodyResource GetComp(Pawn animal)
    {
        return animal.TryGetComp<CompShearable>();
    }
}