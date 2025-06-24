using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public abstract class AnimalHarvestingSpot : JobDriver_GatherAnimalBodyResources
{
    protected Thing Spot;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref Spot, "harvesting_spot");
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (Spot == null || Spot.Destroyed)
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

                pawn.pather.StartPath(Spot.Position, PathEndMode.Touch);
                var gotojob = new Job(JobDefOf.GotoWander, Spot.Position)
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
                if (Find.TickManager.TicksGame % 200 != 0)
                {
                    return;
                }

                var target = TargetA.Thing as Pawn;

                if (target != null && pawn.Position.DistanceToSquared(target.Position) < 32f)
                {
                    var waitJob = new Job(JobDefOf.Wait, 200);
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

                    target.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                    var gotoJob = new Job(JobDefOf.GotoWander, Spot.Position)
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
            var target = TargetA.Thing as Pawn;
            target?.jobs.EndCurrentJob(JobCondition.InterruptForced);
        });
        waitVictim.FailOnDespawnedOrNull(TargetIndex.A);
        yield return waitVictim;

        var releaseVictim = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        releaseVictim.AddFinishAction(() =>
        {
            if (TargetA.Thing is Pawn target)
            {
                target.ClearMind(true);
            }
        });
        yield return releaseVictim;

        foreach (var toil in base.MakeNewToils())
        {
            yield return toil;
        }
    }

    protected bool CanTarget(Pawn trg)
    {
        return !(trg.GetStatValue(StatDefOf.MoveSpeed) <= pawn.GetStatValue(StatDefOf.MoveSpeed) / 2f);
    }
}