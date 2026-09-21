using System.Collections.Generic;
using UnityEngine;
public class ToolUpgrades
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        Upgrades.AddNewTab("Tools", GetUpgrades());
    }

    private static List<IUpgrade> GetUpgrades()
    {
        var list = new List<IUpgrade>();

        //THRUSTER
        list.Add(new Upgrade(new("Unlock Thruster",
                "We've devised a new tool to propel the particle further after it's been launched. You'll just have to press 'SPACE' and the thruster will take care of the rest"),
            35, upgrades => upgrades.ThrusterUnlockable.Unlock()));

        list.Add(new TieredUpgrade((upgrades, value) =>
                upgrades.ThrusterForce.Set(Stat.OverrideType.PreEvaluationOffset, "Thruster Force Upgrade", value),
            new()
            {
                new("Thruster Force I", 45, 10,
                    "We've added an identical second thruster, which should improve overall thruster strength."),
                new("Thruster Force II", 90, 18,
                    "A reviewed the thruster's designs was overdue. We've swapped the thruster shape for a parabolic one, making it thrust more efficiently forward."),
                new("Thruster Force III", 90, 32,
                    "The thruster's fuel mixture has been adjusted in order to create more impact."),
            }, new UnlockCondition(Upgrades.ActiveUpgrades.ThrusterUnlockable)
        ));

        list.Add(new TieredUpgrade((upgrades, value) =>
                upgrades.ThrusterFuel.Set(Stat.OverrideType.PreEvaluationOffset, "Thruster Fuel Upgrade", value),
            new()
            {
                new("Thruster Fuel I", 45, 1.5f,
                    "We've added an identical second thruster, which should improve overall thruster strength."),
                new("Thruster Fuel II", 90, 3, "This adds a second fuel tank."),
                new("Thruster Fuel III", 90, 5.5f,
                    "The main fuel compound has been swapped for a lighter gas, allowing equal propulsion for less fuel."),
            }, new UnlockCondition(Upgrades.ActiveUpgrades.ThrusterUnlockable)
        ));

        //REDIRECT
        list.Add(new Upgrade(
            new UpgradeInfo("Unlock Redirect Tech",
                "This is the result of a new research being developed which redirects kinetic energy, enabling further acceleration."),
            120, upgrades => upgrades.RedirectUnlockable.Unlock()));
        
        

        return list;
    }
}
