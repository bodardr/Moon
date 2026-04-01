public class StatusEffectInstance
{
    public IStatusEffect StatusEffect { get; set; }
    public float Amplitude { get; set; }
    
    public int Level => StatusEffectHandler.GetEffectLevel(Amplitude);
    public float LevelRatio => StatusEffectHandler.GetEffectLevelRaw(Amplitude) % 1;
}
