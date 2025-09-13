using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace AnimalHarvestingSpot;

public class JobDriver_Slaughter_OnSpot : JobDriver_Slaughter
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (TargetB.Thing != null && TargetA.Thing is Pawn target && CanTarget(target))
        {
            return pawn.Reserve(target, job);
        }

        return base.TryMakePreToilReservations(errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (TargetB.Thing == null || TargetB.Thing.Destroyed || !TargetB.Thing.Spawned
            || TargetB.Thing.Map != TargetA.Thing.Map
            || !TargetB.Thing.Position.IsValid || !(TargetA.Thing is Pawn pawn) || TargetB.Thing.Position.IsForbidden(pawn))
        {
            foreach (var makeNewToil in base.MakeNewToils())
            {
                yield return makeNewToil;
            }

            yield break;
        }

        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

        var callVictim = new Toil
        {
            initAction = () =>
            {
                var target = TargetA.Thing as Pawn;
                var gotoJob = new Job(JobDefOf.GotoWander, TargetB.Thing.Position)
                {
                    locomotionUrgency = LocomotionUrgency.Sprint
                };
                target?.jobs.StartJob(gotoJob, JobCondition.InterruptOptional, null, true, false, null,
                    JobTag.MiscWork);
            },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
        callVictim.FailOnDespawnedNullOrForbidden(TargetIndex.B);
        yield return callVictim;

        yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDestroyedNullOrForbidden(TargetIndex.B);

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

                    target.jobs.EndCurrentJob(JobCondition.InterruptForced, false);
                    var gotojob = new Job(JobDefOf.GotoWander, TargetB.Thing.Position)
                    {
                        locomotionUrgency = LocomotionUrgency.Sprint
                    };
                    target.jobs.StartJob(gotojob, JobCondition.InterruptOptional, null, true, false, null,
                        JobTag.MiscWork);
                }
            }
        };
        waitVictim.AddFinishAction(() =>
        {
            var target = TargetA.Thing as Pawn;
            target?.jobs.EndCurrentJob(JobCondition.InterruptForced);
        });
        waitVictim.FailOnDespawnedNullOrForbidden(TargetIndex.B);
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