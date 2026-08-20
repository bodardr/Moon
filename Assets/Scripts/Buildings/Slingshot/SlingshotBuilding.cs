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
        upgrades.Add(new BuildingUpgrade("Build Slingshot", _ => IsBuilt = true, true,
            new ResourceWithAmount(ResourceType.Lux, 20)));
    }

    protected override void InitializeUpgrades()
    {
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (IsBuilt && !slingshotShooter.LaunchReady)
            slingshotShooter.LoadMaterials();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (IsBuilt && slingshotShooter.LaunchReady)
            slingshotShooter.OnLaunchHold();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (IsBuilt && slingshotShooter.LaunchReady)
            slingshotShooter.OnLauncherRelease();
    }
}
