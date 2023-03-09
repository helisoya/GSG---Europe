using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Federation
{
    public List<Pays> members;
    public Pays leader;
    public Color color;


    public Federation()
    {
        members = new List<Pays>();
        leader = null;
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public void AddMember(Pays pays)
    {
        if (members.Contains(pays))
        {
            return;

        }
        members.Add(pays);
        pays.federation = this;
    }

    public void RemoveMember(Pays pays)
    {
        if (members.Contains(pays))
        {
            members.Add(pays);
            pays.federation = null;
            if (pays == leader)
            {
                GetNewLeader();
            }
        }

    }

    public void GetNewLeader()
    {
        int indexMax = 0;
        int max = GetTotalRelationScoreOfCountry(indexMax);

        for (int i = 1; i < members.Count; i++)
        {
            int val = GetTotalRelationScoreOfCountry(i);
            if (val > max)
            {
                max = val;
                indexMax = i;
            }
        }
        leader = members[indexMax];
    }

    int GetTotalRelationScoreOfCountry(int indexToExclude)
    {
        Pays p = members[indexToExclude];
        int tot = 0;
        for (int i = 0; i < members.Count; i++)
        {
            if (i != indexToExclude) tot += p.relations[members[indexToExclude].ID].relationScore;
        }
        return tot;
    }

    public void SetLeader(Pays pays)
    {
        leader = pays;
    }
}
