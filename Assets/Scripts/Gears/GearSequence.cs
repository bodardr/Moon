using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using Utils;

public class GearSequence : MonoBehaviour
{
    public static ObjectPool<GearDraggableBehavior> GearPool;
    private List<GearDraggableBehavior> activeGears = new();
    private List<List<Gear>> sequence;

    private GearDraggableBehavior activeDraggable = null;
    private BuildingTooltip buildingTooltip;
    //private GearDraggableBehavior draggablePreview = null; todo later

    [SerializeField] private GameObject gearElement;

    public List<List<Gear>> Sequence
    {
        get => sequence;
        set
        {
            sequence = value;
            Refresh();
        }
    }

    private void Awake()
    {
        GearPool ??= ObjectPoolUtility.CreatePoolFast<GearDraggableBehavior>(gearElement);
        buildingTooltip = GetComponentInParent<BuildingTooltip>();
    }

    private void OnEnable()
    {
        if (buildingTooltip.Current != null && buildingTooltip.Current is IGearHolder gearHolder)
            Sequence = gearHolder.Gears;
    }

    private void Refresh()
    {
        transform.DetachChildren();
        foreach (var gear in activeGears)
            if (gear.transform.IsChildOf(transform))
                GearPool.Release(gear);
        activeGears.Clear();

        var maxDepth = 1f;
        foreach (var gearColumn in sequence)
        {
            var draggableGear = GearPool.Get();
            draggableGear.transform.SetParent(transform);
            draggableGear.transform.localScale = Vector3.one;
            draggableGear.GearColumn = gearColumn;
            draggableGear.LayoutElement.ignoreLayout = false;
            draggableGear.GearSequence = this;
            activeGears.Add(draggableGear);
            maxDepth = Mathf.Max(maxDepth, gearColumn.Count);
        }
    }
    public void BeginDrag(GearDraggableBehavior draggableBehavior)
    {
        //Since this is a 2D grid of gears, we either remove the column entirely...
        if (draggableBehavior.GearColumn != null)
            sequence.Remove(draggableBehavior.GearColumn);
        //Or, we remove this gear from the column it came from.
        else if (draggableBehavior.ColumnParent != null)
            draggableBehavior.ColumnParent.GearColumn.Remove(draggableBehavior.Gear);

        //Here if it was the root of a column, we convert it to a solo gear.
        if (draggableBehavior.GearColumn != null)
            draggableBehavior.Gear = draggableBehavior.GearColumn[0];

        //We reset the gear column
        draggableBehavior.GearColumn = null;
        draggableBehavior.LayoutElement.ignoreLayout = true;

        activeDraggable = draggableBehavior;
        activeGears.Remove(activeDraggable);
        activeDraggable.transform.SetParent(transform.root);
    }

    public void EndDrag(GearDraggableBehavior draggableBehavior)
    {
        if (draggableBehavior != activeDraggable)
            return;

        var draggablePos = activeDraggable.transform.position;

        GearDraggableBehavior closestDraggableInSequence = null;
        var minDistance = Mathf.Infinity;
        var maxUndisplaceableIndex = activeGears.FindLastIndex(x => !x.Gear.Displaceable);
        for (var i = 0; i < activeGears.Count; i++)
        {
            var gear = activeGears[i];
            if (gear == activeDraggable ||
                //If this gear is not displaceable but it's not the last undisplaceable one
                !gear.Gear.Displaceable && i < maxUndisplaceableIndex)
                continue;

            var dist = Vector3.Distance(gear.transform.position, draggablePos);
            if (dist >= minDistance)
                continue;

            minDistance = dist;
            closestDraggableInSequence = gear;
        }

        if (closestDraggableInSequence != null)
        {
            //If the closest draggable is a column root, then we have to insert a new column.
            if (closestDraggableInSequence.ColumnParent == null)
            {
                var insertBefore = closestDraggableInSequence.Gear.Displaceable && draggablePos.x < closestDraggableInSequence.transform.position.x;
                var index = sequence.IndexOf(closestDraggableInSequence.GearColumn);
                sequence.Insert(insertBefore ? index : index + 1, new() { activeDraggable.Gear });
            }
            //Else, we insert the new gear inside the existing column (think a row).
            else
            {
                var insertBefore = draggablePos.y > closestDraggableInSequence.transform.position.y;
                var index = closestDraggableInSequence.GearColumn.IndexOf(closestDraggableInSequence.Gear);
                closestDraggableInSequence.GearColumn.Insert(insertBefore ? index : index + 1, activeDraggable.Gear);
            }
        }
        else
        {
            //todo : if (Rect.PointToNormalized((RectTransform)transform.parent))
            sequence.Add(new List<Gear> { activeDraggable.Gear });
        }

        GearPool.Release(activeDraggable);
        activeDraggable = null;
        Refresh();
    }

    private void Update()
    {
        if (activeDraggable == null)
            return;

        UpdateDrag();
    }

    private void UpdateDrag()
    {
        var mousePos = Mouse.current.position.value;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(activeDraggable.RectTransform, mousePos, null,
            out var rectPos);
        activeDraggable.RectTransform.position = rectPos;
    }
}
