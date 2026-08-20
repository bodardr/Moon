public interface IBuildingUpgrade
{
    public string Name { get; }
    public ResourceWithAmount[] Costs { get; }
    public bool ShouldShow { get; }
    public bool IsUnlocked { get; }
}