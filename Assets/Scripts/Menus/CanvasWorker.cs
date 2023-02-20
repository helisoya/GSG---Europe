using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasWorker : MonoBehaviour
{
    [HideInInspector] public Manager manager;

    private string current_countryinfo = "000";


    private Province current_city;

    public Color32[] piechart_colors;

    public Image piechart_prefab;

    private List<string> current_cores;

    public List<string> focus_names;

    public static CanvasWorker instance;


    [Space]
    [Header("Country Infos")]
    [SerializeField] private GameObject holder_CountryInfo;
    [SerializeField] private Text infoName;
    [SerializeField] private Text infoGovernement;
    [SerializeField] private Text infoLeader;
    [SerializeField] private Text infoFocus;
    [SerializeField] private Image infoFlag;
    [SerializeField] private GameObject infoDiploRoot;
    [SerializeField] private Button infoDiploWar;
    [SerializeField] private Button infoDiploVassal;
    [SerializeField] private Button infoDiploPeace;


    [Space]
    [Header("Political Tab")]
    [SerializeField] private GameObject politicalRoot;
    [SerializeField] private Transform politicalGraph;
    [SerializeField] private PartyGUI[] politicalPartiesGUI;
    [SerializeField] private Text politicalElections;
    [SerializeField] private Text politicalAP;
    [SerializeField] private Text politicalGovernement;
    [SerializeField] private Text politicalGovernementDesc;
    [SerializeField] private Dropdown_Formable_Manager politicalFormables;

    [Space]
    [Header("Utilities")]
    [SerializeField] private GameObject utilityRoot;
    [SerializeField] private Text utilityDate;
    [SerializeField] private Text utilityAP;
    [SerializeField] private Text utilitySpeed;
    [SerializeField] private Text utilityUC;


    [Space]
    [Header("Province Details")]
    [SerializeField] private GameObject provinceDetailsRoot;
    [SerializeField] private Text provinceDetailsName;
    [SerializeField] private Dropdown provinceDetailsDropdown;

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
    [SerializeField] private SettingsMenu settingsRoot;
    [SerializeField] private FocusMenu focusMenu;


    void Start()
    {
        instance = this;
    }

    public void UpdateSpeedText(int speed)
    {
        utilitySpeed.text = "Speed : " + speed.ToString();
    }

    public void UpdateUtilityUnitCap()
    {
        utilityUC.text = manager.GetCountry(manager.player).units.Count + "/" + manager.GetCountry(manager.player).unitCap;
    }

    public void OpenSettingsMenu()
    {
        HideEverything();
        settingsRoot.gameObject.SetActive(true);
    }

    public void OpenPeaceDealTab(string side1, string side2)
    {
        Timer.instance.StopTime();
        HideEverything();
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


    public void Show_CountryInfo(string NEW)
    {
        current_countryinfo = NEW;
        UpdateInfo(manager.GetCountry(NEW));

        infoDiploRoot.SetActive(false);

        if (NEW != manager.player)
        { // Cas nation IA
            infoDiploRoot.SetActive(true);
            infoDiploPeace.onClick.RemoveAllListeners();
            infoDiploWar.onClick.RemoveAllListeners();
            infoDiploVassal.onClick.RemoveAllListeners();

            infoDiploPeace.onClick.AddListener(() =>
            {
                OpenPeaceDealTab(manager.player, NEW);
                //UpdateRelations_ShortCut(manager.player, NEW, 0);
                //Show_CountryInfo(NEW);
            });
            infoDiploWar.onClick.AddListener(() =>
            {
                UpdateRelations_ShortCut(manager.player, NEW, 1);
                Show_CountryInfo(NEW);
            });
            infoDiploVassal.onClick.AddListener(() =>
            {
                string old = manager.player;
                manager.player = NEW;
                manager.GetCountry(old).RefreshProvinces();
                manager.GetCountry(NEW).RefreshProvinces();
                Show_CountryInfo(NEW);
            });

            if (manager.GetCountry(manager.player).relations[NEW] == 1)
            { // Pays en guerre
                infoDiploPeace.gameObject.SetActive(true);
                infoDiploWar.gameObject.SetActive(false);
                infoDiploVassal.gameObject.SetActive(false);
            }
            else if (manager.GetCountry(manager.player).relations[NEW] == 2)
            { // Cas Vassal
                infoDiploPeace.gameObject.SetActive(false);
                infoDiploWar.gameObject.SetActive(false);
                infoDiploVassal.gameObject.SetActive(true);
            }
            else
            {
                infoDiploPeace.gameObject.SetActive(false);
                infoDiploWar.gameObject.SetActive(true);
                infoDiploVassal.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateInfo()
    {
        UpdateInfo(manager.GetCountry(current_countryinfo));
    }

    public void UpdateInfo(Pays country)
    {
        infoName.text = country.nom;
        infoGovernement.text = manager.GetGovernementName(country.Government_Form);
        infoLeader.text = country.leader.prenom + " " + country.leader.nom + "\n" + country.leader.age.ToString() + " years old";
        infoFocus.text = "National Focus : \n" +
            (country.currentFocus.Equals("NONE") ? "None" : manager.focus[country.currentFocus].focusName);
        infoFlag.GetComponent<Image>().sprite = country.currentFlag;
    }

    public void MakeGraph(Party[] values)
    {
        float total = 0f;
        float zrotation = 0f;
        for (int i = 0; i < values.Length; i++)
        {
            total += values[i].popularity;
        }
        for (int i = 0; i < values.Length; i++)
        {
            Image newWedge = Instantiate(piechart_prefab) as Image;
            newWedge.transform.SetParent(politicalGraph.transform, false);
            newWedge.color = piechart_colors[i];
            newWedge.fillAmount = values[i].popularity / total;
            newWedge.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, zrotation));
            zrotation -= newWedge.fillAmount * 360f;
        }
    }


    public void Update_Date()
    {
        utilityDate.text = manager.GetDate();
    }

    public void OpenPolitiqueTab()
    {
        manager.GetComponent<Timer>().StopTime();
        utilityRoot.SetActive(false);
        holder_CountryInfo.SetActive(false);
        provinceDetailsRoot.SetActive(false);
        politicalRoot.SetActive(true);

        for (int i = 0; i < politicalGraph.childCount; i++)
        {
            Destroy(politicalGraph.GetChild(i).gameObject);
        }

        MakeGraph(manager.GetCountry(manager.player).parties);

        for (int i = 0; i < piechart_colors.Length; i++)
        {
            politicalPartiesGUI[i].image.color = piechart_colors[i];
            politicalPartiesGUI[i].text.text = manager.GetCountry(manager.player).parties[i].partyName;
        }

        if (manager.GetCountry(manager.player).date_elections == -1)
        {
            politicalElections.text = "Next Elections : Never";
        }
        else
        {
            politicalElections.text = "Next Elections : " + manager.GetCountry(manager.player).date_elections.ToString();
        }
        politicalAP.text = "Admin Power : " + manager.GetCountry(manager.player).AP.ToString();
        politicalGovernement.text = manager.GetGovernementName(manager.GetCountry(manager.player).Government_Form);
        politicalGovernementDesc.text = manager.GetGovernementDesc(manager.GetCountry(manager.player).Government_Form);
        politicalFormables.RefreshDropdown(manager.GetCountry(manager.player));
    }

    public void ClosePolitique()
    {
        manager.gameObject.GetComponent<Timer>().ResumeTime();
        utilityRoot.SetActive(true);
        holder_CountryInfo.SetActive(true);
        provinceDetailsRoot.SetActive(false);
        politicalRoot.SetActive(false);
    }


    public void Event(string Title, string Desc)
    {
        manager.gameObject.GetComponent<Timer>().StopTime();
        HideEverything();
        eventsRoot.SetActive(true);
        eventsTitle.text = Title;
        eventsDesc.text = Desc;
    }

    public void Hide_Event()
    {
        manager.GetComponent<Timer>().ResumeTime();
        manager.GetCountry(manager.player).Reset_Flag();
        Show_CountryInfo(manager.player);
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

    public void Add_Popularity_ShortCut(int index)
    {

        if (manager.GetCountry(manager.player).AP < 10)
        {
            return;
        }
        manager.GetCountry(manager.player).AP -= 10;
        politicalAP.text = "Admin Power : " + manager.GetCountry(manager.player).AP.ToString();
        UpdatePPBar();

        manager.GetCountry(manager.player).Add_Popularity(index, 5);

        for (int i = 0; i < politicalGraph.childCount; i++)
        {
            Destroy(politicalGraph.GetChild(i).gameObject);
        }
        MakeGraph(manager.GetCountry(manager.player).parties);
    }

    public void UpdateRelations_ShortCut(string A, string B, int st)
    { //Met a jour A selon st, puis B en logique
        switch (st)
        {
            case 0:
                manager.GetCountry(A).MakePeaceWithCountry(B);
                if (manager.GetCountry(B).provinces.Count == 0)
                {
                    Show_CountryInfo(A);
                }
                break;

            case 1:
                manager.GetCountry(A).DeclareWarOnCountry(B);
                break;

            case 2:
                List<string> keys = new List<string>();
                foreach (string key in manager.GetCountry(B).relations.Keys)
                {
                    keys.Add(key);
                }
                foreach (string pays in keys)
                {
                    manager.GetCountry(B).relations[pays] = 0;
                }

                manager.GetCountry(A).relations[B] = st;
                manager.GetCountry(B).relations[A] = 3;
                manager.GetCountry(B).CopyCat(A);
                manager.GetCountry(B).MimicColor(A);
                manager.GetCountry(B).vassal = true;
                break;

            case 3:
                foreach (string pays in manager.GetCountry(A).relations.Keys)
                {
                    manager.GetCountry(A).relations[pays] = 0;
                }

                manager.GetCountry(A).relations[B] = st;
                manager.GetCountry(B).relations[A] = 2;
                manager.GetCountry(A).CopyCat(B);
                manager.GetCountry(A).MimicColor(B);
                manager.GetCountry(A).vassal = true;
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

        manager.GetCountry(manager.player).RemoveProvince(current_city);
        manager.GetCountry(released).AddProvince(current_city);

        if (manager.GetCountry(released).provinces.Count == 1)
        {
            UpdateRelations_ShortCut(manager.player, released, 2);
        }
        provinceDetailsRoot.SetActive(false);
    }


    List<string> GetAllCountriesProvince(Province prov)
    {
        List<string> l = new List<string>();

        foreach (string key in manager.pays.Keys)
        {
            if (key != manager.player)
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
        if (manager.GetCountry(manager.player).canBuyUnit && current_city.owner == manager.player
         && current_city.controller == manager.player && manager.GetCountry(manager.player).AP >= 100)
        {
            manager.GetCountry(manager.player).AP -= 100;
            current_city.SpawnUnitAtCity();
            UpdatePPBar();
        }

    }

    public void UpdateFocus()
    {
        focusMenu.ShowFocusMenu();
    }

    public void HideEverything()
    {
        holder_CountryInfo.SetActive(false);
        politicalRoot.SetActive(false);
        utilityRoot.SetActive(false);
        provinceDetailsRoot.SetActive(false);
        eventsRoot.SetActive(false);
        settingsRoot.gameObject.SetActive(false);
        focusMenu.gameObject.SetActive(false);
        peaceDealRoot.SetActive(false);
    }

    public void ShowDefault()
    {
        holder_CountryInfo.SetActive(true);
        utilityRoot.SetActive(true);
    }

    public void UpdatePPBar()
    {
        utilityAP.text = "AP : " + manager.GetCountry(manager.player).AP.ToString();
    }

}


[System.Serializable]
public class PartyGUI
{
    public Image image;
    public Text text;
}