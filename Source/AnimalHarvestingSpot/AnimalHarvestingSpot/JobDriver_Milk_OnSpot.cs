using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

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

