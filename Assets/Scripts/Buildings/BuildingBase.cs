using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingBase : MonoBehaviour
{
    protected List<BuildingUpgrade> upgrades = new();
    
    [Header("Building Info")]
    [SerializeField] private string buildingName;

    public string BuildingName => buildingName;

    public IReadOnlyList<BuildingUpgrade> Upgrades => upgrades;

    private void Start()
    {
        InitializeUpgrades();
        
        var saveFile = SaveFile.Current;
        foreach (var upgrade in upgrades)
            if (saveFile.buildingUpgrades.Contains(upgrade.Name))
                upgrade.Unlock(true);
    }
    protected virtual void InitializeUpgrades()
    {
    }
    
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
