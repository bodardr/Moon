using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.EventSystems;
public class SlingshotBuilding : BuildingBase, INotifyPropertyChanged
{
    private bool isBuilt;
    [SerializeField] private SlingshotShooter slingshotShooter;

    public bool IsBuilt
    {
        get => isBuilt;
        private set
        {
            isBuilt = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsBuilt)));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void Awake()
    {
        upgrades.Add(new BuildingUpgrade("Build Slingshot", () => IsBuilt = true,
            new ResourceWithAmount(Resource.Wood, 20)));
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (IsBuilt && !slingshotShooter.LaunchReady)
            slingshotShooter.LoadMaterials();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (IsBuilt && slingshotShooter.LaunchReady)
            slingshotShooter.OnLaunchHold();
    }

    override public void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (IsBuilt && slingshotShooter.LaunchReady)
            slingshotShooter.OnLauncherRelease();
    }
}
