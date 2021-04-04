using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot
{
    public class JobDriver_Slaughter_OnSpot : JobDriver_Slaughter
    {
        private Thing spot;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref spot, "harvesting_spot");
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (Prefs.DevMode)
            {
                //Log.Message("JobDriver_Slaughter_OnSpot: try to reserve!");
            }

            var list = new List<Thing>();
            foreach (var bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
                ThingDef.Named("AnimalHarvestingSpot")))
            {
                list.Add(bld);
            }

            foreach (var bld in pawn.Map.listerBuildings.AllBuildingsColonistOfDef(
                ThingDef.Named("AnimalSlaughteringSpot")))
            {
                list.Add(bld);
            }

            //if (Prefs.DevMode) Log.Message($"JobDriver_Slaughter_OnSpot: found following things: {string.Join(",", list)}");
            if (TargetA.Thing is Pawn target)
            {
                spot = GenClosest.ClosestThing_Global_Reachable(
                    target.Position, target.Map, list,
                    PathEndMode.Touch, TraverseParms.For(target), 999f);

                if (spot != null && CanTarget(target))
                {
                    if (Prefs.DevMode)
                    {
                        //Log.Message("JobDriver_Slaughter_OnSpot: spot is " + spot);
                    }

                    return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
                }
            }

            if (Prefs.DevMode)
            {
                Log.Message("JobDriver_Slaughter_OnSpot: spot is not found or unreachable");
            }

            spot = null;
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            if (spot != null && !spot.Destroyed)
            {
                var CallVictim = new Toil
                {
                    initAction = () =>
                    {
                        var target = TargetA.Thing as Pawn;
                        if (Prefs.DevMode)
                        {
                            //Log.Message("JobDriver_Slaughter_OnSpot: goto spot with" + target);
                        }

                        pawn.pather.StartPath(spot.Position, PathEndMode.Touch);
                        var gotojob = new Job(JobDefOf.GotoWander, spot.Position)
                        {
                            locomotionUrgency = LocomotionUrgency.Sprint
                        };
                        target?.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null,
                            JobTag.MiscWork);
                    },
                    defaultCompleteMode = ToilCompleteMode.PatherArrival
                };
                CallVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return CallVictim;

                var WaitVictim = new Toil
                {
                    defaultCompleteMode = ToilCompleteMode.Never,
                    tickAction = () =>
                    {
                        if (Find.TickManager.TicksGame % 100 != 0)
                        {
                            return;
                        }

                        var target = TargetA.Thing as Pawn;
                        if (Prefs.DevMode)
                        {
                            //Log.Message("JobDriver_Slaughter_OnSpot: waiting target" + target);
                        }

                        if (target != null && pawn.Position.DistanceToSquared(target.Position) < 32f)
                        {
                            var waitjob = new Job(JobDefOf.Wait, 100);
                            target.jobs.StartJob(waitjob, JobCondition.InterruptForced, null, false, true, null,
                                JobTag.MiscWork);
                            ReadyForNextToil();
                        }
                        else
                        {
                            if (target == null)
                            {
                                return;
                            }

                            target.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                            var gotojob = new Job(JobDefOf.GotoWander, spot.Position)
                            {
                                locomotionUrgency = LocomotionUrgency.Sprint
                            };
                            target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null,
                                JobTag.MiscWork);
                        }
                    }
                };
                WaitVictim.AddFinishAction(() =>
                {
                    var target = TargetA.Thing as Pawn;
                    target?.jobs.EndCurrentJob(JobCondition.InterruptForced);
                });
                WaitVictim.FailOnDespawnedOrNull(TargetIndex.A);
                yield return WaitVictim;
            }

            foreach (var toil in base.MakeNewToils())
            {
                yield return toil;
            }
        }

        protected virtual bool CanTarget(Pawn trg)
        {
            if (trg.GetStatValue(StatDefOf.MoveSpeed) <= pawn.GetStatValue(StatDefOf.MoveSpeed) / 2f)
            {
                return false;
            }

            return true;
        }
    }
}