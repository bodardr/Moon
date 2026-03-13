using Save;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using Utils;
using Random = UnityEngine.Random;

public class LuxGeneratorBuilding : BuildingBase
{
    private uint solarPanelCount;
    private float solarPanelTime;

    private bool allowRebounds = false;
    private bool allowSolarPanelRebounds = false;

    private Vector3 sparkPosition;
    private float holdTime;

    private ObjectPool<Spark> sparkPool;

    [SerializeField] private uint baseGeneration = 1;

    [Header("Solar Panels")]
    [SerializeField] private float solarPanelInterval = 1f;

    [Header("Rebounds Settings")]
    [SerializeField] private float reboundChance = 0.05f;
    [SerializeField] private float furtherReboundChanceMultiplier = 0.5f;
    [SerializeField] private uint maxRebounds = 2;

    [SerializeField] private GameObject sparkPrefab;

    private void Awake()
    {
        sparkPool = ObjectPoolUtility.CreatePoolFast<Spark>(sparkPrefab, transform);
    }

    protected override void InitializeUpgrades()
    {
        base.InitializeUpgrades();

        var solarPanel1 = new BuildingUpgrade("Solar Panel I", InstallSolarPanel, false,
            new ResourceWithAmount(ResourceType.Lux, 25));
        var solarPanel2 = new BuildingUpgrade("Solar Panel II", InstallSolarPanel, solarPanel1, false,
            new ResourceWithAmount(ResourceType.Lux, 40));
        var solarPanel3 = new BuildingUpgrade("Solar Panel III", InstallSolarPanel, solarPanel2, true,
            new ResourceWithAmount(ResourceType.Lux, 75));

        var rebounds = new BuildingUpgrade("Rebounds I", () => allowRebounds = true, true,
            new ResourceWithAmount(ResourceType.Lux, 150));
        var solarPanelRebounds = new BuildingUpgrade("Solar Panels can Rebound", () => allowSolarPanelRebounds = true,
            true, new ResourceWithAmount(ResourceType.Lux, 500));

        upgrades.Add(solarPanel1);
        upgrades.Add(solarPanel2);
        upgrades.Add(solarPanel3);

        upgrades.Add(rebounds);
        upgrades.Add(solarPanelRebounds);
    }

    private void Update()
    {
        if (solarPanelCount <= 0)
            return;

        solarPanelTime += Time.deltaTime;
        var interval = solarPanelInterval / solarPanelCount;

        while (solarPanelTime > interval)
        {
            solarPanelTime -= interval;
            Tick(allowSolarPanelRebounds);
        }
    }

    private void InstallSolarPanel()
    {
        solarPanelCount++;
        //visual feedback here
    }
    
    private void Tick(bool allowRebounds = false)
    {
        var bounceCount = 1;

        if (allowRebounds)
        {
            var activeReboundChance = reboundChance;
            for (int i = 0; i < maxRebounds; i++)
            {
                if (Random.Range(0, 1f) <= activeReboundChance)
                {
                    ++bounceCount;
                    activeReboundChance *= furtherReboundChanceMultiplier;
                }
                else
                    break;
            }
        }

        var initialPosition = Random.insideUnitSphere.normalized * (transform.GetChild(0).localScale.x / 2);

        var spark = sparkPool.Get();
        spark.Initialize(bounceCount, initialPosition, sparkPool, OnSparkRebound);
    }

    private void OnSparkRebound(int bounceIndex)
    {
        SaveFile.Current.Lux.Amount += (uint)Mathf.Pow(baseGeneration, bounceIndex);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        holdTime = Time.time;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Tick(allowRebounds);
    }
}
