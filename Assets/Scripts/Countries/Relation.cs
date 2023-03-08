using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation
{
    public int relationScore;
    public bool atWar;

    public List<string> wargoals;

    public Relation()
    {
        atWar = false;
        wargoals = new List<string>();
    }

    public void AddScore(int amount)
    {
        relationScore = Mathf.Clamp(relationScore + amount, -100, 100);
    }
}
