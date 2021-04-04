using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot
{
    public class JobDriver_Shear_OnSpot : AnimalHarvestingSpot
    {
        protected override float WorkTotal => 1700f;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Prefs.DevMode)
            {
                //Log.Message("JobDriver_AnimalGatheringOnSpot: try to reserve!");
            }

            var list = new List<Thing>();
            foreach (var bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
                ThingDef.Named("AnimalHarvestingSpot")))
            {
                list.Add(bld);
            }

            foreach (var bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalShearingSpot"))
            )
            {
                list.Add(bld);
            }

            if (TargetA.Thing is Pawn target)
            {
                spot = GenClosest.ClosestThing_Global_Reachable(
                    target.Position, target.Map, list,
                    PathEndMode.Touch, TraverseParms.For(target), 999f);

                if (spot != null && CanTarget(target))
                {
                    if (Prefs.DevMode)
                    {
                        //Log.Message("JobDriver_AnimalGatheringOnSpot: spot is " + spot);
                    }

                    return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
                }
            }

            if (Prefs.DevMode)
            {
                Log.Message("JobDriver_AnimalGatheringOnSpot: spot is null or cant be targeted");
            }

            spot = null;
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        protected override CompHasGatherableBodyResource GetComp(Pawn animal)
        {
            return animal.TryGetComp<CompShearable>();
        }
    }
}