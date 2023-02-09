using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party
{
    public string partyName;
    public float popularity;

    public Party(string newName, float newPop)
    {
        partyName = newName;
        popularity = newPop;
    }
}
