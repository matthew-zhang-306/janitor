using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System;

public class Upgradeable : MonoBehaviour
{
    //Reflection method of finding all upgradable properties (should be m_{prop})
    protected Dictionary<string, float> baseProps;
    protected Dictionary<string, float> modifier;

    // protected virtual void Start ()
    // {
    //     GetBaseProps();
    // }

    public void GetBaseProps ()
    {
        //should be called on START or reset.
        var hs = this.UpgradeableParameters();
        this.baseProps = new Dictionary<string, float>();
        this.modifier = new Dictionary<string, float>();
        var obj = JsonUtility.ToJson(this);
        var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);

        // JsonUtility.FromJsonOverwrite(obj, this.baseProps);
        foreach (string prop in hs) {
            this.baseProps[prop] = Convert.ToSingle (values[prop]);
            this.modifier[prop] = 1;
        }
        
    }

    public HashSet<string> UpgradeableParameters()
    {
        var propset = new HashSet<string>();
        var obj = JsonUtility.ToJson(this);
        var test = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);

        // JsonUtility.FromJsonOverwrite(obj, this.baseProps);
        foreach (var key in test) {
            if (key.Value is Double || key.Value is Int64) {
                propset.Add (key.Key);
            }
            
        }
        return propset;
    }

    public void ApplyUpgrade(Upgrade u)
    {
        if (baseProps.ContainsKey(u.parameter))
        {
            var obj = JsonUtility.ToJson(this);
            var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);

            modifier[u.parameter] += u.value;
            values[u.parameter] = baseProps[u.parameter] * modifier[u.parameter];
            
            JsonUtility.FromJsonOverwrite (JsonConvert.SerializeObject(values), this);
        }
    }

    public void Reset()
    {
        // Debug.Log("reseting " + gameObject.name);
        var obj = JsonUtility.ToJson(this);
        var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(obj);

        foreach (var x in baseProps) 
        {
            values[x.Key] = x.Value;
            this.modifier[x.Key] = 1;
            // Debug.Log(String.Format ("{0} {1} {2}", gameObject.name, x.Key, x.Value));

        }
        JsonUtility.FromJsonOverwrite (JsonConvert.SerializeObject(values), this);
    }

    public void ApplyAllUpgrades(List<Upgrade> ulist)
    {
        Reset();
        foreach (var u in ulist) {
            ApplyUpgrade (u);
        }
    }
}

[System.Serializable]
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