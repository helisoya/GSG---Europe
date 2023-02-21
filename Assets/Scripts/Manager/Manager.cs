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

    public CulturesWorker cultures;

    public List<GameObject> selected_unit = new List<GameObject>();

    public enum MAPMODE
    {
        POLITICAL,
        IDEOLOGICAL,
        DIPLOMATIC,
        FORMABLE
    }
    public MAPMODE currentMapMode = MAPMODE.POLITICAL;

    public Color32[] colors_ideologie;

    public Color32[] colors_relations;

    public Color32[] colors_formable;

    public FormableWorker formables;

    public FormableNation currentFormable;

    public bool picked = false;

    public bool inPeaceDeal = false;

    public string peaceDealSide1;
    public string peaceDealSide2;
    public List<Province> provincesToBeTakenInPeaceDeal;

    private Dictionary<string, Province> provinces;

    public Dictionary<string, Focus> focus;

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

    void Start()
    {
        loading = null;
        instance = this;

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

        loadingImg.fillAmount = 0;
        loadingText.text = "Loading Provinces";
        yield return new WaitForEndOfFrame();
        provinces = Parser.ParseProvinces();


        loadingImg.fillAmount = 1f / 5f;
        loadingText.text = "Loading Focus";
        yield return new WaitForEndOfFrame();
        focus = Parser.ParseFocus();


        loadingImg.fillAmount = 2f / 5f;
        loadingText.text = "Loading Countries";
        yield return new WaitForEndOfFrame();
        gouvs_data = GetComponent<Gouvernements>();
        pays = Parser.ParsePays();
        cultures = GetComponent<CulturesWorker>();

        loadingImg.fillAmount = 3f / 5f;
        loadingText.text = "Loading Formables";
        yield return new WaitForEndOfFrame();
        formables = GetComponent<FormableWorker>();
        formables.SetFormables(Parser.ParseFormables(provinces));


        loadingImg.fillAmount = 4f / 5f;
        loadingText.text = "Loading History";
        yield return new WaitForEndOfFrame();
        Parser.ParseHistory(provinces, pays, "history");

        Destroy(loadingRoot);

        CanvasWorker.instance.manager = this;

        picker.Init();

        loading = null;
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


    public void CheckEveryItems()
    {
        foreach (Pays pays in pays.Values)
        {
            if (pays.provinces.Count != 0)
            {
                pays.CheckCitiesUnits();
            }
        }
    }


    public void StartGame(Pays chosenCountry)
    {
        player = chosenCountry;
        picked = true;
        CanvasWorker.instance.ShowDefault();
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
        CanvasWorker.instance.Update_Date();
        CanvasWorker.instance.UpdateInfo();
        CanvasWorker.instance.UpdatePPBar();
        CanvasWorker.instance.UpdateUtilityUnitCap();
    }

    public Pays GetCountry(string ID)
    {
        return pays[ID];
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
        CanvasWorker.instance.Update_Date();
        CanvasWorker.instance.UpdateInfo();
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
        CanvasWorker.instance.UpdatePPBar();
        CanvasWorker.instance.UpdateUtilityUnitCap();
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

        if (Input.GetKeyDown(KeyCode.Alpha1) && currentMapMode != 0)
        {
            currentMapMode = MAPMODE.POLITICAL;
            RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentMapMode = MAPMODE.IDEOLOGICAL;
            RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentMapMode = MAPMODE.DIPLOMATIC;
            RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && currentFormable != null)
        {
            currentMapMode = MAPMODE.FORMABLE;
            RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Timer>().StopTime();
            CanvasWorker.instance.OpenSettingsMenu();
        }
    }

}
