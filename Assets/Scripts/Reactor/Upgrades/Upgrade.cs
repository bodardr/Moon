using System;
using System.Linq;
using Save;
using UnityEngine;

[Serializable]
public class Upgrade : IUpgrade
{
    private readonly int cost;
    private readonly Action<Upgrades> onUpgradeApplied;
    private readonly UpgradeInfo upgradeInfo;

    [SerializeField] private bool unlocked;

    public UpgradeInfo UpgradeInfo => upgradeInfo;
    public int Cost => cost;
    public bool Unlocked => unlocked;
    public IUnlockCondition[] UnlockConditions { get; }

    public Upgrade(UpgradeInfo upgradeInfo, int cost, Action<Upgrades> onUpgradeApplied,
        params IUnlockCondition[] unlockConditions)
    {
        this.upgradeInfo = upgradeInfo;
        this.cost = cost;
        this.onUpgradeApplied = onUpgradeApplied;
        UnlockConditions = unlockConditions;
    }

    public void Apply(Upgrades upgrades)
    {
        onUpgradeApplied(upgrades);
    }

    public void Unlock()
    {
        unlocked = true;
    }

    public void Save()
    {
        SaveFile.Current.upgrades.Add(upgradeInfo.name);
    }
    public void Load(Upgrades upgrades)
    {
        unlocked = SaveFile.Current.upgrades.Contains(upgradeInfo.name);
        if (unlocked)
            Apply(upgrades);
    }
}
