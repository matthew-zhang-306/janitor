using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Upgradeable : MonoBehaviour
{
    //Reflection method of finding all upgradable properties (should be m_{prop})
    protected Dictionary<string, float> baseProps;

    public void GetBaseProps ()
    {
        //should be called on START or reset.
        var hs = this.UpgradeableParameters();
        this.baseProps = new Dictionary<string, float>();
        SerializedObject so = new SerializedObject (this);
        foreach (string prop in hs) {
            baseProps[prop] = so.FindProperty(prop).floatValue;
        }
        foreach (var k in baseProps) {
            Debug.Log(k.Value);
        }
    }

    public HashSet<string> UpgradeableParameters()
    {
        var propset = new HashSet<string>();
        SerializedObject so = new SerializedObject (this);
        SerializedProperty it = so.GetIterator();
        while (it.Next(true)) {
            if (it.type == "float") {
                propset.Add(it.name);
            }
        }
        return propset;
    }

    public void ApplyUpgrade(Upgrade u)
    {
        if (baseProps.ContainsKey(u.parameter))
        {
            SerializedObject so = new SerializedObject (this);

            //Fix this later
            so.FindProperty(u.parameter).floatValue += u.value;
            so.ApplyModifiedProperties();
        }
    }

    public void Reset()
    {
        SerializedObject so = new SerializedObject (this);
        foreach (var x in baseProps) 
        {
            so.FindProperty (x.Key).floatValue = x.Value;
        }
        so.ApplyModifiedProperties();
    }

    public void ApplyAllUpgrades(List<Upgrade> ulist)
    {
        Reset();
        foreach (var u in ulist) {
            ApplyUpgrade (u);
        }
    }
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