using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormableWorker : MonoBehaviour
{
    public List<FormableNation> FormableNations = new List<FormableNation>();

    private Manager manager;

    void Start()
    {
        manager = GetComponent<Manager>();
    }

    public void SetFormables(List<FormableNation> list)
    {
        FormableNations = list;
    }

    public List<FormableNation> GetFormableByCountry(string country)
    {
        List<FormableNation> list = new List<FormableNation>();
        foreach (FormableNation formable in FormableNations)
        {
            if ((formable.contestants.Contains(country) || formable.contestants.Contains(manager.GetCountry(country).nom)) && !formable.alreadyFormed)
            {
                list.Add(formable);
            }
        }
        return list;
    }


    public void FormNation(Pays country, FormableNation formable)
    {
        formable.alreadyFormed = true;
        country.nom = formable.Name;
        country.cosmeticID = formable.id;
        manager.currentFormable = null;

        foreach (Unit unit in country.units)
        {
            unit.UpdateFlag();
        }
        foreach (Province prov in formable.required)
        {
            if (!country.cores.Contains(prov))
            {
                country.cores.Add(prov);
            }
        }
    }

    public FormableNation GetFormableNationFromName(string name)
    {
        foreach (FormableNation formable in FormableNations)
        {
            if (formable.Name == name)
            {
                return formable;
            }
        }
        return null;
    }

}
