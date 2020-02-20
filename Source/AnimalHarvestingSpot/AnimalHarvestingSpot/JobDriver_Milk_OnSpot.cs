using RimWorld;
using Verse;

namespace AnimalHarvestingSpot
{
    public class JobDriver_Milk_OnSpot: AnimalHarvestingSpot
    {
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

