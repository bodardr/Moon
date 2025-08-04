using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private List<BuildingUpgrade> upgrades = new();

    [SerializeField] private Vector2 tooltipOffset;

    public IReadOnlyList<BuildingUpgrade> Upgrades => upgrades;

    protected virtual void Start()
    {
        var saveFile = SaveFile.Current;
        foreach (var upgrade in upgrades)
            if (saveFile.buildingUpgrades.Contains(upgrade.Name))
                upgrade.Activate(true);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        BuildingTooltip.Instance.ShowFromBuilding(this, tooltipOffset);
        throw new System.NotImplementedException();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        BuildingTooltip.Instance.Hide();
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }
}
