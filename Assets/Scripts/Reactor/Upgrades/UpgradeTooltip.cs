using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
public class UpgradeTooltip : MonoBehaviour, INotifyPropertyChanged
{
    public class StatChange
    {
        public string StatName;
        public float OldValue;
        public float NewValue;

        public StatChange(string statName, float oldValue, float newValue)
        {
            StatName = statName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    
    private Upgrades upgradesPreview = new();
    private IUpgrade hoveredUpgrade;

    private readonly List<StatChange> statsChanged = new();
    private readonly List<string> unlocksChanged = new();

    public List<StatChange> StatsChanged => statsChanged;
    public List<string> UnlocksChanged => unlocksChanged;
    public IUpgrade HoveredUpgrade => hoveredUpgrade;

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnEnable()
    {
        upgradesPreview.OnUnlock += OnUnlock;
        upgradesPreview.OnStatChanged += OnStatChanged;
        upgradesPreview.Initialize();
    }

    private void OnDisable()
    {
        upgradesPreview.OnUnlock -= OnUnlock;
        upgradesPreview.OnStatChanged -= OnStatChanged;
        upgradesPreview.Uninitialize();
    }

    public void Show(Upgrades activeUpgrades, IUpgrade upgrade)
    {
        //this might be costly to do this every single time.
        //We might have to research ways to avoid doing that.
        hoveredUpgrade = upgrade;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HoveredUpgrade)));

        upgradesPreview = activeUpgrades.Copy();
        
        unlocksChanged.Clear();
        statsChanged.Clear();

        gameObject.SetActive(true);

        upgrade.Apply(upgradesPreview);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatsChanged)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UnlocksChanged)));
    }

    public void Hide(IUpgrade upgrade)
    {
        //The upgrade is added as a parameter because the pointer up callback
        //gets called if the user switches upgrades by hovering.
        //If it is the case, it might be disabled while it's hovering on a 
        //new upgrade. This checks if it's the most recent upgrade being checked.
        if (hoveredUpgrade != upgrade)
            return;

        gameObject.SetActive(false);
    }

    private void OnStatChanged(string statName, float oldValue, float newValue)
    {
        statsChanged.Add(new(statName, oldValue, newValue));
    }

    private void OnUnlock(string statName)
    {
        unlocksChanged.Add(statName);
    }
}
