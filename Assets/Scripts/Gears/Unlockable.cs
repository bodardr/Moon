using System;

public class Unlockable
{
    public bool Unlocked { get; private set; } = false;

    public string Name { get; private set; }
    public event Action<string> OnUnlocked;

    public Unlockable(string name)
    {
        Name = name;
    }

    public void Unlock()
    {
        Unlocked = true;
        OnUnlocked?.Invoke(Name);
    }

    public void CopyFrom(Unlockable unlockable)
    {
        Unlocked = unlockable.Unlocked;
        Name = unlockable.Name;
    }
}
