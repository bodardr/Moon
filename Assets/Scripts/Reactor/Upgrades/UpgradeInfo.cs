using System;

[Serializable]
public class UpgradeInfo
{
    public string name;
    public string description;
    public UpgradeInfo(string name, string description)
    {
        this.name = name;
        this.description = description;
    }
}
