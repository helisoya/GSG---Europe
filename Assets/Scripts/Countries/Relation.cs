using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation
{
    public int relationScore;
    public bool atWar;

    public List<string> wargoals;

    public Pays[] pays;

    public Relation(Pays p1, Pays p2)
    {
        atWar = false;
        wargoals = new List<string>();
        pays = new Pays[]{
            p1,
            p2
        };
    }

    public void AddScore(int amount)
    {
        relationScore = Mathf.Clamp(relationScore + amount, -100, 100);
        if (relationScore == -100 && !pays[0].atWarWith.ContainsKey(pays[1].ID))
        {
            pays[0].AI_MARKFORWAR.Add(pays[1].ID);
            pays[1].AI_MARKFORWAR.Add(pays[0].ID);
        }
    }
}
