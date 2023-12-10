using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A party of a country
/// </summary>
public class Party
{
    public string partyName;
    public float popularity;


    /// <summary>
    /// Creates a new party
    /// </summary>
    /// <param name="newName">Party name</param>
    /// <param name="newPop">Party popularity</param>
    public Party(string newName, float newPop)
    {
        partyName = newName;
        popularity = newPop;
    }
}
