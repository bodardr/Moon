using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GearDraggableBehavior : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler,
    IPointerExitHandler, INotifyPropertyChanged
{
    private RectTransform rectTransform;
    private LayoutElement layoutElement;

    private Gear gear;
    private List<Gear> gearColumn;
    private GearDraggableBehavior columnParent;
    private bool isDragging;

    private List<GearDraggableBehavior> activeGearsInColumn = new();

    [SerializeField] private Transform gearColumnParent;

    public Gear Gear
    {
        get => GearColumn?[0] ?? gear;
        set
        {
            gear = value;
            if (gear != null)
                gearColumn = null;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gear)));
        }
    }

    public List<Gear> GearColumn
    {
        get => gearColumn;
        set
        {
            gearColumn = value;
            if (gearColumn != null)
                gear = null;
            RefreshColumn();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gear)));
        }
    }

    public GearDraggableBehavior ColumnParent => columnParent;

    public RectTransform RectTransform => rectTransform;
    public LayoutElement LayoutElement => layoutElement;

    public GearSequence GearSequence { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();
        rectTransform = (RectTransform)transform;
    }

    private void Update()
    {
        if (!isDragging || !Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        isDragging = false;
        GearSequence.EndDrag(this);
    }

    private void RefreshColumn()
    {
        var gearPool = GearSequence.GearPool;
        foreach (var activeGear in activeGearsInColumn)
            gearPool.Release(activeGear);
        activeGearsInColumn.Clear();

        if (GearColumn == null)
            return;

        for (int i = 1; i < GearColumn.Count; i++)
        {
            var draggableGear = gearPool.Get();
            draggableGear.transform.SetParent(gearColumnParent);
            draggableGear.transform.localScale = Vector3.one;
            draggableGear.Gear = GearColumn[i];
            draggableGear.columnParent = this;
            draggableGear.LayoutElement.ignoreLayout = false;
            draggableGear.GearSequence = GearSequence;
            activeGearsInColumn.Add(draggableGear);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!Gear.Displaceable)
            return;

        isDragging = true;
        GearSequence.BeginDrag(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging)
            return;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging)
            return;
    }
}
