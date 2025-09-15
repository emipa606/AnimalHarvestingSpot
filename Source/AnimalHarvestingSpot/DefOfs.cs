using RimWorld;
using Verse;

namespace AnimalHarvestingSpot;

[DefOf]
public static class DefOfs
{
    public static ThingDef AnimalHarvestingSpot;
    public static ThingDef AnimalMilkingSpot;
    public static ThingDef AnimalShearingSpot;
    public static ThingDef AnimalSlaughteringSpot;
    public static ThingDef AnimalTrainingSpot;

    static DefOfs()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(DefOfs));
    }
}
