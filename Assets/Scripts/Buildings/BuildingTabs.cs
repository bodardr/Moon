using System;

[Flags]
public enum BuildingTabs
{
    Upgrades = 1 << 0,
    Gears = 1 << 1,
    Research = 1 << 2,
    All =  Upgrades | Gears | Research,
    None = 0,
}
