using System;
using System.Collections.Generic;
using Save;

[Serializable]
public class GenerateLuxBehavior : IGearBehavior
{
    public void OnGearTriggered(GearTarget[] targets, ref uint amplitude, List<Gear> column,
        Dictionary<string, object> additionalParameters)
    {
        foreach (var target in targets)
            SaveFile.Current.Lux.Amount += amplitude;
    }
}
