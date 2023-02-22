using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public Manager manager;


    public void IA(Pays country)
    {

        if (Random.Range(0, 5) != 0)
        {
            return;
        }

        if (!country.hasTech_Naval)
        {
            country.hasTech_Naval = true;
        }

        BuyUnit(country);
        if (!country.vassal)
        {
            WageWar(country);
            BoostIdeologie(country);
        }
        UnitMovement(country);
        if (country.currentFocusTime == 0)
        {
            ChangeFocus(country);
        }
    }


    void BuyUnit(Pays country)
    {
        if (country.provinces == null || !country.canBuyUnit || country.provinces.Count == 0)
        {
            return;
        }
        foreach (Province prov in country.provinces)
        {
            if (prov.controller == country && Random.Range(0, 50) == 0 && country.AP >= 100)
            {
                country.AP -= 100;
                prov.GetComponent<Province>().SpawnUnitAtCity();
            }
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
            if (pays != country && pays.provinces.Count != 0 && country.relations[key] < 2)
            {
                if (country.relations[key] == 0 &&
                Random.Range(0, 100) < (manager.player == pays ? 1 : 3))
                {
                    country.DeclareWarOnCountry(pays);
                    return;
                }
                else if (pays != manager.player && country.relations[key] == 1 && Random.Range(0, 100) < 10)
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
                    Pays p = manager.GetCountry(new List<string>(country.atWarWith.Keys)[Random.Range(0, country.atWarWith.Keys.Count)]);
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


    void BoostIdeologie(Pays country)
    {
        if (Random.Range(0, 100) < 30)
        {
            country.Add_Popularity(Random.Range(0, country.parties.Length), Random.Range(5, 20));
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
