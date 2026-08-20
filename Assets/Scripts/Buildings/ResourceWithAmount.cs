using Save;
public class ResourceWithAmount
{
    public ResourceType ResourceType;
    public uint Amount;

    public bool CanAfford => SaveFile.Current[ResourceType].Amount >= Amount;

    public ResourceWithAmount(ResourceType resourceType, uint amount = 0)
    {
        ResourceType = resourceType;
        Amount = amount;
    }
}