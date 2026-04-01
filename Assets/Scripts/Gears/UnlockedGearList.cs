using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Utils;
public class UnlockedGearList : MonoBehaviour
{
    private Grid grid;

    private ObjectPool<GearDraggableBehavior> gearPool;
    private List<GearDraggableBehavior> activeGears = new();

    [SerializeField] private GameObject gearElementPrefab;
    [SerializeField] private GearSequence gearSequence;

    private void Awake()
    {
        gearPool = ObjectPoolUtility.CreatePoolFast<GearDraggableBehavior>(gearElementPrefab, transform);
    }

    private void OnEnable()
    {
        transform.DetachChildren();
        foreach (var activeGear in activeGears)
            if (activeGear.transform.IsChildOf(transform))
                gearPool.Release(activeGear);
        activeGears.Clear();

        foreach (var gear in GearCache.AllGears)
        {
            var gearElement = gearPool.Get();
            gearElement.Gear = gear;
            gearElement.GearSequence = gearSequence;
            activeGears.Add(gearElement);
        }
    }
}
