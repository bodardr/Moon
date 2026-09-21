using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeSystem : MonoSingleton<UpgradeSystem>, INotifyPropertyChanged
{
    [SerializeField] private UpgradeTooltip upgradeTooltip;
    [SerializeField] private Carousel carousel;
    
    public UpgradeTab CurrentTab => carousel.CurrentIndex >= Upgrades.UpgradeTabs.Count ? null : Upgrades.UpgradeTabs[carousel.CurrentIndex];

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnEnable()
    {
        carousel.OnCurrentIndexChanged += UpdateCurrentTab;
        Upgrades.ActiveUpgrades.Initialize();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTab)));
    }
    private void OnDisable()
    {
        carousel.OnCurrentIndexChanged -= UpdateCurrentTab;
        Upgrades.ActiveUpgrades.Uninitialize();
    }
    
    private void UpdateCurrentTab(int index)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentTab)));
    }
    
    public void OnUpgradeHovered(IUpgrade upgrade)
    {
        upgradeTooltip.Show(Upgrades.ActiveUpgrades, upgrade);
    }
    
    public void OnUpgradeUnhovered(IUpgrade upgrade)
    {
        upgradeTooltip.Hide(upgrade);
    }

    public void OnUpgradeClicked(IUpgrade upgrade)
    {
        if (upgrade.Unlocked || upgrade.Cost > Currencies.Active.Credits)
            return;

        Currencies.Active.Credits -= upgrade.Cost;
        upgrade.Apply(Upgrades.ActiveUpgrades);
        upgrade.Unlock();
        upgrade.Save();
    }

    public void GoToReactorScene()
    {
        SceneManager.LoadScene("Reactor Scene");
    }
}