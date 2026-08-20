using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class StatusEffectBehavior : IGearBehavior
{
    [SerializeReference] private IStatusEffect statusEffect;
    [SerializeField] private float amplitudeScaling = 1;

    public void OnGearTriggered(GearTarget[] targets, ref uint amplitude, List<Gear> gearColumn,
        Dictionary<string, object> additionalParameters)
    {
        foreach (var target in targets)
            StatusEffectHandler.Instance.ApplyStatusEffect(target, statusEffect, amplitudeScaling);
    }
}