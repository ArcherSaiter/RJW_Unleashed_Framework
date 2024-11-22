using RimWorld;
using Verse;

namespace RJW_Unleashed_Framework
{
    public class SourceEffectLinkEx : DefModExtension
    {
        public ThingDef sourceName;
        public HediffDef effectHediffDef;
        public ThoughtDef effectThoughtDef;
        public TraitDef effectTraitDef;
        public GeneDef effectGeneDef;

        public float severityMultiplier;
        public float thoughtDurationDays;

        public double traitGainProbability;
        public double geneGainProbability;

        public bool traitForced;
        public bool addAsXenogene;
        public bool considerBodysize;
        public bool applicableForMales;
        public bool applicableForFemales;
    }
    public class TransformationAddon : DefModExtension
    {
        public GeneDef transformationGene;

        public bool applicableForFemales;
        public bool applicableForMales;
    }
}
