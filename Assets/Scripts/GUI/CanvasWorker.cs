using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasWorker : MonoBehaviour
{
    [HideInInspector] public Manager manager;

    private Pays current_countryinfo = null;


    private Province current_city;

    public Color32[] piechart_colors;

    public Image piechart_prefab;

    private List<string> current_cores;


    public static CanvasWorker instance;


    [Space]
    [Header("Country Infos")]
    [SerializeField] private GameObject holder_CountryInfo;
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoGovernement;
    [SerializeField] private TextMeshProUGUI infoLeader;
    [SerializeField] private TextMeshProUGUI infoFocus;
    [SerializeField] private Image infoFocusFill;
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
    [SerializeField] private TextMeshProUGUI utilityDate;
    [SerializeField] private TextMeshProUGUI utilityAP;
    [SerializeField] private TextMeshProUGUI utilitySpeed;
    [SerializeField] private TextMeshProUGUI utilityUC;


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
        utilityUC.text = manager.player.units.Count + "/" + manager.player.unitCap;
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

    public void Show_CountryInfoPlayer()
    {
        Show_CountryInfo(manager.player);
    }


    public void Show_CountryInfo(Pays NEW)
    {
        infoFlag.transform.parent.gameObject.SetActive(true);
        current_countryinfo = NEW;
        UpdateInfo(NEW);

        infoDiploRoot.SetActive(false);

        if (NEW != manager.player)
        { // Cas nation IA
            infoDiploRoot.SetActive(true);
            infoDiploPeace.onClick.RemoveAllListeners();
            infoDiploWar.onClick.RemoveAllListeners();
            infoDiploVassal.onClick.RemoveAllListeners();

            infoDiploPeace.onClick.AddListener(() =>
            {
                OpenPeaceDealTab(manager.player.ID, NEW.ID);
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
                Pays old = manager.player;
                manager.player = NEW;
                old.RefreshProvinces();
                NEW.RefreshProvinces();
                Show_CountryInfo(NEW);
            });

            if (manager.player.relations[NEW.ID] == 1)
            { // Pays en guerre
                infoDiploPeace.gameObject.SetActive(true);
                infoDiploWar.gameObject.SetActive(false);
                infoDiploVassal.gameObject.SetActive(false);
            }
            else if (manager.player.relations[NEW.ID] == 2)
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
        UpdateInfo(current_countryinfo);
    }

    public void UpdateInfo(Pays country)
    {
        infoName.text = country.nom;
        infoGovernement.text = manager.GetGovernementName(country.Government_Form);
        infoLeader.text = country.leader.prenom + " " + country.leader.nom + "\n" + country.leader.age.ToString() + " years old";
        infoFocus.text = (country.currentFocus.Equals("NONE") ? "Doing Nothing" : country.focusTree[country.currentFocus].focusName);
        infoFocusFill.fillAmount = (country.currentFocus.Equals("NONE") ? 0 : country.currentFocusTime / (float)country.maxFocusTime);
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

        MakeGraph(manager.player.parties);

        for (int i = 0; i < piechart_colors.Length; i++)
        {
            politicalPartiesGUI[i].image.color = piechart_colors[i];
            politicalPartiesGUI[i].text.text = manager.player.parties[i].partyName;
        }

        if (manager.player.date_elections == -1)
        {
            politicalElections.text = "Next Elections : Never";
        }
        else
        {
            politicalElections.text = "Next Elections : " + manager.player.date_elections.ToString();
        }
        politicalAP.text = "Admin Power : " + manager.player.AP.ToString();
        politicalGovernement.text = manager.GetGovernementName(manager.player.Government_Form);
        politicalGovernementDesc.text = manager.GetGovernementDesc(manager.player.Government_Form);
        politicalFormables.RefreshDropdown(manager.player);
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

        manager.player.Reset_Flag();
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

        if (manager.player.AP < 10)
        {
            return;
        }
        manager.player.AP -= 10;
        politicalAP.text = "Admin Power : " + manager.player.AP.ToString();
        UpdatePPBar();

        manager.player.Add_Popularity(index, 5);

        for (int i = 0; i < politicalGraph.childCount; i++)
        {
            Destroy(politicalGraph.GetChild(i).gameObject);
        }
        MakeGraph(manager.player.parties);
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
                    Show_CountryInfo(A);
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
        utilityAP.text = "AP : " + manager.player.AP.ToString();
    }

}


[System.Serializable]
public class PartyGUI
{
    public Image image;
    public Text text;
}