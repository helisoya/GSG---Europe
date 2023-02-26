using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropdown_Formable_Manager : MonoBehaviour
{
    public Manager manager;

    public FormableWorker formables;

    private List<FormableNation> current;

    public CanvasWorker canvas;

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
        transform.Find("List").GetComponent<UnityEngine.UI.Dropdown>().ClearOptions();
        transform.Find("List").GetComponent<UnityEngine.UI.Dropdown>().AddOptions(choices);
        SelectNewFormable();
    }


    public void SelectNewFormable()
    {
        manager.currentFormable = current[transform.Find("List").GetComponent<UnityEngine.UI.Dropdown>().value];

        if (manager.currentFormable.CountryHasAllRequirement(manager.player))
        {
            transform.Find("Form").gameObject.SetActive(true);
        }
        else
        {
            transform.Find("Form").gameObject.SetActive(false);
        }
    }

    public void ShowMap()
    {
        MapModes.currentMapMode = MapModes.MAPMODE.FORMABLE;
        foreach (string key in manager.pays.Keys)
        {
            manager.GetCountry(key).RefreshProvinces();
        }
        canvas.ClosePolitique();
    }

    public void FormShortcut()
    {
        manager.formables.FormNation(manager.player, manager.currentFormable);
        canvas.ClosePolitique();
        canvas.Show_CountryInfo(manager.player);
    }
}
