
    using UnityEngine;
    public class Haste : IStatusEffect
    {
        public const string HASTE_MODIFIER = "Haste";
        public const float HASTE_MULTIPLIER_PER_LEVEL = 2f;

        public Sprite Sprite => Resources.Load<Sprite>("icon_haste");
        
        public void OnEffectApplied(GearTarget target, int effectLevel)
        {
            target.Stats.SpeedRate.Add(Stat.StatOverrideType.AdditiveMultiplier, HASTE_MODIFIER, effectLevel * HASTE_MULTIPLIER_PER_LEVEL);
        }

        public void OnEffectLevelChanged(GearTarget target, int newLevel)
        {
            target.Stats.SpeedRate.Modify(Stat.StatOverrideType.AdditiveMultiplier, HASTE_MODIFIER, newLevel * HASTE_MULTIPLIER_PER_LEVEL);
        }
        
        public void OnEffectTick(GearTarget target, int effectLevel)
        {
            //Nothing to do!
        }

        public void OnEffectRemoved(GearTarget target)
        {
            target.Stats.SpeedRate.Remove(Stat.StatOverrideType.AdditiveMultiplier, HASTE_MODIFIER);
        }
    }

