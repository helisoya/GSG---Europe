using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    public Pays player = null;

    private Gouvernements gouvs_data;

    public Dictionary<string, Pays> pays;

    private int mois = 0;

    public int an = 2030;


    public List<GameObject> selected_unit = new List<GameObject>();



    public FormableWorker formables;

    public FormableNation currentFormable;

    public bool picked = false;

    public bool inPeaceDeal = false;

    public string peaceDealSide1;
    public string peaceDealSide2;
    public List<Province> provincesToBeTakenInPeaceDeal;

    private Dictionary<string, Province> provinces;

    public Dictionary<string, Dictionary<string, Focus>> focuses;
    public Dictionary<string, Culture> cultures;
    public Dictionary<string, GameEvent> events;
    public GameObject prefabRailroad;
    private Dictionary<string, Railroad> railroads;

    [Header("Parser")]
    public GameObject prefabProvince;
    public Transform provinceParent;
    [SerializeField] private GameObject loadingRoot;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Image loadingImg;
    [SerializeField] private CountryPicker picker;
    private Coroutine loading;

    public bool isLoading
    {
        get { return loading != null; }
    }


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        loading = null;

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


    IEnumerator LoadFiles()
    {
        loadingRoot.SetActive(true);
        loadingImg.fillAmount = 0;
        loadingText.text = "Loading Provinces";
        yield return new WaitForEndOfFrame();
        provinces = Parser.ParseProvinces();
        railroads = new Dictionary<string, Railroad>();

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

        picker.Init();

        loading = null;
    }


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

    public void EndPeaceDeal(bool vassalizeB)
    {
        Pays A = GetCountry(peaceDealSide1);
        Pays B = GetCountry(peaceDealSide2);
        inPeaceDeal = false;
        foreach (Province province in provincesToBeTakenInPeaceDeal)
        {
            B.RemoveProvince(province);
            A.AddProvince(province, false);
        }

        foreach (Province province in A.provinces)
        {
            if (province.controller == B) province.controller = A;
        }

        foreach (Province province in B.provinces)
        {
            if (province.controller == A) province.controller = B;
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


    public void UpdateMapGFXSeen(bool value)
    {
        foreach (Pays pays in pays.Values)
        {
            if (pays.provinces.Count != 0)
            {
                pays.UpdateUnitsSeen(value);
            }
        }

        foreach (Railroad railroad in railroads.Values)
        {
            railroad.UpdateIsSeen(value);
        }
    }


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
                p.Refresh_Relations();
                p.RandomizeLeader();
                p.RefreshProvinces();
            }
            else
            {
                toDelete.Add(p.ID);
                while (p.provinces.Count > 0)
                {
                    Province obj = p.provinces[0];
                    p.RemoveProvince(obj);
                    DestroyImmediate(obj.gameObject);
                }
            }
        }

        foreach (string key in toDelete)
        {
            pays.Remove(key);
        }
        Ajout_Mois();
        CanvasWorker.instance.ShowDefault();
        CanvasWorker.instance.UpdateInfo();
    }

    public Pays GetCountry(string ID)
    {
        return pays[ID];
    }

    public Province GetProvince(string ID)
    {
        return provinces[ID];
    }

    public string GetGovernementDesc(int ID)
    {
        return gouvs_data.descs[ID];
    }

    public string GetGovernementName(int ID)
    {
        return gouvs_data.noms[ID];
    }


    void Ajout_Mois()
    {
        mois += 1;
        if (mois > 12)
        {
            mois = 1;
            an += 1;
            foreach (string key in pays.Keys)
            {
                GetCountry(key).NewYear();
            }
        }
    }

    public void NextTurn()
    {
        Ajout_Mois();
        foreach (Pays country in pays.Values)
        {
            if (country.provinces.Count > 0)
            {
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

    public string GetDate()
    {
        string[] values = { "january", "february", "march", "april", "may", "june", "july", "august", "september", "october", "november", "december" };
        return values[mois - 1] + " " + an.ToString();
    }


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
    }

}
