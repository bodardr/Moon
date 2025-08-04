using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;
public class BuildingTooltip : MonoSingleton<BuildingTooltip>, INotifyPropertyChanged, IPointerDownHandler,
    IPointerUpHandler
{
    private bool isPointedAt = false;
    private bool isHeld;

    private BuildingBase current;
    private List<BuildingUpgrade> upgradesToShow;

    public bool IsHeld
    {
        get => isHeld;
        set
        {
            isHeld = value;
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
        transform.position = (Vector2)building.transform.position + offset;
        IsHeld = true;
    }
    public void Hide()
    {
        IsHeld = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPointedAt = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPointedAt = false;
    }

    private void UpdateUpgrades()
    {
        UpgradesToShow = current?.Upgrades.Where(x =>
                x.Prerequisites.Count == 0 ||
                !x.Prerequisites.Any(y => SaveFile.Current.buildingUpgrades.Contains(y.Name)))
            .ToList();
    }

    private void UpdateShow()
    {
        gameObject.SetActive(IsHeld || IsPointedAt);
    }
}
