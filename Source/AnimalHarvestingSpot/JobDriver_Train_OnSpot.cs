using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class JobDriver_Train_OnSpot : JobDriver_Train
{
    private Thing spot;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref spot, "harvesting_spot");
    }

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        var list = pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("AnimalTrainingSpot"));

        if (!list.Any())
        {
            return base.TryMakePreToilReservations(errorOnFailed);
        }

        if (TargetA.Thing is Pawn target)
        {
            spot = GenClosest.ClosestThing_Global_Reachable(
                target.Position, target.Map, list,
                PathEndMode.Touch, TraverseParms.For(target), 999f);

            if (spot != null && CanTarget(target))
            {
                return pawn.Reserve(job.GetTarget(TargetIndex.A), job);
            }
        }

        spot = null;
        return base.TryMakePreToilReservations(errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (spot == null || spot.Destroyed)
        {
            foreach (var makeNewToil in base.MakeNewToils())
            {
                yield return makeNewToil;
            }

            yield break;
        }

        var callVictim = new Toil
        {
            initAction = () =>
            {
                var target = TargetA.Thing as Pawn;
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
        callVictim.FailOnDespawnedOrNull(TargetIndex.A);
        yield return callVictim;

        var waitVictim = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never,
            tickAction = () =>
            {
                if (Find.TickManager.TicksGame % 100 != 0)
                {
                    return;
                }

                var target = TargetA.Thing as Pawn;

                if (target != null && pawn.Position.DistanceToSquared(target.Position) < 32f)
                {
                    var waitJob = new Job(JobDefOf.Wait, 100);
                    target.jobs.StartJob(waitJob, JobCondition.InterruptForced, null, false, true, null,
                        JobTag.MiscWork);
                    ReadyForNextToil();
                }
                else
                {
                    if (target == null)
                    {
                        return;
                    }

                    target.jobs.EndCurrentJob(JobCondition.InterruptOptional, false);
                    var gotoJob = new Job(JobDefOf.GotoWander, spot.Position)
                    {
                        locomotionUrgency = LocomotionUrgency.Sprint
                    };
                    target.jobs.StartJob(gotoJob, JobCondition.InterruptOptional, null, true, false, null,
                        JobTag.MiscWork);
                }
            }
        };
        waitVictim.AddFinishAction(() =>
        {
            if (TargetA.Thing is Pawn target)
            {
                target.jobs.EndCurrentJob(JobCondition.InterruptForced);
            }
        });
        waitVictim.FailOnDespawnedOrNull(TargetIndex.A);
        yield return waitVictim;

        foreach (var toil in base.MakeNewToils())
        {
            yield return toil;
        }
    }

    protected virtual bool CanTarget(Pawn trg)
    {
        return !(trg.GetStatValue(StatDefOf.MoveSpeed) <= pawn.GetStatValue(StatDefOf.MoveSpeed) / 2f);
    }
}