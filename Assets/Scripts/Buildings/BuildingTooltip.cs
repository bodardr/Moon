using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingTooltip : MonoSingleton<BuildingTooltip>, INotifyPropertyChanged, IPointerMoveHandler
{
    private bool pointerMoving;
    private bool isPointedAt = false;
    private bool isHeld;
    private List<RaycastResult> raycastResults = new();

    private BuildingBase current;
    private RectTransform rectTransform;

    public bool IsHeld
    {
        get => isHeld;
        set
        {
            isHeld = value;

            if (value)
                UpdateShow();
        }
    }
    public bool IsPointedAt
    {
        get => isPointedAt;
        set
        {
            isPointedAt = value;
            UpdateShow();
        }
    }

    public BuildingBase Current
    {
        get => current;
        set
        {
            current = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Current)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void ShowFromBuilding(BuildingBase building, Vector2 pivot, Vector2 offset)
    {
        Current = building;

        if (rectTransform == null)
            rectTransform = (RectTransform)transform;

        rectTransform.pivot = pivot;
        transform.position =
            (Vector2)PixelateCamera.Instance.UpscaleCamera.WorldToScreenPoint(building.transform.position) + offset;

        IsHeld = true;
    }
    public void Hide()
    {
        IsHeld = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        pointerMoving = true;
    }

    private void Update()
    {
        var pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Mouse.current.position.ReadValue();

        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        IsPointedAt = raycastResults.Find(x => x.gameObject == gameObject).isValid;
        UpdateShow();
    }

    private void UpdateShow()
    {
        gameObject.SetActive(IsHeld || IsPointedAt);
    }
    public void OnItemClicked(int index)
    {
        throw new System.NotImplementedException();
    }
}
