using System.Collections.Generic;
using UnityEngine;
public class GearColumnBehavior : MonoBehaviour
{
    private readonly List<GearSequenceElement> activeGearsInColumn = new();
    private List<Gear> column;

    public List<Gear> Column
    {
        get => column;
        set
        {
            column = value;
            RefreshColumn();
        }
    }

    public GearsEditor GearsEditor { get; set; }

    public List<GearSequenceElement> ActiveGearsInColumn => activeGearsInColumn;
    public List<List<Gear>> Sequence { get; set; }

    private void RefreshColumn()
    {
        var gearPool = GearsEditor.GearPool;
        foreach (var activeGear in activeGearsInColumn)
            gearPool.Release(activeGear);
        activeGearsInColumn.Clear();

        if (Column == null)
            return;

        foreach (var gear in Column)
        {
            var draggableGear = gearPool.Get();
            draggableGear.transform.SetParent(transform);
            draggableGear.transform.SetAsLastSibling();
            draggableGear.transform.localScale = Vector3.one;
            draggableGear.Gear = gear;
            draggableGear.ColumnParent = this;
            draggableGear.GearSequence = Sequence;
            activeGearsInColumn.Add(draggableGear);
        }
    }

    private void OnDisable()
    {
        var gearPool = GearsEditor.GearPool;
        foreach (var activeGear in activeGearsInColumn)
            gearPool.Release(activeGear);
        activeGearsInColumn.Clear();
    }
}
