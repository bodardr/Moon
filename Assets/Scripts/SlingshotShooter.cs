using System.ComponentModel;
using Save;
using UnityEngine;

public enum Resource
{
    Wood,
    Stone,
    Iron,
    Lunarite,
    Gold
}

public class SlingshotShooter : MonoBehaviour, INotifyPropertyChanged
{
    private uint capacityLoaded;

    [SerializeField] private float capacityLoadRatioPerClick = 0.15f;
    [SerializeField] private uint loadCapacityRequired;
    [SerializeField] private Resource ammoType;

    public float CapacityLoadRatio => capacityLoaded / (float)loadCapacityRequired;
    public bool LaunchReady => CapacityLoadRatio >= 1f;

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnClick()
    {
        var save = SaveFile.Current;

        uint amount = 0;
        switch (ammoType)
        {
            case Resource.Wood:
                amount = save.wood;
                break;
            case Resource.Stone:
                amount = save.stone;
                break;
            case Resource.Iron:
                amount = save.iron;
                break;
            case Resource.Lunarite:
                amount = save.lunarite;
                break;
            case Resource.Gold:
                amount = save.gold;
                break;
        }


        var amountToLoad = loadCapacityRequired * CapacityLoadRatio;
        if (amount > amountToLoad)
        {
            capacityLoaded += (uint)amountToLoad;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CapacityLoadRatio)));   
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LaunchReady)));   
        }
    }
}
