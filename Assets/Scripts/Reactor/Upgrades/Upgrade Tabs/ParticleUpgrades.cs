using System.Collections.Generic;
using UnityEngine;
public class ParticleUpgrades
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        Upgrades.AddNewTab("Particle", GetUpgrades());
    }

    private static List<IUpgrade> GetUpgrades()
    {
        var list = new List<IUpgrade>();

        list.Add(new TieredUpgrade((upgrades, val) =>
                upgrades.ParticleMass.Set(Stat.OverrideType.PreEvaluationOffset, "Particle Mass Upgrade", val),
            new List<TieredUpgrade.Tier>
            {
                new TieredUpgrade.Tier("Mass Upgrade I", 1, 1.5f,
                    "With a stronger polymer shell, our particle will be able to withstand stronger collisions."),
                new TieredUpgrade.Tier("Mass Upgrade II", 7, 5f,
                    "We are ditching plastic for a metallic structure. We're starting off with a copper shell to maintain malleability"),
            }));

        list.Add(new TieredUpgrade((upgrades, val) =>
                upgrades.ParticleSize.Set(Stat.OverrideType.PreEvaluationOffset, "Particle Size Upgrade", val),
            new List<TieredUpgrade.Tier>
            {
                new TieredUpgrade.Tier("Size Upgrade I", 3, 0.05f,
                    "We've finally convinced Jimmy from nanotech to stop cramming everything into the smallest possible particle."),
                new TieredUpgrade.Tier("Size Upgrade II", 11, 0.15f,
                    "We believe there's more room for a bigger particle. Jimmy agreed once we jiggled his employment papers next to him."),
            }));

        list.Add(new Upgrade(new UpgradeInfo("Collision booster", "Boosts velocity on impact with other particles."),
            10,
            upgrades => upgrades.CollisionBoost.Set(Stat.OverrideType.PreEvaluationOffset,
                "Collision Boost Upgrade", 5f)));

        return list;
    }
}
