using System.Collections.Generic;
public static class GearUtility
{
    public static List<List<Gear>> CreateGearSequence(params Gear[] gears)
    {
        var list = new List<List<Gear>>();
        foreach (var gear in gears)
            list.Add(new List<Gear> { gear });
        return list;
    }

    public static void PlayGearSequence(List<List<Gear>> gears, uint amplitude,
        Dictionary<string, object> additionalParameters = null, params GearTarget[] gearTargets)
    {
        foreach (var gearColumn in gears)
            gearColumn[0].TriggerBehavior?.OnGearTriggered(gearTargets, ref amplitude, gearColumn, additionalParameters);
    }
    public static void PlayGearColumn(List<Gear> gearColumn, uint amplitude, Dictionary<string, object> additionalParameters, params GearTarget[] gearTargets)
    {
        //We skip the first one since it is the trigger from that column.
        for (var i = 1; i < gearColumn.Count; i++)
        {
            var gear = gearColumn[i];
            gear.TriggerBehavior?.OnGearTriggered(gearTargets, ref amplitude, null, additionalParameters);
        }
    }
}
