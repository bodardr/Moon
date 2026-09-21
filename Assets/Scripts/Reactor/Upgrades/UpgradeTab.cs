using System.Collections.Generic;
public class UpgradeTab
{
    public string TabName;
    public List<IUpgrade> Upgrades;
    
    public UpgradeTab(string tabName, List<IUpgrade> upgrades)
    {
        TabName = tabName;
        Upgrades = upgrades;
    }
}
