using System;
using System.Collections.Generic;
using System.Diagnostics;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot
{
	public class JobDriver_Shear_OnSpot: AnimalHarvestingSpot
    {
		protected override float WorkTotal
		{
			get
			{
				return 1700f;
			}
		}
		protected override CompHasGatherableBodyResource GetComp(Pawn animal)
		{
			return animal.TryGetComp<CompShearable>();
		}
	}
}

