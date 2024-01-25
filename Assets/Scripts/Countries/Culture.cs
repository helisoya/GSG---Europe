using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A culture contains different informations to make country more unique
/// </summary>
public class Culture
{
    public string id;
    public string[] surnames;

    public string[] names;

    public Dictionary<UnitType, GameObject> prefabUnits;
    public Dictionary<string, Focus> focusTree;


    /// <summary>
    /// Return a random last name
    /// </summary>
    /// <returns>A random last name</returns>
    public string GetRandom_Surname()
    {

        return surnames[Random.Range(0, surnames.Length)];
    }

    /// <summary>
    /// Return a random first name
    /// </summary>
    /// <returns>A random first name</returns>
    public string GetRandom_Name()
    {
        return names[Random.Range(0, names.Length)];
    }


}