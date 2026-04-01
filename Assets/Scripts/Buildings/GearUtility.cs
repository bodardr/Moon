using System.Collections.Generic;
public static class GearUtility
{
    public static List<List<Gear>> CreateGearSequence(params Gear[] gears)
    {
        var list = new List<List<Gear>>();
        foreach (var gear in gears)
            list.Add(new List<Gear> {gear});
        return list;
    }
    
    public static void PlayGearSequence(List<List<Gear>> gears, uint amplitude, params GearTarget[] gearTargets)
    {
        foreach (var gear in gears)
            gear[0].TriggerBehavior?.OnGearTriggered(gearTargets, amplitude);
    }
}
