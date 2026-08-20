using System.Collections.Generic;
using Save;
using UnityEngine;
using UnityEngine.EventSystems;

public class LuxGeneratorBuilding : BuildingBase, IFirstLoadCallback
{
    private uint solarPanelCount;
    private float solarPanelTime;

    private bool allowRebounds = false;

    private Vector3 sparkPosition;
    private float holdTime;

    private GearTarget gearTarget;
    private GearHolder gearHolder;

    private Dictionary<string, object> sparkParams;

    [SerializeField] private uint baseGeneration = 1;

    [Header("Solar Panels")]
    [SerializeField] private float solarPanelInterval = 1f;

    [Header("Rebounds Settings")]
    [SerializeField] private float reboundChance = 0.05f;
    [SerializeField] private float furtherReboundChanceMultiplier = 0.5f;
    [SerializeField] private uint maxRebounds = 2;

    [SerializeField] private GameObject sparkPrefab;
    [SerializeField] private CraftingBuilding observatory;

    [SerializeField] private Gear[] initialClickGears;

    [SerializeField] private Gear solarPanelGear;
    [SerializeField] private Gear sparkGear;
    [SerializeField] private Gear luxGear;

    [SerializeField] private Gear ampGear;

    private void Awake()
    {
        gearTarget = GetComponent<GearTarget>();
        gearHolder = GetComponent<GearHolder>();
        sparkParams = new Dictionary<string, object>
        {
            { SparkGearBehavior.ALLOW_REBOUNDS_KEY, allowRebounds },
            { SparkGearBehavior.MAX_REBOUNDS_KEY, maxRebounds },
            { SparkGearBehavior.REBOUND_CHANCE_KEY, reboundChance },
        };
    }

    public void OnFirstLoad(SaveFile saveFile)
    {
        gearHolder.Gears = new List<List<List<Gear>>>
        {
            GearUtility.CreateGearSequence(initialClickGears),
            new() { new() { solarPanelGear }, new() { sparkGear, luxGear } },
        };
    }

    protected override void InitializeUpgrades()
    {
        var solarPanel1 = new BuildingUpgrade("Solar Panel", InstallSolarPanel, false,
            new ResourceWithAmount(ResourceType.Lux, 20));
        var solarPanel2 = new BuildingUpgrade("Solar Panel II", InstallSolarPanel, solarPanel1, false,
            new ResourceWithAmount(ResourceType.Lux, 45));
        var solarPanel3 = new BuildingUpgrade("Solar Panel III", InstallSolarPanel, solarPanel2, true,
            new ResourceWithAmount(ResourceType.Lux, 75));

        var solarPanelRebounds = new BuildingUpgrade("Solar Panels can Rebound", _ => allowRebounds = true,
            solarPanel3, true, new ResourceWithAmount(ResourceType.Lux, 500));

        var unlockObservatory = new BuildingUpgrade("Unlock Observatory", UnlockObservatoryAndGears, true,
            new ResourceWithAmount(ResourceType.Lux, 125));

        upgrades.Add(solarPanel1);
        upgrades.Add(solarPanel2);
        upgrades.Add(solarPanel3);

        upgrades.Add(unlockObservatory);

        upgrades.Add(solarPanelRebounds);
    }


    private void Update()
    {
        if (solarPanelCount <= 0)
            return;

        solarPanelTime += Time.deltaTime * gearTarget.Stats.SpeedRate.EvaluatedValue;
        var interval = solarPanelInterval / solarPanelCount;

        while (solarPanelTime > interval)
        {
            solarPanelTime -= interval;
            GearUtility.PlayGearSequence(gearHolder.Gears[1], 2, sparkParams, gearTarget);
        }
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        GearUtility.PlayGearSequence(gearHolder.Gears[0], 1, null, gearTarget);
    }

    private void InstallSolarPanel(bool fromLoad)
    {
        solarPanelCount++;
    }

    private void UnlockObservatoryAndGears(bool fromLoad)
    {
        observatory.gameObject.SetActive(true);
        SaveFile.Current.GearsUnlocked = true;

        if (!fromLoad)
            SaveFile.Current.AddGearToInventory(ampGear);
    }
}
