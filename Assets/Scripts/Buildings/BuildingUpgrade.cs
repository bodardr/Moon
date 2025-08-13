using System;
using System.Collections.Generic;
using Save;

public class BuildingUpgrade
{
    public string Name;
    public bool IsUnlocked;
    public ResourceWithAmount[] Costs;
    public List<BuildingUpgrade> Prerequisites;
    public Action OnUpgrade;

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

    public BuildingUpgrade(string name, Action onUpgrade, params ResourceWithAmount[] costs)
    {
        Name = name;
        Costs = costs;
        OnUpgrade = onUpgrade;
    }

    public BuildingUpgrade(string name, Action onUpgrade, List<BuildingUpgrade> prerequisites,
        params ResourceWithAmount[] costs) : this(name, onUpgrade, costs)
    {
        Prerequisites = prerequisites;
    }

    public void Activate(bool fromLoad = false)
    {
        IsUnlocked = true;
        OnUpgrade?.Invoke();

        if (fromLoad)
            return;

        SaveFile.Current.buildingUpgrades.Add(Name);
    }

    public void SubtractCosts()
    {
        if (!CanAfford)
            return;

        var saveFile = SaveFile.Current;
        foreach (var cost in Costs)
        {
            switch (cost.Resource)
            {
                case Resource.Wood:
                    saveFile.wood -= cost.Amount;
                    break;
                case Resource.Stone:
                    saveFile.stone -= cost.Amount;
                    break;
                case Resource.Iron:
                    saveFile.iron -= cost.Amount;
                    break;
                case Resource.Lunarite:
                    saveFile.lunarite -= cost.Amount;
                    break;
                case Resource.Gold:
                    saveFile.gold -= cost.Amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

public class ResourceWithAmount
{
    public Resource Resource;
    public uint Amount;

    public bool CanAfford
    {
        get
        {
            var saveFile = SaveFile.Current;
            uint resource = Resource switch
            {
                Resource.Wood => saveFile.wood,
                Resource.Stone => saveFile.stone,
                Resource.Iron => saveFile.iron,
                Resource.Lunarite => saveFile.lunarite,
                Resource.Gold => saveFile.gold,
                _ => throw new NotImplementedException()
            };
            return resource >= Amount;
        }
    }

    public ResourceWithAmount(Resource resource, uint amount)
    {
        Resource = resource;
        Amount = amount;
    }
}
