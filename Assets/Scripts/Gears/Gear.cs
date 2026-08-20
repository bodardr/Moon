using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear", menuName = "Gear/Gear")]
public class Gear : ScriptableObjectWithID
{
    [SerializeField] private string gearName;
    [SerializeField] private Sprite icon;
    [SerializeField] private GearProperties properties = GearProperties.Unlockable | GearProperties.Displaceable;

    [SerializeReference] private IGearBehavior triggerBehavior;

    public bool Unlockable => Properties.HasFlag(GearProperties.Unlockable);
    public bool Displaceable => Properties.HasFlag(GearProperties.Displaceable);
    public bool Expandable => Properties.HasFlag(GearProperties.Expandable);
    public string GearName => gearName;
    public Sprite Icon => icon;
    public GearProperties Properties => properties;
    public IGearBehavior TriggerBehavior => triggerBehavior;
    
    public static Dictionary<string, Gear> AllGears;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        AllGears = Resources.LoadAll<Gear>("").ToDictionary(x => x.UID, x => x);
    }
}