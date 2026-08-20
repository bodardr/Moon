using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.Pool;
using Utils;
public class UnlockedGearList : MonoBehaviour
{
    private Grid grid;

    private ObjectPool<GearInventoryElement> gearPool;
    private List<GearInventoryElement> activeGears = new();

    [SerializeField] private GameObject gearElementPrefab;

    private void Awake()
    {
        gearPool = ObjectPoolUtility.CreatePoolFast<GearInventoryElement>(gearElementPrefab);
    }

    private void OnEnable()
    {
        foreach (var activeGear in activeGears)
            if (activeGear.transform.IsChildOf(transform))
                gearPool.Release(activeGear);
        activeGears.Clear();

        foreach (var (gearID, amount) in SaveFile.Current.availableGearInventory)
        {
            var gearElement = gearPool.Get();
            gearElement.transform.SetParent(transform);
            gearElement.transform.localScale = Vector3.one;
            gearElement.Initialize(Gear.AllGears[gearID], amount);
            activeGears.Add(gearElement);
        }
    }
}
