using UnityEngine;

public interface IStatusEffect
{
    public Sprite Sprite { get; }

    public void OnEffectApplied(GearTarget target, int effectLevel);
    public void OnEffectLevelChanged(GearTarget target, int newLevel);
    public void OnEffectTick(GearTarget target, int effectLevel);
    public void OnEffectRemoved(GearTarget target);
}
