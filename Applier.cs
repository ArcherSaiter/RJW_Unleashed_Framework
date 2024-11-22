using System;
using RimWorld;
using Verse;

namespace RJW_Unleashed_Framework
{
    public class ApplyEffect
    {
        public static void FullExtension(Pawn pawn, ThingDef effectSourceDef)
        {
            if (effectSourceDef == null || pawn == null) return;
            SourceEffectLinkEx Ext = effectSourceDef.GetModExtension<SourceEffectLinkEx>();
            if (Ext == null) return;

            ThingDef sourceName = Ext.sourceName;
            HediffDef effectHediffDef = Ext.effectHediffDef;
            ThoughtDef effectThoughtDef = Ext.effectThoughtDef;
            TraitDef effectTraitDef = Ext.effectTraitDef;
            GeneDef effectGeneDef = Ext.effectGeneDef;

            float severityMultiplier = Ext.severityMultiplier;
            float thoughtDurationDays = Ext.thoughtDurationDays;
            double traitGainProbability = Ext.traitGainProbability;
            double geneGainProbability = Ext.geneGainProbability;

            bool traitForced = Ext.traitForced;
            bool addAsXenogene = Ext.addAsXenogene;
            bool considerBodysize = Ext.considerBodysize;
            bool applicableForMales = Ext.applicableForMales;
            bool applicableForFemales = Ext.applicableForFemales;

            if (!applicableForMales && pawn.gender == Gender.Male)
            {
                return;
            }
            if (!applicableForFemales && pawn.gender == Gender.Female)
            {
                return;
            }
            if (effectHediffDef != null && severityMultiplier > 0)
            {
                GiveHediff(pawn, effectHediffDef, severityMultiplier, considerBodysize);
            }
            if (effectThoughtDef != null && thoughtDurationDays > 0)
            {
                GiveThought(pawn, effectThoughtDef, thoughtDurationDays);
            }
            if (effectTraitDef != null && traitGainProbability > 0.0)
            {
                double traitRoll = new System.Random().NextDouble();
                if (traitRoll <= traitGainProbability)
                {
                    if (!traitForced)
                    {
                        GiveTrait(pawn, effectTraitDef);
                    }
                    else
                    {
                        ForceTrait(pawn, effectTraitDef);
                    }
                }
            }
            if (effectGeneDef != null && geneGainProbability > 0.0)
            {
                double geneRoll = new System.Random().NextDouble();
                if (geneRoll <= traitGainProbability)
                {
                    GiveGene(pawn, effectGeneDef, addAsXenogene);
                }
            }
        }
        public static void GiveHediff(Pawn pawn, HediffDef hediffDef, float hediffSeverity, bool considerBodysize)
        {
            if (!considerBodysize)
            {
                HealthUtility.AdjustSeverity(pawn, hediffDef, hediffSeverity);
            }
            else
            {
                float severity = (1f / pawn.BodySize * hediffSeverity);
                HealthUtility.AdjustSeverity(pawn, hediffDef, severity);
            }
        }
        public static void GiveThought(Pawn pawn, ThoughtDef thoughtDef, float durationDays)
        {
            if (pawn?.needs?.mood != null)
            {
                Thought_Memory existingThought = pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(thoughtDef);
                if (existingThought != null)
                {
                    existingThought.SetForcedStage(Math.Max(0, Math.Min(thoughtDef.stages.Count - 1, existingThought.CurStageIndex + 1)));
                    existingThought.moodPowerFactor = 1f;
                    existingThought.durationTicksOverride = (int)(durationDays * 60000);
                }
                else
                {
                    Thought_Memory newThought = ThoughtMaker.MakeThought(thoughtDef, 1);
                    newThought.moodPowerFactor = 1f;
                    newThought.durationTicksOverride = (int)(durationDays * 60000);
                    pawn.needs.mood.thoughts.memories.TryGainMemory(newThought);
                }
            }
        }
        public static void GiveTrait(Pawn pawn, TraitDef traitDef)
        {
            Trait trait = new Trait(traitDef, 0, false);
            pawn.story.traits.GainTrait(trait);
        }
        public static void ForceTrait(Pawn pawn, TraitDef traitDef)
        {
            Trait trait = new Trait(traitDef, 0, false);
            pawn.story.traits.allTraits.Add(trait);
        }
        public static void GiveGene(Pawn pawn, GeneDef geneDef, bool xenogene)
        {
            if (pawn?.genes != null)
            {
                if (!pawn.genes.GenesListForReading.ConvertAll(gene => gene.def).Contains(geneDef))
                {
                    pawn.genes.AddGene(geneDef, xenogene);
                }
            }
        }
        public static void ExtensionApplierGenetic(Pawn pawn, ThingDef thingDef)
        {
            if (ModLister.GetActiveModWithIdentifier("vegapnk.rjw.genes") == null) return;

            if (thingDef == null) return;
            if (pawn == null) return;
            TransformationAddon Ext = thingDef.GetModExtension<TransformationAddon>();
            if (Ext == null) return;

            GeneDef transformationGene = Ext.transformationGene;
            bool applicableForFemales = Ext.applicableForFemales;
            bool applicableForMales = Ext.applicableForMales;

            if (transformationGene != null)
            {
                if (pawn.gender == Gender.Female && applicableForFemales)
                {
                    RJW_Genes.Patch_Aftersex_ApplyProgressingGeneticTransformations.ApplyTransformation(pawn, transformationGene);
                }
                else if (pawn.gender == Gender.Male && applicableForMales)
                {
                    RJW_Genes.Patch_Aftersex_ApplyProgressingGeneticTransformations.ApplyTransformation(pawn, transformationGene);
                }
            }
        }
    }
}