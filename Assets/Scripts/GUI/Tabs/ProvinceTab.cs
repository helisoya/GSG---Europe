using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Show informations about a province
/// </summary>
public class ProvinceTab : GUITab
{
    [SerializeField] private TextMeshProUGUI provinceName;
    [SerializeField] private TMP_Dropdown provinceCores;

    private Province currentProvine;
    private List<string> current_cores;

    [SerializeField] private GameObject showIfOwner;
    [SerializeField] private GameObject showIfOwnerIsVassal;
    [SerializeField] private Image ownerFlag;
    [SerializeField] private Image controllerFlag;

    /// <summary>
    /// Show a province's details
    /// </summary>
    /// <param name="prov">The province</param>
    public void ShowProvinceDetails(Province prov)
    {

        currentProvine = prov;
        OpenTab();
        provinceName.text = prov.name;

        current_cores = GetAllCountriesProvince(prov);

        List<string> l = new List<string>();

        foreach (string str in current_cores)
        {
            l.Add(Manager.instance.GetCountry(str).nom);
        }
        provinceCores.ClearOptions();
        provinceCores.AddOptions(l);

        showIfOwner.SetActive(prov.owner == Manager.instance.player);
        showIfOwnerIsVassal.SetActive(prov.owner.lord == Manager.instance.player);

        ownerFlag.sprite = prov.owner.currentFlag;
        if (prov.owner == prov.controller)
        {
            controllerFlag.gameObject.SetActive(false);
        }
        else
        {
            controllerFlag.gameObject.SetActive(true);
            controllerFlag.sprite = prov.controller.currentFlag;
        }
    }

    /// <summary>
    /// Show the informations on the current province's owner
    /// </summary>
    public void ShowOwner()
    {
        GameGUI.instance.Show_CountryInfo(currentProvine.owner);
    }

    /// <summary>
    /// Show the informations on the current province's controller
    /// </summary>
    public void ShowController()
    {
        GameGUI.instance.Show_CountryInfo(currentProvine.controller);
    }

    /// <summary>
    /// Seize the current province for the player
    /// </summary>
    public void SeizeProvince()
    {
        showIfOwner.SetActive(true);
        showIfOwnerIsVassal.SetActive(false);
        if (currentProvine.owner.provinces.Count == 1) GameGUI.instance.Show_CountryInfoPlayer();

        currentProvine.owner.relations[Manager.instance.player.ID].AddScore(-30);
        currentProvine.owner.RemoveProvince(currentProvine);
        currentProvine.owner.RefreshProvinces();
        Manager.instance.player.AddProvince(currentProvine, true);
    }

    /// <summary>
    /// Release a country from the current province
    /// </summary>
    public void ReleaseCountry()
    {
        if (current_cores.Count == 0)
        {
            return;
        }

        Manager manager = Manager.instance;
        string released = current_cores[provinceCores.value];

        manager.player.RemoveProvince(currentProvine);

        Country releasedP = manager.GetCountry(released);

        releasedP.AddProvince(currentProvine);

        if (releasedP.provinces.Count == 1)
        {
            GameGUI.instance.UpdateRelations_ShortCut(manager.player, releasedP, 2);
        }
        CloseTab();
    }




    /// <summary>
    /// Get a list of all countries who own this province as a core
    /// </summary>
    /// <param name="prov">The target province</param>
    /// <returns>The list of countries</returns>
    List<string> GetAllCountriesProvince(Province prov)
    {
        Manager manager = Manager.instance;
        List<string> l = new List<string>();

        foreach (string key in manager.pays.Keys)
        {
            if (key != manager.player.ID)
            {
                if (manager.GetCountry(key).cores.Contains(prov))
                {
                    l.Add(key);
                }
            }
        }
        return l;
    }

    /// <summary>
    /// Buy a unit inside the current province
    /// </summary>
    public void BuyUnit()
    {
        Manager manager = Manager.instance;
        if (manager.player.canBuyUnit && currentProvine.owner == manager.player
         && currentProvine.controller == manager.player && manager.player.AP >= 100)
        {
            manager.player.AP -= 100;
            currentProvine.SpawnUnitAtCity();
            GameGUI.instance.RefreshUtilityBar();
        }

    }

    /// <summary>
    /// Add a railroad to the current province
    /// </summary>
    public void AddRailroad()
    {
        if (!currentProvine.hasRailroad)
        {
            Manager.instance.AddRailRoadToProvince(currentProvine);
        }
    }
}
