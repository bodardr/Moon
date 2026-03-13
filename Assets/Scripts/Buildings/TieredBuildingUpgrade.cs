using System;
public class TieredBuildingUpgrade : IBuildingUpgrade
{
    private Action<int> onUpgrade;
    
    public TieredBuildingUpgrade(string name, Action<int> onUpgrade, bool showAfterMaxed = false, params ResourceWithAmount[] costs)
    {
    }
    public string Name { get; }
    public ResourceWithAmount[] Costs { get; }
    public bool ShouldShow { get; }
    public bool IsUnlocked { get; }
}
