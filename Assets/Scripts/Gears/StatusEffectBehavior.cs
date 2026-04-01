using System;
using UnityEngine;
[Serializable]
public class StatusEffectBehavior : IGearBehavior
{
    [SerializeReference] private IStatusEffect statusEffect;
    [SerializeField] private int level = 1;

    public void OnGearTriggered(GearTarget[] targets, uint amplitude)
    {
        foreach (var target in targets)
            StatusEffectHandler.Instance.ApplyStatusEffect(target, statusEffect, amplitude);
    }
}