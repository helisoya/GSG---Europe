using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Manager manager;


    public void IA(Pays country)
    {
        if (country.provinces.Count == 0) return;

        if (country.currentFocusTime == 0)
        {
            ChangeFocus(country);
        }

        if (Random.Range(0, 5) != 0)
        {
            return;
        }

        if (!country.hasTech_Naval)
        {
            country.hasTech_Naval = true;
        }
        BuyUnit(country);

        if (country.lord == null)
        {
            ManageDiplomacy(country);
            ManageFederation(country);
        }
        UnitMovement(country);

    }


    void BuyUnit(Pays country)
    {
        if (country.provinces == null || !country.canBuyUnit || country.provinces.Count == 0 || country.AP < 100)
        {
            return;
        }

        if (Random.Range(0, 50) == 0)
        {
            country.AP -= 100;
            country.provinces[Random.Range(0, country.provinces.Count)].SpawnUnitAtCity();
        }
    }

    void ManageDiplomacy(Pays country)
    {
        List<string> list = new List<string>(country.atWarWith);
        foreach (string pays in list)
        {
            Pays p = manager.GetCountry(pays);
            if (p.CompletelyOccupied())
            {
                country.MakePeaceWithCountry(p);
            }
        }

        if (country.AI_NEIGHBOORS.Count == 0) return;

        Pays diplomacyWith = country.AI_NEIGHBOORS[Random.Range(0, country.AI_NEIGHBOORS.Count)];

        if (diplomacyWith != country)
        {
            if (Random.Range(0, 100) <= 5 && (country.federation == null || country.federation != diplomacyWith.federation))
            {
                country.relations[diplomacyWith.ID].AddScore(-666);
            }
            else
            {
                country.relations[diplomacyWith.ID].AddScore(Random.Range(-30, 30));
            }

        }

        foreach (string pays in country.AI_MARKFORWAR)
        {
            if (!country.atWarWith.Contains(pays))
            {
                country.DeclareWarOnCountry(manager.GetCountry(pays));
            }
        }
        country.AI_MARKFORWAR.Clear();
    }


    void ManageFederation(Pays country)
    {
        if (country.AI_NEIGHBOORS.Count == 0) return;
        Pays tryWith = country.AI_NEIGHBOORS[Random.Range(0, country.AI_NEIGHBOORS.Count)];
        if (tryWith.federation != null || tryWith == manager.player ||
        tryWith.atWarWith.Contains(country.ID) || country.relations[tryWith.ID].relationScore < 50) return;


        if (country.federation == null)
        {
            Federation federation = new Federation();
            federation.AddMember(country);
            federation.AddMember(tryWith);
            federation.SetLeader(country);
            manager.federations.Add(federation);
        }
        else if (country.federation.leader == country)
        {
            country.federation.AddMember(tryWith);
        }
    }


    void WageWar(Pays country)
    {
        if (Random.Range(0, 100) < 40)
        {
            return;
        }

        Pays pays;
        foreach (string key in country.relations.Keys)
        {
            pays = manager.GetCountry(key);
            if (pays != country && pays.provinces.Count != 0 && (country.lord != pays && pays.lord != country))
            {
                if (!country.relations[key].atWar &&
                Random.Range(0, 100) < (manager.player == pays ? 1 : 3))
                {
                    country.DeclareWarOnCountry(pays);
                    return;
                }
                else if (pays != manager.player && country.relations[key].atWar && Random.Range(0, 100) < 10)
                {
                    country.MakePeaceWithCountry(pays);
                    return;
                }
            }
        }
    }

    Vector3 GetRandomPosInsideCountry(Pays country)
    {
        Vector3 pos = country.provinces[Random.Range(0, country.provinces.Count)].center;
        pos.y = 0.3f;
        return pos;
    }

    void UnitMovement(Pays country)
    {
        foreach (Unit unit in country.units)
        {
            if (unit.target == unit.transform.position)
            {
                if (Random.Range(0, 50) == 0 && country.provinces.Count != 0)
                {
                    unit.target = GetRandomPosInsideCountry(country);
                }
                else if (Random.Range(0, 30) == 0 && country.atWarWith.Count != 0)
                {
                    Pays p = manager.GetCountry(country.atWarWith[Random.Range(0, country.atWarWith.Count)]);
                    if (p.provinces.Count <= 0)
                    {
                        country.MakePeaceWithCountry(p);
                    }
                    else
                    {
                        unit.target = GetRandomPosInsideCountry(p);
                    }

                }
            }
        }
    }

    void ChangeFocus(Pays country)
    {
        List<Focus> available = country.GetAvailableFocus();
        if (available.Count != 0)
        {
            country.ChangeFocus(available[Random.Range(0, available.Count)].id);
        }
    }
}
