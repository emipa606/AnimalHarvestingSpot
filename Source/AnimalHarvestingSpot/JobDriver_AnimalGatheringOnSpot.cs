using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;


namespace AnimalHarvestingSpot
{
    public abstract class AnimalHarvestingSpot : JobDriver_GatherAnimalBodyResources
    {
        public Thing spot = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref spot, "harvesting_spot");
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (spot != null && !spot.Destroyed)
            {
                var CallVictim = new Toil()
                {
                    initAction = () =>
                    {
                        var target = TargetA.Thing as Pawn;
                        if (Prefs.DevMode)
                        {
                            //Log.Message("JobDriver_AnimalGatheringOnSpot: goto spot with " + target);
                        }

                        pawn.pather.StartPath(spot.Position, PathEndMode.Touch);
                        var gotojob = new Job(JobDefOf.GotoWander, spot.Position)
                        {
                            locomotionUrgency = LocomotionUrgency.Sprint
                        };
                        target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null, JobTag.MiscWork, false);
                    },
                    defaultCompleteMode = ToilCompleteMode.PatherArrival
                };
                CallVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return CallVictim;
                var WaitVictim = new Toil()
                {
                    defaultCompleteMode = ToilCompleteMode.Never,
                    tickAction = () =>
                    {
                        if (Find.TickManager.TicksGame % 200 == 0)
                        {
                            var target = TargetA.Thing as Pawn;
                            if (Prefs.DevMode)
                            {
                                //Log.Message("JobDriver_AnimalGatheringOnSpot: waiting target " + target);
                            }

                            if (pawn.Position.DistanceToSquared(target.Position) < 32f)
                            {
                                var waitjob = new Job(JobDefOf.Wait, 200);
                                target.jobs.StartJob(waitjob, JobCondition.InterruptForced, null, false, true, null, JobTag.MiscWork, false);
                                ReadyForNextToil();
                            }
                            else
                            {
                                target.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                                var gotojob = new Job(JobDefOf.GotoWander, spot.Position)
                                {
                                    locomotionUrgency = LocomotionUrgency.Sprint
                                };
                                target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null, JobTag.MiscWork, false);
                            }
                        }
                    }
                };
                WaitVictim.AddFinishAction(() =>
                {
                    var target = TargetA.Thing as Pawn;
                    target.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
                });
                WaitVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return WaitVictim;

                var ReleaseVictim = new Toil()
                {
                    defaultCompleteMode = ToilCompleteMode.Instant,
                };
                ReleaseVictim.AddFinishAction(() =>
                {
                    if (TargetA.Thing is Pawn target)
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
            if (trg.GetStatValue(StatDefOf.MoveSpeed, true) <= pawn.GetStatValue(StatDefOf.MoveSpeed, true) / 2f)
            {
                return false;
            }
            return true;
        }
    }
}

