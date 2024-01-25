using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using System;

public class Parser : MonoBehaviour
{

    public static Dictionary<int, Vector3> ParsePoints()
    {

        string json = Resources.Load<TextAsset>("JSON/points").text;

        JSON obj = JSON.ParseString(json);

        JArray array = obj.GetJArray("points");
        Dictionary<int, Vector3> list = new Dictionary<int, Vector3>();

        for (int i = 0; i < array.Length; i++)
        {
            JSON vec = array.GetJSON(i);
            list.Add(vec.GetInt("id"), new Vector3(vec.GetFloat("x"), vec.GetFloat("y"), vec.GetFloat("z")));
        }
        return list;
    }


    public static Dictionary<int, Province> ParseProvinces()
    {
        Dictionary<int, Vector3> points = ParsePoints();

        string json = Resources.Load<TextAsset>("JSON/provinces").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<int, Province> list = new Dictionary<int, Province>();

        JArray array = obj.GetJArray("provinces");
        JArray arrayVertices;
        JSON jsonProv;
        Province province;
        Vector3[] vecs;
        for (int i = 0; i < array.Length; i++)
        {
            jsonProv = array.GetJSON(i);
            province = Instantiate(Manager.instance.prefabProvince, Manager.instance.provinceParent).GetComponent<Province>();
            province.gameObject.name = jsonProv.GetString("name");
            province.Province_Name = jsonProv.GetString("name");
            province.id = jsonProv.GetInt("id");
            province.type = System.Enum.Parse<ProvinceType>(jsonProv.GetString("type"));
            arrayVertices = jsonProv.GetJArray("vertices");

            vecs = new Vector3[arrayVertices.Length];

            for (int j = 0; j < arrayVertices.Length; j++)
            {
                vecs[j] = points[arrayVertices.GetInt(j)];
            }
            province.Init(vecs);
            list.Add(province.id, province);
        }

        for (int i = 0; i < array.Length; i++)
        {
            jsonProv = array.GetJSON(i);

            int id = jsonProv.GetInt("id");

            arrayVertices = jsonProv.GetJArray("adjacencies");
            list[id].adjacencies = new Province[arrayVertices.Length];

            for (int j = 0; j < arrayVertices.Length; j++)
            {
                if (!list.ContainsKey(arrayVertices.GetInt(j)))
                {
                    print("Error : " + arrayVertices.GetString(j));
                    continue;
                }
                list[id].adjacencies[j] = list[arrayVertices.GetInt(j)];
            }
        }

        return list;
    }


    public static Dictionary<string, GameEvent> ParseEvents()
    {
        string json = Resources.Load<TextAsset>("JSON/events").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, GameEvent> list = new Dictionary<string, GameEvent>();

        JArray array = obj.GetJArray("events");
        JSON jsonEvent;
        JArray arrayIn;
        JArray arrayEffects;
        for (int i = 0; i < array.Length; i++)
        {
            jsonEvent = array.GetJSON(i);
            GameEvent gameEvent = new GameEvent();
            gameEvent.id = jsonEvent.GetString("id");
            gameEvent.title = jsonEvent.GetString("title");
            gameEvent.description = jsonEvent.GetString("description");


            arrayIn = jsonEvent.GetJArray("buttons");
            for (int j = 0; j < arrayIn.Length; j++)
            {
                JSON button = arrayIn.GetJSON(j);
                gameEvent.buttons[j] = new GameEvent.EventButton();
                gameEvent.buttons[j].label = button.GetString("name");
                arrayEffects = button.GetJArray("effect");
                gameEvent.buttons[j].effects = new string[arrayEffects.Length];
                for (int y = 0; y < arrayEffects.Length; y++)
                {
                    gameEvent.buttons[j].effects[y] = arrayEffects.GetString(y);
                }
            }


            list.Add(gameEvent.id, gameEvent);
        }
        return list;
    }

    public static Dictionary<string, Culture> ParseCultures(Dictionary<string, Dictionary<string, Focus>> focuses)
    {
        string json = Resources.Load<TextAsset>("JSON/cultures").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, Culture> list = new Dictionary<string, Culture>();

        JArray array = obj.GetJArray("cultures");
        JSON jsonCulture;
        JArray arrayIn;
        for (int i = 0; i < array.Length; i++)
        {
            jsonCulture = array.GetJSON(i);
            Culture culture = new Culture();
            culture.id = jsonCulture.GetString("id");

            culture.prefabUnits = new Dictionary<UnitType, GameObject>();
            foreach (UnitType type in Enum.GetValues(typeof(UnitType)))
            {
                culture.prefabUnits.Add(type, Resources.Load<GameObject>("Units/Cultures/" + culture.id + "/" + type.ToString()));
            }

            culture.focusTree = focuses[jsonCulture.GetString("focus")];

            arrayIn = jsonCulture.GetJArray("names");
            culture.names = new string[arrayIn.Length];
            for (int j = 0; j < arrayIn.Length; j++)
            {
                culture.names[j] = arrayIn.GetString(j);
            }

            arrayIn = jsonCulture.GetJArray("surnames");
            culture.surnames = new string[arrayIn.Length];
            for (int j = 0; j < arrayIn.Length; j++)
            {
                culture.surnames[j] = arrayIn.GetString(j);
            }

            list.Add(culture.id, culture);
        }
        return list;
    }

    public static Dictionary<string, Country> ParsePays(Dictionary<string, Culture> cultures)
    {

        string json = Resources.Load<TextAsset>("JSON/pays").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, Country> list = new Dictionary<string, Country>();

        JArray array = obj.GetJArray("pays");
        JSON jsonPays;
        JArray color;
        for (int i = 0; i < array.Length; i++)
        {
            jsonPays = array.GetJSON(i);
            Country pays = new Country();
            pays.ID = jsonPays.GetString("id");
            pays.cosmeticID = jsonPays.GetString("id");
            pays.nom = jsonPays.GetString("name");
            pays.culture = cultures[jsonPays.GetString("culture")];
            pays.DestroyIfNotSelected = jsonPays.GetString("secretNation").Equals("True");
            color = jsonPays.GetJArray("color");
            pays.SetColor(new Color(color.GetFloat(0) / 255f, color.GetFloat(1) / 255f, color.GetFloat(2) / 255f));
            list.Add(pays.ID, pays);
        }

        int comparison;
        foreach (Country pays in list.Values)
        {
            foreach (Country other in list.Values)
            {
                comparison = pays.ID.CompareTo(other.ID);
                if (comparison == 0) continue;

                if (other.relations.ContainsKey(pays.ID))
                {
                    pays.relations.Add(other.ID, other.relations[pays.ID]);
                }
                else
                {
                    pays.relations.Add(other.ID, new Relation(pays, other));
                }
            }
        }
        return list;
    }

    public static void ParseHistory(Dictionary<int, Province> allProvinces, Dictionary<string, Country> allCountries, string fileName)
    {

        string json = Resources.Load<TextAsset>("JSON/" + fileName).text;

        JSON obj = JSON.ParseString(json);

        JArray array = obj.GetJArray("pays");
        JSON jsonPays;
        JArray arrayParties;
        JArray arrayProvinces;
        JSON party;
        for (int i = 0; i < array.Length; i++)
        {
            jsonPays = array.GetJSON(i);
            Country pays = allCountries[jsonPays.GetString("id")];
            pays.Government_Form = jsonPays.GetInt("governement");

            arrayParties = jsonPays.GetJArray("parties");
            pays.parties = new Party[arrayParties.Length];
            for (int j = 0; j < arrayParties.Length; j++)
            {
                party = arrayParties.GetJSON(j);
                pays.parties[j] = new Party(party.GetString("name"), party.GetFloat("popularity"), j);
            }

            arrayProvinces = jsonPays.GetJArray("provinces");
            pays.provinces = new List<Province>();
            for (int j = 0; j < arrayProvinces.Length; j++)
            {
                pays.AddProvince(allProvinces[arrayProvinces.GetInt(j)], false);
            }

            arrayProvinces = jsonPays.GetJArray("cores");
            pays.cores = new List<Province>();
            for (int j = 0; j < arrayProvinces.Length; j++)
            {
                pays.cores.Add(allProvinces[arrayProvinces.GetInt(j)]);
            }
            pays.ResetFlag();
            pays.ResetElections();
            pays.RefreshProvinces();
            pays.AddToTraversalOptions(pays);
        }
        foreach (Country pays in allCountries.Values)
        {
            pays.CheckNeighboors();
        }
    }

    public static List<FormableNation> ParseFormables(Dictionary<int, Province> allProvinces)
    {

        string json = Resources.Load<TextAsset>("JSON/formables").text;

        JSON obj = JSON.ParseString(json);

        List<FormableNation> list = new List<FormableNation>();

        JArray array = obj.GetJArray("formables");
        JSON formableJson;
        JArray arrayContestants;
        JArray arrayRequired;
        for (int i = 0; i < array.Length; i++)
        {
            formableJson = array.GetJSON(i);
            FormableNation formable = new FormableNation();
            formable.id = formableJson.GetString("id");
            formable.Name = formableJson.GetString("name");

            arrayContestants = formableJson.GetJArray("contestants");
            formable.contestants = new List<string>();
            for (int j = 0; j < arrayContestants.Length; j++)
            {
                formable.contestants.Add(arrayContestants.GetString(j));
            }

            arrayRequired = formableJson.GetJArray("required");
            formable.required = new List<Province>();
            for (int j = 0; j < arrayRequired.Length; j++)
            {
                formable.required.Add(allProvinces[arrayRequired.GetInt(j)]);
            }

            list.Add(formable);
        }
        return list;
    }

    public static Dictionary<string, Dictionary<string, Focus>> ParseFocus()
    {
        Dictionary<string, Dictionary<string, Focus>> res = new Dictionary<string, Dictionary<string, Focus>>();
        TextAsset[] textAssets = Resources.LoadAll<TextAsset>("JSON/Focuses");

        foreach (TextAsset asset in textAssets)
        {
            string json = asset.text;

            JSON obj = JSON.ParseString(json);

            Dictionary<string, Focus> list = new Dictionary<string, Focus>();

            JArray array = obj.GetJArray("focus");
            JSON focusJSON;
            JArray arrayExclusive;
            JArray arrayRequired;
            for (int i = 0; i < array.Length; i++)
            {
                focusJSON = array.GetJSON(i);
                Focus focus = new Focus();
                focus.id = focusJSON.GetString("id");
                focus.focusName = focusJSON.GetString("name");
                focus.desc = focusJSON.GetString("desc");
                focus.requireAll = focusJSON.GetString("requireAll").Equals("True");
                focus.x = focusJSON.GetInt("x");
                focus.y = focusJSON.GetInt("y");

                arrayRequired = focusJSON.GetJArray("required");
                focus.required = new List<string>();
                for (int j = 0; j < arrayRequired.Length; j++)
                {
                    focus.required.Add(arrayRequired.GetString(j));
                }

                arrayExclusive = focusJSON.GetJArray("exclusive");
                focus.exclusive = new List<string>();
                for (int j = 0; j < arrayExclusive.Length; j++)
                {
                    focus.exclusive.Add(arrayExclusive.GetString(j));
                }


                focus.effect = new List<string>();
                arrayExclusive = focusJSON.GetJArray("effect");
                for (int j = 0; j < arrayExclusive.Length; j++)
                {
                    focus.effect.Add(arrayExclusive.GetString(j));
                }


                list.Add(focus.id, focus);
            }
            res.Add(obj.GetString("id"), list);
        }


        return res;
    }
}

