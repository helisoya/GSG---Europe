using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasWorker : MonoBehaviour
{
    [HideInInspector] public Manager manager;

    private Province current_city;
    private List<string> current_cores;


    public static CanvasWorker instance;





    [Space]
    [Header("Utilities")]
    [SerializeField] private GameObject utilityRoot;
    [SerializeField] private TextMeshProUGUI utilityDate;
    [SerializeField] private TextMeshProUGUI utilityAP;
    [SerializeField] private TextMeshProUGUI utilitySpeed;
    [SerializeField] private TextMeshProUGUI utilityUC;


    [Space]
    [Header("Province Details")]
    [SerializeField] private GameObject provinceDetailsRoot;
    [SerializeField] private TextMeshProUGUI provinceDetailsName;
    [SerializeField] private TMP_Dropdown provinceDetailsDropdown;

    [Space]
    [Header("Events")]
    [SerializeField] private GameObject eventsRoot;
    [SerializeField] private Text eventsTitle;
    [SerializeField] private Text eventsDesc;
    [SerializeField] private Button[] eventsButtons;

    [Space]
    [Header("Peace Deal")]
    [SerializeField] private GameObject peaceDealRoot;
    [SerializeField] private Toggle peaceDealVassal;

    [Space]
    [Header("Other Menus")]
    [SerializeField] private PauseTab pauseTab;
    [SerializeField] private FocusTab focusMenu;
    [SerializeField] private PoliticalTab politicalTab;
    [SerializeField] private CountryInfoTab countryInfoTab;


    void Awake()
    {
        instance = this;
    }

    public void UpdateSpeedText(int speed)
    {
        utilitySpeed.text = "Speed : " + speed.ToString();
    }

    public void UpdateUtilityUnitCap()
    {
        utilityUC.text = manager.player.units.Count + "/" + manager.player.unitCap;
    }

    public void OpenSettingsMenu()
    {
        if (pauseTab.isOpen) return;
        HideEverything();
        pauseTab.OpenTab();
    }

    public void OpenPeaceDealTab(string side1, string side2)
    {
        HideEverything();
        Timer.instance.StopTime();
        manager.inPeaceDeal = true;
        manager.peaceDealSide1 = side1;
        manager.peaceDealSide2 = side2;
        manager.provincesToBeTakenInPeaceDeal = new List<Province>();
        peaceDealRoot.SetActive(true);
        manager.RefreshMap();
    }

    public void EndPeaceDeal()
    {
        HideEverything();
        ShowDefault();
        manager.EndPeaceDeal(peaceDealVassal.isOn);
    }

    public void PeaceDealProvinceSelection(Province prov)
    {
        if (!manager.provincesToBeTakenInPeaceDeal.Remove(prov))
        {
            manager.provincesToBeTakenInPeaceDeal.Add(prov);
        }
        prov.RefreshColor();
    }

    public void Show_CountryInfoPlayer()
    {
        countryInfoTab.Show_CountryInfo(manager.player);
    }

    public void Show_CountryInfo(Pays country)
    {
        countryInfoTab.Show_CountryInfo(country);
    }



    public void UpdateInfo()
    {
        countryInfoTab.UpdateInfo();
    }




    public void Update_Date()
    {
        utilityDate.text = manager.GetDate();
    }




    public void Event(string Title, string Desc)
    {
        HideEverything();
        Timer.instance.StopTime();
        eventsRoot.SetActive(true);
        eventsTitle.text = Title;
        eventsDesc.text = Desc;
    }

    public void Hide_Event()
    {
        Timer.instance.ResumeTime();

        manager.player.Reset_Flag();
        countryInfoTab.Show_CountryInfo(manager.player);
        eventsRoot.SetActive(false);
        ShowDefault();
        Reset_Bindings_Event();
    }

    void Reset_Bindings_Event()
    {
        for (int i = 1; i < eventsButtons.Length; i++)
        {
            eventsButtons[i].onClick.RemoveAllListeners();
            eventsButtons[i].gameObject.SetActive(false);
        }
    }



    public void UpdateRelations_ShortCut(Pays A, Pays B, int st)
    { //Met a jour A selon st, puis B en logique

        List<string> keys;
        switch (st)
        {
            case 0:
                A.MakePeaceWithCountry(B);
                if (B.provinces.Count == 0)
                {
                    countryInfoTab.Show_CountryInfo(A);
                }
                break;

            case 1:
                A.DeclareWarOnCountry(B);
                break;

            case 2:

                keys = new List<string>(B.relations.Keys);
                foreach (string key in keys)
                {
                    B.relations[key] = 0;
                }

                A.relations[B.ID] = st;
                B.relations[A.ID] = 3;
                B.CopyCat(A);
                B.MimicColor(A);
                B.vassal = true;
                break;

            case 3:
                keys = new List<string>(A.relations.Keys);
                foreach (string pays in keys)
                {
                    A.relations[pays] = 0;
                }

                A.relations[B.ID] = st;
                B.relations[A.ID] = 2;
                A.CopyCat(B);
                A.MimicColor(B);
                A.vassal = true;
                break;
        }

    }


    public void ShowBuyUnit(Province c)
    {
        current_city = c;
        provinceDetailsRoot.SetActive(true);
        provinceDetailsName.text = c.name;

        current_cores = GetAllCountriesProvince(c);

        List<string> l = new List<string>();

        foreach (string str in current_cores)
        {
            l.Add(manager.GetCountry(str).nom);
        }
        provinceDetailsDropdown.ClearOptions();
        provinceDetailsDropdown.AddOptions(l);
    }

    public void ReleaseCountry()
    {
        if (current_cores.Count == 0)
        {
            return;
        }
        string released = current_cores[provinceDetailsDropdown.value];

        manager.player.RemoveProvince(current_city);

        Pays releasedP = manager.GetCountry(released);

        releasedP.AddProvince(current_city);

        if (releasedP.provinces.Count == 1)
        {
            UpdateRelations_ShortCut(manager.player, releasedP, 2);
        }
        provinceDetailsRoot.SetActive(false);
    }


    List<string> GetAllCountriesProvince(Province prov)
    {
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
        if (manager.player.canBuyUnit && current_city.owner == manager.player
         && current_city.controller == manager.player && manager.player.AP >= 100)
        {
            manager.player.AP -= 100;
            current_city.SpawnUnitAtCity();
            UpdatePPBar();
        }

    }

    public void UpdateFocus()
    {
        if (focusMenu.isOpen)
        {
            focusMenu.ShowFocusMenu();
        }
    }

    public void OpenFocusTab()
    {
        focusMenu.OpenTab();
    }

    public void HideEverything()
    {
        if (countryInfoTab.isOpen) countryInfoTab.CloseTab();
        utilityRoot.SetActive(false);
        provinceDetailsRoot.SetActive(false);
        eventsRoot.SetActive(false);

        if (pauseTab.isOpen) pauseTab.CloseTab();
        if (focusMenu.isOpen) focusMenu.CloseTab();
        if (politicalTab.isOpen) politicalTab.CloseTab();
        peaceDealRoot.SetActive(false);
    }

    public void ShowDefault()
    {
        utilityRoot.SetActive(true);
    }

    public void UpdatePPBar()
    {
        utilityAP.text = "AP : " + manager.player.AP.ToString();
    }

}


