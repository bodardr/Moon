using System;
public interface IGearUnlockCondition
{
    public void Subscribe();
    public void Unsubscribe();
    public void UpdateUnlockCondition();

    public event Action OnUnlocked;
}