using System.Collections.Generic;
using UnityEngine;

public class GearTarget : MonoBehaviour
{
    public GearStats Stats { get; } = new();

    public Dictionary<IStatusEffect, StatusEffectInstance> StatusEffects { get; } = new();

    private void OnEnable()
    {
        StatusEffectHandler.Instance.Targets.Add(this);
    }

    private void OnDisable()
    {
        if (StatusEffectHandler.Instance != null)
            StatusEffectHandler.Instance.Targets.Remove(this);
    }
}