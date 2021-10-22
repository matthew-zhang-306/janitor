using System.Collections;
using System.Collections.Generic;
using System.Linq;


public interface IUpgradeable
{
    //Reflection method of finding all upgradable properties (should be m_{prop})
    HashSet<string> UpgradeableParameters ();
    void ApplyUpgrade (Upgrade u);

    void Reset ();

    void ApplyAllUpgrades (List<Upgrade> ulist);
}

public struct Upgrade
{
    //ALL UPGRADES ARE ADDITIVE PERCENTAGES
    public string parameter;
    public float value;

    public Upgrade(string p, float v)
    {
        parameter = p;
        value = v;
    }
}