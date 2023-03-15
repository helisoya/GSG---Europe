using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProvinceTab : GUITab
{
    [SerializeField] private TextMeshProUGUI provinceName;
    [SerializeField] private TMP_Dropdown provinceCores;

    private Province currentProvine;
    private List<string> current_cores;

    [SerializeField] private GameObject showIfOwner;
    [SerializeField] private GameObject showIfOwnerIsVassal;

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
    }

    public void SeizeProvince()
    {
        showIfOwner.SetActive(true);
        showIfOwnerIsVassal.SetActive(false);
        if (currentProvine.owner.provinces.Count == 1) CanvasWorker.instance.Show_CountryInfoPlayer();

        currentProvine.owner.relations[Manager.instance.player.ID].AddScore(-20);
        currentProvine.owner.RemoveProvince(currentProvine);
        currentProvine.owner.RefreshProvinces();
        Manager.instance.player.AddProvince(currentProvine, true);
    }


    public void ReleaseCountry()
    {
        if (current_cores.Count == 0)
        {
            return;
        }

        Manager manager = Manager.instance;
        string released = current_cores[provinceCores.value];

        manager.player.RemoveProvince(currentProvine);

        Pays releasedP = manager.GetCountry(released);

        releasedP.AddProvince(currentProvine);

        if (releasedP.provinces.Count == 1)
        {
            CanvasWorker.instance.UpdateRelations_ShortCut(manager.player, releasedP, 2);
        }
        CloseTab();
    }





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

    public void BuyUnit()
    {
        Manager manager = Manager.instance;
        if (manager.player.canBuyUnit && currentProvine.owner == manager.player
         && currentProvine.controller == manager.player && manager.player.AP >= 100)
        {
            manager.player.AP -= 100;
            currentProvine.SpawnUnitAtCity();
            CanvasWorker.instance.RefreshUtilityBar();
        }

    }

    public void AddRailroad()
    {
        if (!currentProvine.hasRailroad)
        {
            Manager.instance.AddRailRoadToProvince(currentProvine);
        }
    }
}
