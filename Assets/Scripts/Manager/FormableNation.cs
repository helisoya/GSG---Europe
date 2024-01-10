using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A formable nation is a nation that can be formed by certain countries that own certain provinces
/// </summary>
[System.Serializable]
public class FormableNation
{
    public string Name;

    public string id;

    public List<string> contestants = new List<string>();

    public List<Province> required = new List<Province>();

    public bool alreadyFormed = false;


    public bool CountryHasAllRequirement(Country country)
    {
        foreach (Province prov in required)
        {
            if (!country.provinces.Contains(prov))
            {
                return false;
            }
        }
        return true;
    }
}
