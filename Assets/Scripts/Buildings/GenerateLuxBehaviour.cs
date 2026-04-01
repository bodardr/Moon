using Save;
public class GenerateLuxBehaviour : IGearBehavior
{
    public void OnGearTriggered(GearTarget[] targets, uint amplitude)
    {
        foreach (var target in targets)
            SaveFile.Current.Lux.Amount += amplitude;
    }
}
