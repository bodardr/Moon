using System.Collections.Generic;
using UnityEngine;
public class StatusEffectHandler : MonoSingleton<StatusEffectHandler>
{
    public HashSet<GearTarget> Targets { get; } = new();

    private void Update()
    {
        var keysToRemove = new List<IStatusEffect>();
        var delta = Time.deltaTime;
        foreach (var target in Targets)
        {
            keysToRemove.Clear();
            foreach (var (effect, effectInstance) in target.StatusEffects)
            {
                var amplitude = effectInstance.Amplitude;
                var effectLevel = GetEffectLevel(amplitude);
                var newAmplitude = amplitude - effectLevel * delta;
                if (newAmplitude <= 0)
                {
                    keysToRemove.Add(effect);
                    continue;
                }

                effectInstance.Amplitude = newAmplitude;
                var newLevel = GetEffectLevel(newAmplitude);
                if (newLevel != effectLevel)
                    effect.OnEffectLevelChanged(target, newLevel);
                effect.OnEffectTick(target, newLevel);
            }

            foreach (var statusEffect in keysToRemove)
            {
                statusEffect.OnEffectRemoved(target);
                target.StatusEffects.Remove(statusEffect);
            }
        }
    }

    public void ApplyStatusEffect(GearTarget target, IStatusEffect statusEffect, float amplitude)
    {
        if (!target.StatusEffects.TryAdd(statusEffect,
            new StatusEffectInstance { StatusEffect = statusEffect, Amplitude = amplitude }))
            target.StatusEffects[statusEffect].Amplitude += amplitude;
        else
            statusEffect.OnEffectApplied(target, GetEffectLevel(amplitude));
    }

    public static int GetEffectLevel(float amplitude)
    {
        return Mathf.FloorToInt(GetEffectLevelRaw(amplitude));
    }
    public static float GetEffectLevelRaw(float amplitude)
    {
        return Mathf.Max(0,Mathf.Log(amplitude, 3)) + 1;
    }
}
