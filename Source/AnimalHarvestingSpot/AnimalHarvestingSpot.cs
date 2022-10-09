using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public abstract class AnimalHarvestingSpot : JobDriver_GatherAnimalBodyResources
{
    protected Thing spot;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref spot, "harvesting_spot");
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

        var CallVictim = new Toil
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
                if (Find.TickManager.TicksGame % 200 != 0)
                {
                    return;
                }

                var target = TargetA.Thing as Pawn;
                if (Prefs.DevMode)
                {
                    //Log.Message("JobDriver_AnimalGatheringOnSpot: waiting target " + target);
                }

                if (target != null && pawn.Position.DistanceToSquared(target.Position) < 32f)
                {
                    var waitjob = new Job(JobDefOf.Wait, 200);
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

        var ReleaseVictim = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        ReleaseVictim.AddFinishAction(() =>
        {
            if (TargetA.Thing is Pawn target)
            {
                target.ClearMind(true);
            }
        });
        yield return ReleaseVictim;

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