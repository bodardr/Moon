using System;
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
public class LoadoutAttribute : Attribute
{
    public enum StorageType
    {
        File,
        EditorPrefs
    }

    public readonly StorageType storageType;

    public LoadoutAttribute(StorageType storageType = StorageType.File)
    {
        this.storageType = storageType;
    }
}
