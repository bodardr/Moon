using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;
using UnityEngine.Pool;
using Utils;

public class GearsEditor : MonoSingleton<GearsEditor>, IGearDroppable
{
    public static ObjectPool<GearSequenceElement> GearPool;

    private ObjectPool<Transform> sequencePool;
    private ObjectPool<GearColumnBehavior> columnPool;

    private List<GearColumnBehavior> activeColumns = new();
    private List<Transform> activeSequences = new();
    private List<List<List<Gear>>> sequences;

    private BuildingTooltip buildingTooltip;

    [SerializeField] private GameObject gearElement;
    [SerializeField] private GameObject gearColumnElement;
    [SerializeField] private GameObject sequenceElement;

    [SerializeField] private Transform sequenceParent;

    public List<List<List<Gear>>> Sequences
    {
        get => sequences;
        set
        {
            sequences = value;
            Refresh();
        }
    }

    private void Awake()
    {
        sequencePool = ObjectPoolUtility.CreatePoolFast<Transform>(sequenceElement);
        columnPool = ObjectPoolUtility.CreatePoolFast<GearColumnBehavior>(gearColumnElement);
        GearPool ??= ObjectPoolUtility.CreatePoolFast<GearSequenceElement>(gearElement);
        buildingTooltip = GetComponentInParent<BuildingTooltip>();
    }

    private void OnEnable()
    {
        if (buildingTooltip.Current == null)
            return;

        var gearHolder = buildingTooltip.Current.GetComponent<GearHolder>();
        if (gearHolder != null)
            Sequences = gearHolder.Gears;
    }

    private void Refresh()
    {
        foreach (var activeColumn in activeColumns)
            columnPool.Release(activeColumn);
        activeColumns.Clear();

        foreach (var activeSequence in activeSequences)
        {
            activeSequence.DetachChildren();
            sequencePool.Release(activeSequence);
        }
        activeSequences.Clear();

        if (sequences == null)
            return;

        foreach (var sequence in sequences)
        {
            var sequenceTr = sequencePool.Get();
            sequenceTr.SetParent(sequenceParent);
            sequenceTr.transform.localScale = Vector3.one;
            activeSequences.Add(sequenceTr);

            foreach (var gearColumn in sequence)
            {
                var draggableGear = columnPool.Get();
                draggableGear.transform.SetParent(sequenceTr);
                draggableGear.transform.localScale = Vector3.one;
                draggableGear.Sequence = sequence;
                draggableGear.Column = gearColumn;
                activeColumns.Add(draggableGear);
            }
        }
    }
    public void BeginDrag(GearSequenceElement sequenceElement)
    {
        if (sequenceElement.ColumnParent != null)
        {
            sequenceElement.ColumnParent.ActiveGearsInColumn.Remove(sequenceElement);
            var column = sequenceElement.ColumnParent.Column;
            column.Remove(sequenceElement.Gear);

            //The remaining gears that will be removed
            //from the column will be added back to the inventory
            foreach (var gear in column)
                SaveFile.Current.AddGearToInventory(gear);

            if (column.Count == 0)
            {
                sequenceElement.GearSequence.Remove(column);
                activeColumns.Remove(sequenceElement.ColumnParent);
                columnPool.Release(sequenceElement.ColumnParent);
            }
        }

        //We reset the gear column
        sequenceElement.ColumnParent = null;
        sequenceElement.GearSequence = null;

        GearPool.Release(sequenceElement);
    }

    public bool TryDrop(Gear gear, Vector3 position)
    {
        GearSequenceElement closestDraggableInSequence = null;
        var minDistance = Mathf.Infinity;
        var maxUndisplaceableIndex = sequences.ToDictionary(x => x, x => x.FindLastIndex(y => !y[0].Displaceable));

        foreach (var column in activeColumns)
        foreach (var gearInColumn in column.ActiveGearsInColumn)
        {
            var dist = Vector3.Distance(gearInColumn.transform.position, position);
            if (dist >= minDistance)
                continue;

            minDistance = dist;
            closestDraggableInSequence = gearInColumn;
        }

        if (closestDraggableInSequence != null)
        {
            var delta = closestDraggableInSequence.transform.position - position;
            var sequence = closestDraggableInSequence.GearSequence;

            var columnIndex = closestDraggableInSequence.ColumnParent
                .Sequence.IndexOf(closestDraggableInSequence.ColumnParent.Column);
            var rowIndex =
                closestDraggableInSequence.ColumnParent.Column.IndexOf(closestDraggableInSequence.Gear);
            if (rowIndex == 0 && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                //If the closest draggable is a column root, then we have to insert a new column.
                var insertBefore = closestDraggableInSequence.Gear.Displaceable &&
                    position.x < closestDraggableInSequence.transform.position.x;
                sequence.Insert(insertBefore ? columnIndex : columnIndex + 1, new() { gear });
            }
            //If it can be inserted in the expandable gear.
            else if (closestDraggableInSequence.ColumnParent.Column[0].Properties.HasFlag(GearProperties.Expandable))
            {
                var insertBefore = rowIndex > 0 && closestDraggableInSequence.Gear.Displaceable &&
                    position.y > closestDraggableInSequence.transform.position.y;
                closestDraggableInSequence.ColumnParent.Column.Insert(insertBefore ? rowIndex : rowIndex + 1, gear);
            }

        }

        SaveFile.Current.RemoveGear(gear);

        Refresh();
        return true;
    }
}
