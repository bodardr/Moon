using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingTooltip : MonoSingleton<BuildingTooltip>, INotifyPropertyChanged, IPointerMoveHandler, ICollectionCallback
{
    private bool pointerMoving;
    private bool isPointedAt = false;
    private bool isHeld;
    private List<RaycastResult> raycastResults = new();

    private BuildingBase current;
    private List<BuildingUpgrade> upgradesToShow;

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
            UpdateUpgrades();
        }
    }
    public List<BuildingUpgrade> UpgradesToShow
    {
        get => upgradesToShow;
        set
        {
            upgradesToShow = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UpgradesToShow)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public void ShowFromBuilding(BuildingBase building, Vector2 offset)
    {
        Current = building;
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

    private void UpdateUpgrades()
    {
        UpgradesToShow = current?.Upgrades.Where(x =>
                x.Prerequisites == null || x.Prerequisites.Count == 0 ||
                !x.Prerequisites.Any(y => SaveFile.Current.buildingUpgrades.Contains(y.Name)))
            .ToList();
    }

    private void UpdateShow()
    {
        gameObject.SetActive(IsHeld || IsPointedAt);
    }
    
    public void OnItemClicked(int index)
    {
        var upgrade = UpgradesToShow[index];
        if (upgrade.IsUnlocked || !upgrade.CanAfford)
            return;
        
        upgrade.SubtractCosts();
        upgrade.Activate();
        
        UpdateUpgrades();
    }
}
