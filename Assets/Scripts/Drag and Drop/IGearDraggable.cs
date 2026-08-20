using UnityEngine;
public interface IGearDraggable
{
    public bool CanDrag { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Returns true if the drag has been accepted.</returns>
    public Gear BeginDrag();

    /// <summary>
    /// Called whenever the drag is dropped without a valid target.
    /// </summary>
    public void OnDragCancelled();
}

public interface IGearDroppable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="gear"></param>
    /// <param name="position"></param>
    /// <returns>Returns true if the drop has been accepted.</returns>
    public bool TryDrop(Gear gear, Vector3 position);
}