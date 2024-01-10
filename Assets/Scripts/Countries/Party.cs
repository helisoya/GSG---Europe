using System;
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
    public PartyType partyType;


    /// <summary>
    /// Creates a new party
    /// </summary>
    /// <param name="newName">Party name</param>
    /// <param name="newPop">Party popularity</param>
    /// /// <param name="partyTypeIndex">PartyType index</param>
    public Party(string newName, float newPop, int partyTypeIndex)
    {
        partyName = newName;
        popularity = newPop;
        partyType = (PartyType)Enum.GetValues(typeof(PartyType)).GetValue(partyTypeIndex);
    }
}


public enum PartyType
{
    COMMUNIST,
    SOCIALIST,
    CENTRIST,
    CONSERVATIVE,
    FASCIST

}