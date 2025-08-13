using Save;
using UnityEngine.EventSystems;
public class ForestBuilding : BuildingBase
{
    private float collectionRate = 1;
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        SaveFile.Current.wood += (uint)collectionRate;
    }
}
