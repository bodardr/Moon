using System;
using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

public class TieredUpgrade : IUpgrade
{
    [Serializable]
    public class Tier
    {
        public UpgradeInfo upgradeInfo;
        public int cost;
        public float value;

        public Tier(string name, int cost, float value, string description)
        {
            upgradeInfo = new UpgradeInfo(name, description);
            this.cost = cost;
            this.value = value;
        }
    }

    private readonly Action<Upgrades, float> onUpgradeApplied;
    private readonly List<Tier> tiers;

    private bool requirementsMet;
    
    private int currentTier = -1;

    public UpgradeInfo UpgradeInfo => tiers[TierToDisplay].upgradeInfo;
    public int Cost => tiers[TierToDisplay].cost;

    public int TierToDisplay => Mathf.Clamp(currentTier, 0, tiers.Count - 1);
    public bool CanBuy => Currencies.Active.Credits >= Cost;
    public bool Unlocked => currentTier == tiers.Count;
    public IReadOnlyList<Tier> Tiers => tiers;
    public IUnlockCondition[] UnlockConditions { get; }

    public TieredUpgrade(Action<Upgrades, float> onUpgradeApplied, List<Tier> tiers, params IUnlockCondition[] unlockConditions)
    {
        this.onUpgradeApplied = onUpgradeApplied;
        this.tiers = tiers;
        UnlockConditions = unlockConditions;
    }

    public void Apply(Upgrades upgrades)
    {
        onUpgradeApplied(upgrades, tiers[TierToDisplay].value);
    }

    public void Unlock()
    {
        if (currentTier < tiers.Count)
            currentTier++;
    }
    public void Save()
    {
        if (currentTier < 0)
            return;

        SaveFile.Current.tieredUpgrades[tiers[0].upgradeInfo.name] = currentTier;
    }

    public void Load(Upgrades upgrades)
    {
        if (SaveFile.Current.tieredUpgrades.TryGetValue(tiers[0].upgradeInfo.name, out currentTier))
            Apply(upgrades);
    }
}
