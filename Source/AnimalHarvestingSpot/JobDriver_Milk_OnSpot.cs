using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot
{
    public class JobDriver_Milk_OnSpot: AnimalHarvestingSpot
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: try to reserve!");
            List<Thing> list = new List<Thing>();
            foreach (Building bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalHarvestingSpot")))
            {
                list.Add(bld as Thing);
            }
            foreach (Building bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalMilkingSpot")))
            {
                list.Add(bld as Thing);
            }
            Pawn target = TargetA.Thing as Pawn;
            base.spot = GenClosest.ClosestThing_Global_Reachable(
                target.Position, target.Map, list,
                PathEndMode.Touch, TraverseParms.For(target, Danger.Deadly, TraverseMode.ByPawn, false), 999f, null, null);

            if (spot != null && CanTarget(target))
            {
                if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: spot is " + spot);
                return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null);
            }
            if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: spot is null or cant be targeted");
            spot = null;
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        protected override float WorkTotal
		{
			get
			{
				return 400f;
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompMilkable>();
		}
	}
}

