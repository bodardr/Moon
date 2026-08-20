using System;
[Flags]
public enum GearProperties
{
    Unlockable = 1 << 0,
    Displaceable = 1 << 1,
    Expandable = 1 << 2,
    All = Unlockable | Displaceable | Expandable,
    None = 0
}