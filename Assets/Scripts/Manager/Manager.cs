using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;
    public string player = "000";

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


    void Awake()
    {
        instance = this;

        provinces = Parser.ParseProvinces();

        focus = Parser.ParseFocus();
        gouvs_data = GetComponent<Gouvernements>();
        pays = Parser.ParsePays();
        cultures = GetComponent<CulturesWorker>();
        formables = GetComponent<FormableWorker>();
        formables.SetFormables(Parser.ParseFormables(provinces));

        Parser.ParseHistory(provinces, pays, "history");

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
            if (province.controller == B.ID) province.controller = A.ID;
        }

        foreach (Province province in B.provinces)
        {
            if (province.controller == A.ID) province.controller = B.ID;
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


        A.MakePeaceWithCountry(B.ID);

        if (vassalizeB)
        {
            CanvasWorker.instance.UpdateRelations_ShortCut(A.ID, B.ID, 2);
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


    public void StartGame()
    {
        picked = true;
        CanvasWorker.instance.ShowDefault();
        CanvasWorker.instance.Show_CountryInfo(player);

        List<string> toDelete = new List<string>();

        foreach (Pays p in pays.Values)
        {
            if (!p.DestroyIfNotSelected || p.ID == player)
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

                if (country.ID != player)
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
