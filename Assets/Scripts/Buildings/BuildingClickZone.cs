using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingClickZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] private BuildingBase buildingBase;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        buildingBase.OnPointerDown(eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        buildingBase.OnPointerUp(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        buildingBase.OnPointerClick(eventData);
    }
}