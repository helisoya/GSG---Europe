using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The manager handles the game logic
/// </summary>
public class Manager : MonoBehaviour
{
    public static Manager instance;
    public Pays player = null;

    private Gouvernements gouvs_data;

    public Dictionary<string, Pays> pays;

    private int mois = 0;
    public int an = 2030;

    public List<Unit> selected_unit = new List<Unit>();

    public FormableWorker formables;

    public FormableNation currentFormable;

    public bool picked = false;

    public bool inPeaceDeal = false;

    public Pays peaceDealSide1;
    public Pays peaceDealSide2;

    public List<Federation> federations;
    public List<Province> provincesToBeTakenInPeaceDeal;
    private Dictionary<int, Province> provinces;

    private Dictionary<string, Dictionary<string, Focus>> focuses;
    private Dictionary<string, Culture> cultures;
    public Dictionary<string, GameEvent> events;
    [SerializeField] private GameObject prefabRailroad;
    private Dictionary<string, Railroad> railroads;

    private List<Unit> allUnits;

    private Graph m_graph;
    public Graph movementGraph
    {
        get
        {
            return m_graph;
        }
    }

    [Header("Parser")]
    public GameObject prefabProvince;
    public Transform provinceParent;
    [SerializeField] private GameObject loadingRoot;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image loadingImg;
    private Coroutine loading;

    public bool isLoading
    {
        get { return loading != null; }
    }


    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Intialize the game
    /// </summary>
    void Start()
    {
        loading = null;
        federations = new List<Federation>();
        allUnits = new List<Unit>();

        loading = StartCoroutine(LoadFiles());

        if (PlayerPrefs.HasKey("clouds"))
        {
            transform.Find("Clouds").gameObject.SetActive((PlayerPrefs.GetString("clouds") == "true"));
        }
        else
        {
            PlayerPrefs.SetString("clouds", "false");
            transform.Find("Clouds").gameObject.SetActive(false);
        }
        if (PlayerPrefs.HasKey("sun"))
        {
            GameObject.Find("Directional Light").GetComponent<Sun>().enabled = (PlayerPrefs.GetString("sun") == "true");
        }
        else
        {
            PlayerPrefs.SetString("sun", "false");
            GameObject.Find("Directional Light").GetComponent<Sun>().enabled = false;
        }
    }

    /// <summary>
    /// Loads the JSON files
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator LoadFiles()
    {
        loadingRoot.SetActive(true);
        loadingImg.fillAmount = 0;
        loadingText.text = "Loading Provinces";
        yield return new WaitForEndOfFrame();
        provinces = Parser.ParseProvinces();
        railroads = new Dictionary<string, Railroad>();
        m_graph = new Graph();
        foreach (Province prov in provinces.Values)
        {
            m_graph.nodes.Add(prov);
        }

        loadingImg.fillAmount = 1f / 7f;
        loadingText.text = "Loading Focus";
        yield return new WaitForEndOfFrame();
        focuses = Parser.ParseFocus();

        loadingImg.fillAmount = 2f / 7f;
        loadingText.text = "Loading Events";
        yield return new WaitForEndOfFrame();
        events = Parser.ParseEvents();

        loadingImg.fillAmount = 3f / 7f;
        loadingText.text = "Loading Cultures";
        yield return new WaitForEndOfFrame();
        cultures = Parser.ParseCultures(focuses);


        loadingImg.fillAmount = 4f / 7f;
        loadingText.text = "Loading Countries";
        yield return new WaitForEndOfFrame();
        gouvs_data = GetComponent<Gouvernements>();
        pays = Parser.ParsePays(cultures);

        loadingImg.fillAmount = 5f / 7f;
        loadingText.text = "Loading Formables";
        yield return new WaitForEndOfFrame();
        formables = GetComponent<FormableWorker>();
        formables.SetFormables(Parser.ParseFormables(provinces));


        loadingImg.fillAmount = 6f / 7f;
        loadingText.text = "Loading History";
        yield return new WaitForEndOfFrame();
        Parser.ParseHistory(provinces, pays, "history");

        Destroy(loadingRoot);

        CanvasWorker.instance.manager = this;

        CountryPicker.instance.Init();

        loading = null;
    }

    /// <summary>
    /// Register a unit
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RegisterUnit(Unit unit)
    {
        allUnits.Add(unit);
    }

    /// <summary>
    /// Unregister a unit
    /// </summary>
    /// <param name="unit">The unit</param>
    public void UnRegisterUnit(Unit unit)
    {
        allUnits.Remove(unit);
    }

    /// <summary>
    /// Adds a railroad to a province (WIP)
    /// </summary>
    /// <param name="province">The province</param>
    public void AddRailRoadToProvince(Province province)
    {
        province.hasRailroad = true;
        foreach (Province prov in province.adjacencies)
        {
            if (!prov.hasRailroad) continue;

            bool val = province.id.CompareTo(prov.id) == -1;
            string key = val ? prov.id + "_" + province.id : province.id + "_" + prov.id;
            if (!railroads.ContainsKey(key))
            {
                Railroad rail = Instantiate(prefabRailroad).GetComponent<Railroad>();
                rail.Init(province.id, prov.id);
                railroads.Add(key, rail);
            }
        }
    }

    /// <summary>
    /// End a peace deal
    /// </summary>
    /// <param name="vassalizeB">Is B vassalized ?</param>
    public void EndPeaceDeal(bool vassalizeB)
    {
        Pays A = peaceDealSide1;
        Pays B = peaceDealSide2;
        inPeaceDeal = false;
        foreach (Province province in provincesToBeTakenInPeaceDeal)
        {
            B.RemoveProvince(province);
            A.AddProvince(province, false);
        }

        foreach (Province province in A.provinces)
        {
            if (province.controller == B) province.SetController(A);
        }

        foreach (Province province in B.provinces)
        {
            if (province.controller == A) province.SetController(B);
        }


        Province candidate;
        if (provincesToBeTakenInPeaceDeal.Count != 0)
        {
            candidate = provincesToBeTakenInPeaceDeal[Random.Range(0, provincesToBeTakenInPeaceDeal.Count)];
        }
        else
        {
            candidate = A.provinces[Random.Range(0, A.provinces.Count)];
        }
        A.RemoveUnitsFromCountry(candidate, B.ID);

        if (B.provinces.Count != 0)
        {
            candidate = B.provinces[Random.Range(0, B.provinces.Count)];
            B.RemoveUnitsFromCountry(candidate, A.ID);
        }


        A.MakePeaceWithCountry(B);

        if (vassalizeB)
        {
            CanvasWorker.instance.UpdateRelations_ShortCut(A, B, 2);
        }

        RefreshMap();
        CanvasWorker.instance.Show_CountryInfo(player);
        Timer.instance.ResumeTime();
    }

    /// <summary>
    /// Refresh the map GFX
    /// </summary>
    /// <param name="value">Show map GFX</param>
    public void UpdateMapGFXSeen(bool value)
    {
        foreach (Unit unit in allUnits)
        {
            unit.UpdateIsSeen(value);
        }

        foreach (Railroad railroad in railroads.Values)
        {
            railroad.UpdateIsSeen(value);
        }
    }

    /// <summary>
    /// Start the game
    /// </summary>
    /// <param name="chosenCountry">Chosen country</param>
    public void StartGame(Pays chosenCountry)
    {
        player = chosenCountry;
        picked = true;
        CanvasWorker.instance.Show_CountryInfo(player);

        List<string> toDelete = new List<string>();

        foreach (Pays p in pays.Values)
        {
            if (!p.DestroyIfNotSelected || p.ID == player.ID)
            {
                p.RandomizeLeader();
                p.RefreshProvinces();
            }
            else
            {
                toDelete.Add(p.ID);
                foreach (Province prov in p.provinces)
                {
                    prov.SetAsSeaProvince();
                }
            }
        }

        foreach (string del in toDelete)
        {
            pays.Remove(del);
        }

        AddMonth();
        CanvasWorker.instance.ShowDefault();
        CanvasWorker.instance.UpdateInfo();
    }

    /// <summary>
    /// Gets a country
    /// </summary>
    /// <param name="ID">Country's ID</param>
    /// <returns>A country</returns>
    public Pays GetCountry(string ID)
    {
        return pays[ID];
    }

    /// <summary>
    /// Gets a province
    /// </summary>
    /// <param name="ID">Province's ID</param>
    /// <returns>A province</returns>
    public Province GetProvince(int ID)
    {
        return provinces[ID];
    }

    /// <summary>
    /// Gets a governement type description
    /// </summary>
    /// <param name="ID">Governement type ID</param>
    /// <returns>The governement type description</returns>
    public string GetGovernementDesc(int ID)
    {
        return gouvs_data.descs[ID];
    }

    /// <summary>
    /// Gets a governement type name
    /// </summary>
    /// <param name="ID">Governement type ID</param>
    /// <returns>The governement type name</returns>
    public string GetGovernementName(int ID)
    {
        return gouvs_data.noms[ID];
    }

    /// <summary>
    /// Increment the month
    /// </summary>
    void AddMonth()
    {
        mois += 1;
        if (mois > 12)
        {
            mois = 1;
            an += 1;
            foreach (Pays key in pays.Values)
            {
                key.NewYear();
            }
            foreach (Federation federation in federations)
            {
                federation.CheckYearElection();
            }
        }
    }

    /// <summary>
    /// Handle next turn
    /// </summary>
    public void NextTurn()
    {
        AddMonth();
        foreach (Pays country in pays.Values)
        {
            if (country.provinces.Count > 0)
            {
                country.DP += country.DP_PerMonth;
                country.AP += country.AP_PerMonth;
                country.IncrementFocus();

                if (country != player)
                {
                    GetComponent<AI>().IA(country);
                }
            }
        }
        CanvasWorker.instance.RefreshUtilityBar();
        CanvasWorker.instance.UpdateInfo();
    }

    /// <summary>
    /// Get current date
    /// </summary>
    /// <returns>the current date</returns>
    public string GetDate()
    {
        string[] values = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        return values[mois - 1] + " " + an.ToString();
    }

    /// <summary>
    /// Refresh the entire map
    /// </summary>
    public void RefreshMap()
    {
        foreach (string key in pays.Keys)
        {
            GetCountry(key).RefreshProvinces();
        }
    }


    void Update()
    {
        if (!picked)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CanvasWorker.instance.OpenSettingsMenu();
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            player.DP += 99999;
            player.AP += 99999;
        }
#endif
    }

}
