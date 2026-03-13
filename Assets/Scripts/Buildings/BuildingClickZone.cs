using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingClickZone : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2 tooltipOffset;
    [SerializeField] private Vector2 tooltipPivot = new Vector2(0.5f, 0);

    [SerializeField] private BuildingBase building;
    
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        BuildingTooltip.Instance.ShowFromBuilding(building, tooltipPivot, tooltipOffset);
        building.OnPointerEnter(eventData);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        BuildingTooltip.Instance.Hide();
        building.OnPointerExit(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        building.OnPointerClick(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        building.OnPointerDown(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        building.OnPointerUp(eventData);
    }
}
