using System.ComponentModel;
using UnityEngine;
public class GearInventoryElement : MonoBehaviour, INotifyPropertyChanged, IGearDraggable
{
    private uint amount;
    private Gear gear;

    [SerializeField] private bool interactable = true;

    public Gear Gear
    {
        get => gear;
        set
        {
            gear = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Gear)));
        }
    }

    public uint Amount
    {
        get => amount;
        set
        {
            amount = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
        }
    }

    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Interactable)));
        }
    }
    public bool CanDrag => Interactable && Amount > 0;

    public event PropertyChangedEventHandler PropertyChanged;

    public void Initialize(Gear gear, uint amount)
    {
        Gear = gear;
        Amount = amount;
    }

    public Gear BeginDrag()
    {
        Amount--;
        return Gear;
    }

    public void OnDragCancelled()
    {
        Amount++;
    }
}
