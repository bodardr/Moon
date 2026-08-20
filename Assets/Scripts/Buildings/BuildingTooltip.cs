using System;
using System.Collections.Generic;
using System.ComponentModel;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingTooltip : MonoSingleton<BuildingTooltip>, INotifyPropertyChanged, IPointerMoveHandler
{
    private bool pointerMoving;
    private bool isPointedAt = false;
    private bool isHeld;
    private List<RaycastResult> raycastResults = new();

    private TabController tabController;

    private BuildingBase current;
    private RectTransform rectTransform;

    [SerializeField] private Button[] tabButtons;

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

    private void Awake()
    {
        tabController = GetComponent<TabController>();
        for (var i = 0; i < tabButtons.Length; i++)
        {
            var button = tabButtons[i];
            var index = i;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => tabController.SetTabIndex(index));
        }
    }

    public void ShowFromBuilding(BuildingBase building, Vector2 pivot, Vector2 offset)
    {
        Current = building;

        if (rectTransform == null)
            rectTransform = (RectTransform)transform;

        rectTransform.pivot = pivot;
        transform.position =
            (Vector2)PixelateCamera.Instance.Camera.WorldToScreenPoint(building.transform.position) + offset;

        UpdateTabs(Current.Tabs);

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
    }

    private void UpdateTabs(BuildingTabs tabs)
    {
        var values = (BuildingTabs[])Enum.GetValues(typeof(BuildingTabs));

        foreach (var tab in values)
        {
            if (tab is BuildingTabs.All or BuildingTabs.None)
                continue;
            
            var active = tabs.HasFlag(tab);

            if (tab == BuildingTabs.Gears)
                active = active && SaveFile.Current.GearsUnlocked;
            
            var index = (int)Mathf.Log((int)tab,2);
            tabButtons[index].gameObject.SetActive(active);
        }
    }
    
    private void UpdateShow()
    {
        //gameObject.SetActive(IsHeld || IsPointedAt);
    }
}
