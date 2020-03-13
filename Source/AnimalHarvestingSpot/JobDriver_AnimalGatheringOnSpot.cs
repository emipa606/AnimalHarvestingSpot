using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;


namespace AnimalHarvestingSpot
{
    public abstract class AnimalHarvestingSpot : JobDriver_GatherAnimalBodyResources
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: try to reserve!");
            List<Thing> list = new List<Thing>();
            foreach (Building bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalHarvestingSpot")))
            {
                list.Add(bld as Thing);
            }
            Pawn target = TargetA.Thing as Pawn;
            spot = GenClosest.ClosestThing_Global_Reachable(
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

        Thing spot = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref spot, "harvesting_spot");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (spot != null && !spot.Destroyed)
            {
                Toil CallVictim = new Toil()
                {
                    initAction = () =>
                    {
                        Pawn target = TargetA.Thing as Pawn;
                        if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: goto spot with " + target);
                        pawn.pather.StartPath(spot.Position, PathEndMode.Touch);
                        Job gotojob = new Job(JobDefOf.GotoWander, spot.Position);
                        gotojob.locomotionUrgency = LocomotionUrgency.Sprint;
                        target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null, JobTag.MiscWork, false);
                    },
                    defaultCompleteMode = ToilCompleteMode.PatherArrival
                };
                CallVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return CallVictim;
                Toil WaitVictim = new Toil()
                {
                    defaultCompleteMode = ToilCompleteMode.Never,
                    tickAction = () =>
                    {
                        if (Find.TickManager.TicksGame % 200 == 0)
                        {
                            Pawn target = TargetA.Thing as Pawn;
                            if (Prefs.DevMode) Log.Message("JobDriver_AnimalGatheringOnSpot: waiting target " + target);
                            if (pawn.Position.DistanceToSquared(target.Position) < 32f)
                            {
                                Job waitjob = new Job(JobDefOf.Wait);
                                target.jobs.StartJob(waitjob, JobCondition.InterruptForced, null, false, true, null, JobTag.MiscWork, false);
                                this.ReadyForNextToil();
                            }
                            else
                            {
                                target.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                                Job gotojob = new Job(JobDefOf.GotoWander, spot.Position);
                                gotojob.locomotionUrgency = LocomotionUrgency.Sprint;
                                target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null, JobTag.MiscWork, false);
                            }
                        }
                    }
                };
                WaitVictim.AddFinishAction(() =>
                {
                    Pawn target = TargetA.Thing as Pawn;
                    target.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
                });
                WaitVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return WaitVictim;

                Toil ReleaseVictim = new Toil()
                {
                    defaultCompleteMode = ToilCompleteMode.Instant,
                };
                ReleaseVictim.AddFinishAction(() =>
                {
                    Pawn target = TargetA.Thing as Pawn;
                    if (target != null)
                    {
                        target.ClearMind(true);
                    }
                });
                yield return ReleaseVictim;
            }
            foreach (Toil toil in base.MakeNewToils())
            {
                yield return toil;
            }
        }

        protected virtual bool CanTarget(Pawn trg)
        {
            if (trg.GetStatValue(StatDefOf.MoveSpeed, true) <= this.pawn.GetStatValue(StatDefOf.MoveSpeed, true) / 2f)
            {
                return false;
            }
            return true;
        }
    }
}

