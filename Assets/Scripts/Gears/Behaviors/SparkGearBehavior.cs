using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Utils;
using Random = UnityEngine.Random;
[Serializable]
public class SparkGearBehavior : IGearBehavior
{
    public const string ALLOW_REBOUNDS_KEY = "allowRebounds";
    public const string REBOUND_CHANCE_KEY = "reboundChance";
    public const string MAX_REBOUNDS_KEY = "maxRebounds";
    public const string FURTHER_REBOUNDS_CHANCE_MULTIPLIER_KEY = "furtherReboundChanceMultiplier";

    private static ObjectPool<Spark> sparkPool;
    [SerializeField] private GameObject sparkPrefab;

    public void OnGearTriggered(GearTarget[] targets, ref uint amplitude, List<Gear> gearColumn,
        Dictionary<string, object> additionalParameters)
    {
        sparkPool ??= ObjectPoolUtility.CreatePoolFast<Spark>(sparkPrefab);

        var reboundsAllowed = (bool)(additionalParameters?.GetValueOrDefault(ALLOW_REBOUNDS_KEY) ?? false);
        var maxRebounds = (uint)(additionalParameters?.GetValueOrDefault(MAX_REBOUNDS_KEY) ?? 0);
        var reboundChance = (float)(additionalParameters?.GetValueOrDefault(REBOUND_CHANCE_KEY) ?? 0);

        foreach (var target in targets)
        {
            var bounceCount = 1;

            if (reboundsAllowed)
            {
                for (bounceCount = 0; bounceCount < maxRebounds; bounceCount++)
                {
                    if (Random.Range(0, 1f) > reboundChance)
                        break;
                }
            }

            var initialPosition = Random.insideUnitSphere.normalized * (target.transform.GetChild(0).localScale.x / 2);

            var spark = sparkPool.Get();
            spark.Initialize(bounceCount, initialPosition, sparkPool, target, gearColumn, amplitude);
        }
    }
}
