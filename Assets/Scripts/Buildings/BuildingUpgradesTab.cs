using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Save;
using UnityEngine;
public class BuildingUpgradesTab : MonoBehaviour, ICollectionCallback, INotifyPropertyChanged
{
    private List<BuildingUpgrade> upgradesToShow;
    private BuildingBase current;

    public List<BuildingUpgrade> UpgradesToShow
    {
        get => upgradesToShow;
        set
        {
            upgradesToShow = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradesToShow)));
        }
    }

    public BuildingBase Current
    {
        get => current;
        set
        {
            current = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
            UpdateUpgrades();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    

    private void UpdateUpgrades()
    {
        UpgradesToShow = current?.Upgrades.Where(x => x.ShouldShow)
            .ToList();
    }

    public void OnItemClicked(int index)
    {
        var upgrade = UpgradesToShow[index];
        if (upgrade.IsUnlocked || !upgrade.CanAfford)
            return;

        //Subtract costs
        var saveFile = SaveFile.Current;
        foreach (var cost in upgrade.Costs)
            saveFile[cost.ResourceType].Amount -= cost.Amount;

        upgrade.Unlock();
        UpdateUpgrades();
    }
}
