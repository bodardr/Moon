using System.Collections.Generic;
public class Stat
{
    public enum StatOverrideType
    {
        PreEvaluationOffset,
        AdditiveMultiplier,
        CompoundMultiplier,
        PostEvaluationOffset
    }

    private float baseValue;

    private Dictionary<string, float> preEvaluationOffsets = new();
    private Dictionary<string, float> additiveMultipliers = new();
    private Dictionary<string, float> compoundMultipliers = new();
    private Dictionary<string, float> postEvaluationOffsets = new();

    public float EvaluatedValue { get; private set; }

    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        EvaluateValue();
    }

    private void EvaluateValue()
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

        EvaluatedValue = value;
    }

    public Stat Add(StatOverrideType type, string overrideKey, float value)
    {
        var collection = type switch
        {
            StatOverrideType.AdditiveMultiplier => additiveMultipliers,
            StatOverrideType.CompoundMultiplier => compoundMultipliers,
            StatOverrideType.PostEvaluationOffset => postEvaluationOffsets,
            StatOverrideType.PreEvaluationOffset or _ => preEvaluationOffsets,
        };

        collection.TryAdd(overrideKey, value);

        return this;
    }

    public Stat Modify(StatOverrideType type, string overrideKey, float value)
    {
        var collection = type switch
        {
            StatOverrideType.AdditiveMultiplier => additiveMultipliers,
            StatOverrideType.CompoundMultiplier => compoundMultipliers,
            StatOverrideType.PostEvaluationOffset => postEvaluationOffsets,
            StatOverrideType.PreEvaluationOffset or _ => preEvaluationOffsets,
        };

        if (collection.ContainsKey(overrideKey))
            collection[overrideKey] = value;

        return this;
    }

    public Stat Remove(StatOverrideType type, string overrideKey)
    {
        var collection = type switch
        {
            StatOverrideType.AdditiveMultiplier => additiveMultipliers,
            StatOverrideType.CompoundMultiplier => compoundMultipliers,
            StatOverrideType.PostEvaluationOffset => postEvaluationOffsets,
            StatOverrideType.PreEvaluationOffset or _ => preEvaluationOffsets,
        };

        collection.Remove(overrideKey);
        return this;
    }
}