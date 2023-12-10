using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable Object representing a unit type and its stats
/// </summary>
[CreateAssetMenu(fileName = "UnitTypeInfo", menuName = "GSG/UnitTypeInfo", order = 1)]
public class UnitTypeInfo : ScriptableObject
{
    public UnitType type;
    public string unitName;
    public Sprite unitSprite;
    public float baseHP;
    public float baseAttack;
    public float baseDefense;
    public float baseTravelSpeed;
    public float baseNavelAttack;
}
