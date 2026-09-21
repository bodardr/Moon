using System;
using System.Collections.Generic;
using System.Linq;

public interface IStat<out T>
{
    public string Name { get; }
    public event Action<string, T, T> OnValueChanged;
}

public class Stat : IStat<float>
{
    public enum OverrideType
    {
        PreEvaluationOffset,
        AdditiveMultiplier,
        CompoundMultiplier,
        PostEvaluationOffset
    }

    private string name;
    private float baseValue;

    private Dictionary<string, float> preEvaluationOffsets = new();
    private Dictionary<string, float> additiveMultipliers = new();
    private Dictionary<string, float> compoundMultipliers = new();
    private Dictionary<string, float> postEvaluationOffsets = new();

    public string Name => name;
    public float BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            UpdateValue();
        }
    }

    public float EvaluatedValue { get; private set; }
    public event Action<string, float, float> OnValueChanged;

    public Stat(string name, float baseValue)
    {
        this.name = name;
        this.baseValue = baseValue;
        UpdateValue();
    }

    private void UpdateValue()
    {
        var value = baseValue;

        foreach (var (_, val) in preEvaluationOffsets)
            value += val;

        var additiveMultiplier = 1f;
        foreach (var (_, val) in additiveMultipliers)
            additiveMultiplier += val;
        value *= additiveMultiplier;

        foreach (var (_, val) in compoundMultipliers)
            value *= val;

        foreach (var (_, val) in postEvaluationOffsets)
            value += val;

        var oldValue = EvaluatedValue;
        EvaluatedValue = value;

        OnValueChanged?.Invoke(name, oldValue, value);
    }

    public Stat Set(OverrideType type, string overrideKey, float value)
    {
        var collection = type switch
        {
            OverrideType.AdditiveMultiplier => additiveMultipliers,
            OverrideType.CompoundMultiplier => compoundMultipliers,
            OverrideType.PostEvaluationOffset => postEvaluationOffsets,
            OverrideType.PreEvaluationOffset or _ => preEvaluationOffsets,
        };

        if (!collection.TryAdd(overrideKey, value))
            collection[overrideKey] = value;

        UpdateValue();

        return this;
    }

    public Stat Remove(OverrideType type, string overrideKey)
    {
        var collection = type switch
        {
            OverrideType.AdditiveMultiplier => additiveMultipliers,
            OverrideType.CompoundMultiplier => compoundMultipliers,
            OverrideType.PostEvaluationOffset => postEvaluationOffsets,
            OverrideType.PreEvaluationOffset or _ => preEvaluationOffsets,
        };

        collection.Remove(overrideKey);
        UpdateValue();

        return this;
    }

    public void CopyFrom(Stat otherStat)
    {
        name = otherStat.name;
        baseValue = otherStat.baseValue;

        //Lazy copy, might be able to optimize it further if it is a bottleneck.
        preEvaluationOffsets = otherStat.preEvaluationOffsets.ToDictionary(x => x.Key, x => x.Value);
        additiveMultipliers = otherStat.additiveMultipliers.ToDictionary(x => x.Key, x => x.Value);
        compoundMultipliers = otherStat.compoundMultipliers.ToDictionary(x => x.Key, x => x.Value);
        postEvaluationOffsets = otherStat.postEvaluationOffsets.ToDictionary(x => x.Key, x => x.Value);
    }
}
