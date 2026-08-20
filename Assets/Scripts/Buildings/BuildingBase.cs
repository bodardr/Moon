using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BuildingBase : MonoBehaviour
{
    protected List<BuildingUpgrade> upgrades = new();
    
    [Header("Building Info")]
    [SerializeField] private string buildingName;

    [SerializeField] private BuildingTabs buildingTabs = BuildingTabs.Upgrades | BuildingTabs.Gears;

    public string BuildingName => buildingName;

    public IReadOnlyList<BuildingUpgrade> Upgrades => upgrades;
    public BuildingTabs Tabs => buildingTabs;

    private void Start()
    {
        InitializeUpgrades();
        
        var saveFile = SaveFile.Current;
        foreach (var upgrade in upgrades)
            if (saveFile.buildingUpgrades.Contains(upgrade.Name))
                upgrade.Unlock(true);
    }
    protected abstract void InitializeUpgrades();
    
    
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
    }
    
    public virtual void OnPointerExit(PointerEventData eventData)
    {
    }
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}