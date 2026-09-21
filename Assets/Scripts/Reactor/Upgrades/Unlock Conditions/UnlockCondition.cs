class UnlockCondition : IUnlockCondition
{
    private readonly Unlockable unlockable;

    public UnlockCondition(Unlockable unlockable)
    {
        this.unlockable = unlockable;
    }

    public bool IsUnlocked() => unlockable.Unlocked;
}
