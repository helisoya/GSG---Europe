using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Federation()
    {
        leaderFrozen = false;
        vassalized = false;
        members = new List<Pays>();
        leader = null;
        color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        yearNextElection = Manager.instance.an + 8;
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

    public void Unite()
    {

        foreach (Pays member in members)
        {
            if (member == leader) continue;

            List<Province> provincesToAnnex = new List<Province>(member.provinces);
            foreach (Province province in provincesToAnnex)
            {
                Debug.Log(province.Province_Name);
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

    public void EnableVassals()
    {
        vassalized = true;



        List<Pays> toKeep = new List<Pays>();

        foreach (Pays member in members)
        {
            if (member == leader)
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

    public void SetLeader(Pays pays)
    {
        leader = pays;
    }


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
