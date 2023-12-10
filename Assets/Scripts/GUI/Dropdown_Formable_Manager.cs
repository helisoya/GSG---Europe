using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A custom dropdown that handles formable nations
/// </summary>
public class Dropdown_Formable_Manager : MonoBehaviour
{
    private Manager manager;
    [SerializeField] private FormableWorker formables;
    private List<FormableNation> current;
    [SerializeField] private PoliticalTab tab;
    [SerializeField] private TMP_Dropdown dropdown;


    void Start()
    {
        manager = Manager.instance;
    }

    /// <summary>
    /// Refresh the dropdown options for a country
    /// </summary>
    /// <param name="country">Target country</param>
    public void RefreshDropdown(Pays country)
    {
        List<string> choices = new List<string>();
        current = formables.GetFormableByCountry(country.ID);

        if (current.Count <= 0)
        {
            if (MapModes.currentMapMode == MapModes.MAPMODE.FORMABLE)
            {
                MapModes.currentMapMode = MapModes.MAPMODE.POLITICAL;
                manager.currentFormable = null;
            }
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        foreach (FormableNation formable in current)
        {
            choices.Add(formable.Name);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(choices);
        SelectNewFormable();
    }

    /// <summary>
    /// Select a new formable
    /// </summary>
    public void SelectNewFormable()
    {
        manager.currentFormable = current[dropdown.value];

        if (manager.currentFormable.CountryHasAllRequirement(manager.player))
        {
            transform.Find("Form").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Form").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Shows the required provinces to form the formable
    /// </summary>
    public void ShowMap()
    {
        MapModes.currentMapMode = MapModes.MAPMODE.FORMABLE;
        foreach (string key in manager.pays.Keys)
        {
            manager.GetCountry(key).RefreshProvinces();
        }
        tab.CloseTab();
    }

    /// <summary>
    /// Form the selected formable
    /// </summary>
    public void FormShortcut()
    {
        manager.formables.FormNation(manager.player, manager.currentFormable);
        tab.CloseTab();
        CanvasWorker.instance.Show_CountryInfoPlayer();
    }
}
