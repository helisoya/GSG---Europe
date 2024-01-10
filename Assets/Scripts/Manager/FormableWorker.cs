using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the various formables of the game
/// </summary>
public class FormableWorker : MonoBehaviour
{
    public List<FormableNation> FormableNations = new List<FormableNation>();

    private Manager manager;

    void Start()
    {
        manager = GetComponent<Manager>();
    }

    /// <summary>
    /// Sets the formables that can be formed
    /// </summary>
    /// <param name="list">The formables</param>
    public void SetFormables(List<FormableNation> list)
    {
        FormableNations = list;
    }

    /// <summary>
    /// Find the formable that a country can form
    /// </summary>
    /// <param name="country">The target country</param>
    /// <returns>A list of formables</returns>
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

    /// <summary>
    /// Form a formable nation
    /// </summary>
    /// <param name="country">Target country</param>
    /// <param name="formable">Target formable</param>
    public void FormNation(Country country, FormableNation formable)
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

    /// <summary>
    /// Find a formable nation by name
    /// </summary>
    /// <param name="name">Formable's name</param>
    /// <returns></returns>
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
