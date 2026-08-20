using System.Collections.Generic;
public interface IGearBehavior
{
    public void OnGearTriggered(GearTarget[] targets, ref uint amplitude, List<Gear> gearColumn, Dictionary<string, object> additionalParameters);
}