using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingBase : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    protected List<BuildingUpgrade> upgrades = new();

    [SerializeField] private Vector2 tooltipOffset;
    [SerializeField] private string buildingName;

    public string BuildingName => buildingName;

    public IReadOnlyList<BuildingUpgrade> Upgrades => upgrades;

    protected virtual void Start()
    {
        var saveFile = SaveFile.Current;
        foreach (var upgrade in upgrades)
            if (saveFile.buildingUpgrades.Contains(upgrade.Name))
                upgrade.Activate(true);
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {

    }
    
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        BuildingTooltip.Instance.ShowFromBuilding(this, tooltipOffset);
    }
    
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        BuildingTooltip.Instance.Hide();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
    }
}
