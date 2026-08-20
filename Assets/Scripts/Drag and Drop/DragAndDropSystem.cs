using System.Collections.Generic;
using Bodardr.Databinding.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragAndDropSystem : MonoSingleton<DragAndDropSystem>
{
    private bool isDragging = false;

    private RectTransform draggedObject;
    private BindingNode draggedObjectNode;
    private Gear draggedGear;

    private IGearDraggable dragOrigin;

    private List<RaycastResult> raycastResults = new();

    [SerializeField] private GameObject gearDraggablePrefab;

    private void Awake()
    {
        draggedObject = (RectTransform)Instantiate(gearDraggablePrefab, Vector3.zero, Quaternion.identity).transform;
        draggedObjectNode = draggedObject.GetComponent<BindingNode>();
        
        draggedObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckDrag();
        }
        else if (isDragging)
        {
            UpdateDrag();
            if (Mouse.current.leftButton.wasReleasedThisFrame)
                CheckDrop();
        }
    }

    private void UpdateDrag()
    {
        var mousePos = Mouse.current.position.value;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(draggedObject, mousePos, null,
            out var rectPos);
        draggedObject.position = rectPos;
    }

    private void CheckDrag()
    {
        UpdateRaycast();

        foreach (var result in raycastResults)
        {
            if (!result.gameObject.TryGetComponent<IGearDraggable>(out var draggable) || !draggable.CanDrag)
                continue;

            draggedGear = draggable.BeginDrag();
            dragOrigin = draggable;
            
            draggedObject.SetParent(result.gameObject.transform.root);
            draggedObject.SetAsLastSibling();
            draggedObjectNode.Binding = draggedGear;
            draggedObject.gameObject.SetActive(true);
            UpdateDrag();

            isDragging = true;
            
            break;
        }
    }

    private void CheckDrop()
    {
        UpdateRaycast();

        var dropSuccessful = false;
        foreach (var result in raycastResults)
        {
            if (!result.gameObject.TryGetComponent<IGearDroppable>(out var droppable) ||
                !droppable.TryDrop(draggedGear, draggedObject.transform.position))
                continue;

            dropSuccessful = true;
            break;
        }

        if (!dropSuccessful)
            dragOrigin?.OnDragCancelled();

        draggedObject.gameObject.SetActive(false);
        draggedObjectNode.Binding = null;
        
        draggedGear = null;
        dragOrigin = null;

        isDragging = false;
    }

    private void UpdateRaycast()
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.value;
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
    }
}
