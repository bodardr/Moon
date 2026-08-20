using System;
using System.Collections.Generic;
using System.Linq;
using Save;

public class BuildingUpgrade : IBuildingUpgrade
{
    private string name;
    private bool isUnlocked;
    private ResourceWithAmount[] costs;

    public string Name => name;
    public bool IsUnlocked => isUnlocked;
    public ResourceWithAmount[] Costs => costs;
    public List<BuildingUpgrade> Prerequisites;
    public bool ShowAfterUnlock;
    private Action<bool> onUpgrade;

    public bool CanAfford
    {
        get
        {
            foreach (var cost in Costs)
                if (!cost.CanAfford)
                    return false;

            return true;
        }
    }

    public bool ShouldShow => (Prerequisites == null || Prerequisites.Count == 0 ||
            Prerequisites.All(y => SaveFile.Current.buildingUpgrades.Contains(y.Name)))
        && (!IsUnlocked || ShowAfterUnlock);

    public BuildingUpgrade(string name, Action<bool> onUpgrade, bool showAfterUnlock = false, params ResourceWithAmount[] costs)
    {
        this.name = name;
        this.costs = costs;
        this.onUpgrade = onUpgrade;
        ShowAfterUnlock = showAfterUnlock;
    }

    public BuildingUpgrade(string name, Action<bool> onUpgrade, List<BuildingUpgrade> prerequisites,
        bool showAfterUnlock = false, params ResourceWithAmount[] costs) : this(name, onUpgrade, showAfterUnlock, costs)
    {
        Prerequisites = prerequisites;
    }

    public BuildingUpgrade(string name, Action<bool> onUpgrade, BuildingUpgrade prerequisite,
        bool showAfterUnlock = false, params ResourceWithAmount[] costs) : this(name, onUpgrade, showAfterUnlock, costs)
    {
        Prerequisites = new List<BuildingUpgrade> { prerequisite };
    }

    public void Unlock(bool fromLoad = false)
    {
        isUnlocked = true;
        onUpgrade?.Invoke(fromLoad);

        if (fromLoad)
            return;

        SaveFile.Current.buildingUpgrades.Add(Name);
    }
}