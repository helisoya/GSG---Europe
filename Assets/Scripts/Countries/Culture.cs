using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A culture contains different informations to make country more unique
/// </summary>
public class Culture
{
    public string id;
    public string[] noms;

    public string[] prenoms;

    public GameObject prefabTank;
    public Dictionary<string, Focus> focusTree;


    /// <summary>
    /// Return a random last name
    /// </summary>
    /// <returns>A random last name</returns>
    public string GetRandom_Nom()
    {

        return noms[Random.Range(0, noms.Length)];
    }

    /// <summary>
    /// Return a random first name
    /// </summary>
    /// <returns>A random first name</returns>
    public string GetRandom_Prenom()
    {
        return prenoms[Random.Range(0, prenoms.Length)];
    }


}