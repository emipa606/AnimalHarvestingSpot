using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot
{
    public class JobDriver_Train_OnSpot: JobDriver_Train
    {
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref spot, "harvesting_spot");
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Prefs.DevMode) Log.Message("JobDriver_Train_OnSpot: try to reserve!");
            List<Thing> list = new List<Thing>();
            foreach (Building bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalTrainingSpot")))
            {
                list.Add(bld as Thing);
            }
            Pawn target = TargetA.Thing as Pawn;
            spot = GenClosest.ClosestThing_Global_Reachable(
                target.Position, target.Map, list,
                PathEndMode.Touch, TraverseParms.For(target, Danger.Deadly, TraverseMode.ByPawn, false), 999f, null, null);

            if (spot != null && CanTarget(target))
            {
                if (Prefs.DevMode) Log.Message("JobDriver_Train_OnSpot: spot is " + spot);
                return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null);
            }
            if (Prefs.DevMode) Log.Message("JobDriver_Train_OnSpot: spot is not found or unreachable");
            spot = null;
            return true;
        }

        Thing spot = null;
        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (spot != null && !spot.Destroyed)
            {
                Toil CallVictim = new Toil()
                {
                    initAction = () =>
                    {
                        Pawn target = TargetA.Thing as Pawn;
                        if (Prefs.DevMode) Log.Message("JobDriver_Train_OnSpot: goto spot with " + target);
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
                        if (Find.TickManager.TicksGame % 100 == 0)
                        {
                            Pawn target = TargetA.Thing as Pawn;
                            if (Prefs.DevMode) Log.Message("JobDriver_Train_OnSpot: waiting on target " + target);
                            if (pawn.Position.DistanceToSquared(target.Position) < 32f)
                            {
                                Job waitjob = new Job(JobDefOf.Wait, 100);
                                target.jobs.StartJob(waitjob, JobCondition.InterruptForced, null, false, true, null, JobTag.MiscWork, false);
                                this.ReadyForNextToil();
                            }
                            else
                            {
                                target.jobs.EndCurrentJob(JobCondition.InterruptOptional, false);
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

