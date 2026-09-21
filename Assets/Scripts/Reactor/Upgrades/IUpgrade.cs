public interface IUpgrade
{
    public UpgradeInfo UpgradeInfo { get; }
    public int Cost { get; }
    bool Unlocked { get; }
    public void Apply(Upgrades upgrades);
    void Unlock();
    void Save();
    void Load(Upgrades upgrades);
    public IUnlockCondition[] UnlockConditions { get; }
}