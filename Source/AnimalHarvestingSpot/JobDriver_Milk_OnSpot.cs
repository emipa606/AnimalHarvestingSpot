using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class JobDriver_Milk_OnSpot : AnimalHarvestingSpot
{
    protected override float WorkTotal => 400f;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (TargetB.Thing != null && TargetA.Thing is Pawn target && CanTarget(target))
        {
            return pawn.Reserve(target, job);
        }

        return base.TryMakePreToilReservations(errorOnFailed);
    }

    protected override CompHasGatherableBodyResource GetComp(Pawn animal)
    {
        return animal.TryGetComp<CompMilkable>();
    }
}