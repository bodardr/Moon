using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnChildrenUpdatedCallback : UIBehaviour, ICanvasElement
{
    protected override void OnRectTransformDimensionsChange()
    {
        OnContentUpdated?.Invoke();
    }

    private void OnTransformChildrenChanged()
    {
        OnContentUpdated?.Invoke();
    }

    public void Rebuild(CanvasUpdate executing)
    {
        //Nothing to do.
    }

    public void LayoutComplete()
    {
        OnContentUpdated?.Invoke();
    }

    public void GraphicUpdateComplete()
    {
        OnContentUpdated?.Invoke();
    }

    public event Action OnContentUpdated;
}
