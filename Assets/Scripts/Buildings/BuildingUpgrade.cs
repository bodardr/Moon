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
            var saveFile = SaveFile.Current;
            foreach (var cost in Costs)
            {
                var amount = cost.Resource switch
                {
                    Resource.Wood => saveFile.wood,
                    Resource.Stone => saveFile.stone,
                    Resource.Iron => saveFile.iron,
                    Resource.Lunarite => saveFile.lunarite,
                    Resource.Gold => saveFile.gold,
                    _ => throw new NotImplementedException()
                };

                if (amount < cost.Amount)
                    return false;
            }

            return true;
        }
    }

    public void Activate(bool fromSave = false)
    {
        IsUnlocked = true;
        OnUpgrade?.Invoke();

        if (fromSave)
            SaveFile.Current.buildingUpgrades.Add(Name);
    }
}

public struct ResourceWithAmount
{
    public Resource Resource;
    public uint Amount;
}
