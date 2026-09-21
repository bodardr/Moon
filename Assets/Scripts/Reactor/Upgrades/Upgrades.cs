using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

[AutoStaticsCleanup]
public partial class Upgrades
{
    public static event PropertyChangedEventHandler PropertyChanged;

    private static List<UpgradeTab> upgradeTabs = new();
    public static List<UpgradeTab> UpgradeTabs => upgradeTabs;

    public static void AddNewTab(string tabName, List<IUpgrade> upgrades)
    {
        upgradeTabs.Add(new(tabName, upgrades));
        PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(UpgradeTabs)));
    }

    public static Upgrades ActiveUpgrades = new();

    public Stat LauncherSpeed = new Stat("Launcher Speed", 2);

    public Stat ParticleMass = new Stat("Particle Mass", 2);
    public Stat ParticleSize = new Stat("Particle Size", 0.15f);

    public Stat EnvironmentDamping = new Stat("Environment Damping", 0.75f);

    public Stat CollisionBoost = new Stat("Collision Boost", 0);
    public Stat CollisionFalloff = new Stat("Collision Boost Falloff", 0.5f);

    // Thruster
    public Unlockable ThrusterUnlockable = new Unlockable("Thruster");
    public Stat ThrusterFuel = new Stat("Thruster Fuel", 1f);
    public Stat ThrusterForce = new Stat("Thruster Force", 10f);

    public Stat DestroyBonus = new Stat("Particle Destroyed Bonus", 3f);
    public Unlockable RedirectUnlockable = new Unlockable("Redirect Tech");

    public event Action<string, float, float> OnStatChanged;
    public event Action<string> OnUnlock;

    [RuntimeInitializeOnLoadMethod]
    public static void LoadUpgrades()
    {
        ActiveUpgrades = new Upgrades();

        foreach (var tab in UpgradeTabs)
        foreach (var upgrade in tab.Upgrades)
            upgrade.Load(ActiveUpgrades);
    }

    public void Initialize()
    {
        using var stats = GetStats();
        while (stats.MoveNext())
            stats.Current.OnValueChanged += OnStatChanged;

        using var unlocks = GetUnlocks();
        while (unlocks.MoveNext())
            unlocks.Current.OnUnlocked += OnUnlock;
    }

    public void Uninitialize()
    {
        using var stats = GetStats();
        while (stats.MoveNext())
            stats.Current.OnValueChanged -= OnStatChanged;
        
        using var unlocks = GetUnlocks();
        while (unlocks.MoveNext())
            unlocks.Current.OnUnlocked -= OnUnlock;
    }

    public Upgrades Copy()
    {
        var copy = new Upgrades();

        using var objStats = GetStats();
        using var copyStats = copy.GetStats();

        while (objStats.MoveNext() && copyStats.MoveNext())
            copyStats.Current?.CopyFrom(objStats.Current);

        using var objUnlocks = GetUnlocks();
        using var copyUnlocks = copy.GetUnlocks();
        
        while (objUnlocks.MoveNext() && copyUnlocks.MoveNext())
            copyUnlocks.Current?.CopyFrom(objUnlocks.Current);
        
        return copy;
    }

    public IEnumerator<Stat> GetStats()
    {
        yield return LauncherSpeed;
        yield return ParticleMass;
        yield return ParticleSize;
        yield return EnvironmentDamping;
        yield return CollisionBoost;
    }

    public IEnumerator<Unlockable> GetUnlocks()
    {
        yield return ThrusterUnlockable;
    }
}
