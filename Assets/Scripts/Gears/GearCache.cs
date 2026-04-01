using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class GearCache
{
    public static List<Gear> AllGears;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        AllGears = Resources.LoadAll<Gear>("").ToList();
    }
}