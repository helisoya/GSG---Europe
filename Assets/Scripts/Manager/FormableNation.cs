using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FormableNation
{
    public string Name;

    public string id;

    public List<string> contestants = new List<string>();

    public List<Province> required = new List<Province>();

    public bool alreadyFormed = false;


    public bool CountryHasAllRequirement(Pays country)
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
