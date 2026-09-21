using System.Collections.Generic;
using UnityEngine;
public class LauncherUpgrades
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        Upgrades.AddNewTab("Launcher", GetUpgrades());
    }

    private static List<IUpgrade> GetUpgrades()
    {
        var list = new List<IUpgrade>();

        list.Add(new TieredUpgrade((upgrades, val) =>
                upgrades.LauncherSpeed.Set(Stat.OverrideType.PreEvaluationOffset, "Launcher Speed Upgrade", val),
            new List<TieredUpgrade.Tier>
            {
                new TieredUpgrade.Tier("Magnetic Launcher", 1, 2f,
                    "With a simple ferromagnetic slate placed close to the particle, we can easily double the launch speed!"),
                new TieredUpgrade.Tier("Electromagnetic Launcher", 7, 5f,
                    "Now that we have a magnet in place, let's add a small voltage to it to increase the force's amplitude."),
            }));
        
        return list;
    }
}
