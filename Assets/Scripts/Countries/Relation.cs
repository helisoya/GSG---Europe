using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles relations between two countries
/// </summary>
public class Relation
{
    public int relationScore;
    public bool atWar;

    public List<string> wargoals;

    public Pays[] pays;
    public Dictionary<string, int> warScores;

    public Relation(Pays p1, Pays p2)
    {
        atWar = false;
        wargoals = new List<string>();
        pays = new Pays[]{
            p1,
            p2
        };

        warScores = new Dictionary<string, int>();
        warScores.Add(p1.ID, 0);
        warScores.Add(p2.ID, 0);
    }

    /// <summary>
    /// Add warscore
    /// </summary>
    /// <param name="amount">warscore amount</param>
    public void AddScore(int amount)
    {
        relationScore = Mathf.Clamp(relationScore + amount, -100, 100);
        if (relationScore == -100 && !pays[0].atWarWith.Contains(pays[1].ID))
        {
            pays[0].AI_MARKFORWAR.Add(pays[1].ID);
            pays[1].AI_MARKFORWAR.Add(pays[0].ID);
        }
    }

    /// <summary>
    /// Reset war scores
    /// </summary>
    public void ResetWarScores()
    {
        warScores[pays[0].ID] = 0;
        warScores[pays[1].ID] = 0;
    }
}
