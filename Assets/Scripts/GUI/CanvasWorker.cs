using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasWorker : MonoBehaviour
{
    [HideInInspector] public Manager manager;

    public static CanvasWorker instance;





    [Space]
    [Header("Utilities")]
    [SerializeField] private GameObject utilityRoot;
    [SerializeField] private TextMeshProUGUI utilityDate;
    [SerializeField] private TextMeshProUGUI utilityAP;
    [SerializeField] private TextMeshProUGUI utilitySpeed;
    [SerializeField] private TextMeshProUGUI utilityUC;


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
    [SerializeField] private ProvinceTab provinceTab;


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
        provinceTab.ShowProvinceDetails(c);
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
        if (provinceTab.isOpen) provinceTab.CloseTab();
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


