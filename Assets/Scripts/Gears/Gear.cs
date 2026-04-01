using UnityEngine;

[CreateAssetMenu(fileName = "Gear",  menuName = "Gear")]
public class Gear : ScriptableObject
{
    [SerializeField] private bool obtainable = true;
    [SerializeField] private bool displaceable = true;
    
    [SerializeField] private string gearName;
    [SerializeField] private Sprite icon;
    [SerializeField] private GearType type;

    [SerializeReference] private IGearBehavior triggerBehavior;
    
    public bool Obtainable => obtainable;
    public bool Displaceable => displaceable;
    public string GearName => gearName;
    public Sprite Icon => icon;
    public GearType Type => type;
    public IGearBehavior TriggerBehavior => triggerBehavior;
}