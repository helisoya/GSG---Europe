using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A federation is a sort of alliance between multiple countries. It can be united
/// </summary>
public class Federation
{
    public List<Pays> members;
    public Pays leader;
    public Color color;
    public int UCBonus;
    public int DPBonus;
    public int APBonus;

    public bool leaderFrozen;
    public bool vassalized;

    private int yearNextElection;


    /// <summary>
    /// Creates a new Federation
    /// </summary>
    public Federation()
    {
        leaderFrozen = false;
        vassalized = false;
        members = new List<Pays>();
        leader = null;
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        yearNextElection = Manager.instance.an + 8;
    }


    /// <summary>
    /// Add a member to the federation
    /// </summary>
    /// <param name="pays">The new member</param>
    public void AddMember(Pays pays)
    {
        if (members.Contains(pays))
        {
            return;
        }

        foreach (Pays member in members)
        {
            member.AddToTraversalOptions(pays);
        }

        members.Add(pays);
        pays.federation = this;
    }

    /// <summary>
    /// Remove a country from the federation
    /// </summary>
    /// <param name="pays">The country</param>
    public void RemoveMember(Pays pays)
    {
        if (members.Contains(pays))
        {
            members.Remove(pays);
            pays.federation = null;

            foreach (Pays member in members)
            {
                member.RemoveFromTraversalOptions(pays);
            }

            if (members.Count == 0)
            {
                Manager.instance.federations.Remove(this);
                return;
            }

            if (pays == leader)
            {
                GetNewLeader();
            }
        }

    }


    /// <summary>
    /// Unite the federation (Leader annexes all the members)
    /// </summary>
    public void Unite()
    {
        List<Pays> membersCopy = new List<Pays>(members);
        foreach (Pays member in membersCopy)
        {
            if (member == leader) continue;

            leader.RemoveFromTraversalOptions(member);

            List<Province> provincesToAnnex = new List<Province>(member.provinces);
            foreach (Province province in provincesToAnnex)
            {
                member.RemoveProvince(province);
                leader.AddProvince(province, false);
            }
        }

        leader.RefreshProvinces();
        leader.federation = null;
        leader = null;

        members.Clear();
        Manager.instance.federations.Remove(this);
    }


    /// <summary>
    /// Vassalize every member to the leader. If the member has < 50 relation with the leader, it gets kicked out instead
    /// </summary>
    public void EnableVassals()
    {
        vassalized = true;



        List<Pays> toKeep = new List<Pays>();

        foreach (Pays member in members)
        {
            if (member == leader || member.lord == leader)
            {
                toKeep.Add(member);
            }
            else if (member.relations[leader.ID].relationScore >= 50)
            {
                CanvasWorker.instance.UpdateRelations_ShortCut(member, leader, 3);
                toKeep.Add(member);
            }
            else
            {
                member.federation = null;
                member.relations[leader.ID].relationScore -= 20;
            }

        }

        members = toKeep;
    }


    /// <summary>
    /// Find a new leader
    /// </summary>
    public void GetNewLeader()
    {
        int indexMax = 0;
        int max = GetTotalRelationScoreOfCountry(indexMax);

        for (int i = 1; i < members.Count; i++)
        {
            int val = GetTotalRelationScoreOfCountry(i);
            if (val > max || (val == max && Random.Range(0, 2) == 0))
            {
                max = val;
                indexMax = i;
            }
        }
        leader = members[indexMax];
    }


    /// <summary>
    /// Get the total relation score of a country
    /// </summary>
    /// <param name="indexToExclude">The country's index</param>
    /// <returns></returns>
    int GetTotalRelationScoreOfCountry(int indexToExclude)
    {
        Pays p = members[indexToExclude];
        int tot = 0;
        for (int i = 0; i < members.Count; i++)
        {
            if (i != indexToExclude) tot += p.relations[members[i].ID].relationScore;
        }
        return tot;
    }


    /// <summary>
    /// Sets the leader of the country
    /// </summary>
    /// <param name="pays">The new leader</param>
    public void SetLeader(Pays pays)
    {
        leader = pays;
    }

    /// <summary>
    /// Check if it's time for elections
    /// </summary>
    public void CheckYearElection()
    {
        if (leaderFrozen) return;

        if (Manager.instance.an == yearNextElection)
        {
            yearNextElection += 8;
            GetNewLeader();
        }
    }
}
