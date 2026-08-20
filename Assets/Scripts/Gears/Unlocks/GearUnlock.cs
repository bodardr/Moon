using System.Collections.Generic;
using System.Linq;
using Save;
using UnityEngine;

[CreateAssetMenu(fileName = "Gear Unlock", menuName = "Gear/Unlock Condition")]
public class GearUnlock : ScriptableObjectWithID
{
    [SerializeReference] IGearUnlockCondition condition;
    [SerializeField] private Gear gearUnlocked;

    public void Initialize()
    {
        condition?.Subscribe();
        condition.OnUnlocked += OnConditionUnlocked;
    }
    
    private void OnConditionUnlocked()
    {
        SaveFile.Current.availableGearInventory[gearUnlocked.UID] += 1;
        SaveFile.Current.gearUnlocks.Add(UID);
    }
    
    public static List<GearUnlock> AllUnlocks = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void InitializeCache()
    {
        AllUnlocks = Resources.LoadAll<GearUnlock>("").ToList();
    }
}