using System.Linq;
public static class UpgradeUtility
{
    public static bool AreRequirementsMet(IUpgrade upgrade)
    {
        return upgrade.UnlockConditions.All(x => x.IsUnlocked());
    }
}
