using System.Collections.Generic;
using System.ComponentModel;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearSequenceElement : MonoBehaviour, IGearDraggable, INotifyPropertyChanged
{
    private Gear gear;
    private GearColumnBehavior columnParent;
    private bool isDragging;

    public Gear Gear
    {
        get => gear;
        set
        {
            gear = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gear)));
        }
    }

    public GearColumnBehavior ColumnParent
    {
        get => columnParent;
        set => columnParent = value;
    }

    public List<List<Gear>> GearSequence { get; set; }
    public RectTransform RectTransform { get; set; }

    public bool CanDrag => Gear.Displaceable;
    
    public event PropertyChangedEventHandler PropertyChanged;

    private void Awake()
    {
        RectTransform = (RectTransform)transform;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Gear.Displaceable)
            return;

        isDragging = true;
        GearsEditor.Instance.BeginDrag(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging)
            return;
        
        //todo : open tooltip here
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging)
            return;
        
        //todo : close tooltip here
    }

    public Gear BeginDrag()
    {
        GearsEditor.Instance.BeginDrag(this);
        return gear;
    }

    public void OnDragCancelled()
    {
        SaveFile.Current.AddGearToInventory(gear);
    }
}
