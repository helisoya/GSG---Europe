using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GovernementType
{
    REPUBLIC,
    MONARCHY,
    COMMUNISM,
    FASCISM
}

/// <summary>
/// Class representing a governement type in the game
/// </summary>
[System.Serializable]
public class Governement
{
    public GovernementType type;
    public string governementName;
    public string governementDescription;
}