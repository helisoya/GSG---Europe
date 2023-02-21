using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Leguar.TotalJSON;

public class Parser : MonoBehaviour
{

    public static Dictionary<string, Vector3> ParsePoints()
    {

        string json = Resources.Load<TextAsset>("JSON/points").text;

        JSON obj = JSON.ParseString(json);

        JArray array = obj.GetJArray("points");
        Dictionary<string, Vector3> list = new Dictionary<string, Vector3>();

        for (int i = 0; i < array.Length; i++)
        {
            JSON vec = array.GetJSON(i);
            list.Add(vec.GetString("name"), new Vector3(vec.GetFloat("x"), vec.GetFloat("y"), vec.GetFloat("z")));
        }
        return list;
    }


    public static Dictionary<string, Province> ParseProvinces()
    {
        Dictionary<string, Vector3> points = ParsePoints();

        string json = Resources.Load<TextAsset>("JSON/provinces").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, Province> list = new Dictionary<string, Province>();

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
            arrayVertices = jsonProv.GetJArray("vertices");

            vecs = new Vector3[arrayVertices.Length];

            for (int j = 0; j < arrayVertices.Length; j++)
            {
                vecs[j] = points[arrayVertices.GetString(j)];
            }
            province.Init(vecs);
            list.Add(province.Province_Name, province);
        }
        return list;
    }

    public static Dictionary<string, Pays> ParsePays()
    {

        string json = Resources.Load<TextAsset>("JSON/pays").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, Pays> list = new Dictionary<string, Pays>();

        JArray array = obj.GetJArray("pays");
        JSON jsonPays;
        JArray color;
        for (int i = 0; i < array.Length; i++)
        {
            jsonPays = array.GetJSON(i);
            Pays pays = new Pays();
            pays.ID = jsonPays.GetString("id");
            pays.cosmeticID = jsonPays.GetString("id");
            pays.nom = jsonPays.GetString("name");
            pays.culture = jsonPays.GetString("culture");
            pays.DestroyIfNotSelected = jsonPays.GetString("secretNation").Equals("True");
            color = jsonPays.GetJArray("color");
            pays.SetColor(new Color(color.GetFloat(0) / 255f, color.GetFloat(1) / 255f, color.GetFloat(2) / 255f));
            list.Add(pays.ID, pays);
        }
        return list;
    }

    public static void ParseHistory(Dictionary<string, Province> allProvinces, Dictionary<string, Pays> allCountries, string fileName)
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
            Pays pays = allCountries[jsonPays.GetString("id")];
            pays.Government_Form = jsonPays.GetInt("governement");

            arrayParties = jsonPays.GetJArray("parties");
            pays.parties = new Party[arrayParties.Length];
            for (int j = 0; j < arrayParties.Length; j++)
            {
                party = arrayParties.GetJSON(j);
                pays.parties[j] = new Party(party.GetString("name"), party.GetFloat("popularity"));
            }

            arrayProvinces = jsonPays.GetJArray("provinces");
            pays.provinces = new List<Province>();
            for (int j = 0; j < arrayProvinces.Length; j++)
            {
                pays.AddProvince(allProvinces[arrayProvinces.GetString(j)], false);
            }

            arrayProvinces = jsonPays.GetJArray("cores");
            pays.cores = new List<Province>();
            for (int j = 0; j < arrayProvinces.Length; j++)
            {
                pays.cores.Add(allProvinces[arrayProvinces.GetString(j)]);
            }
            pays.Reset_Flag();
            pays.Reset_Elections();
            pays.RefreshProvinces();
        }
    }

    public static List<FormableNation> ParseFormables(Dictionary<string, Province> allProvinces)
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
                formable.required.Add(allProvinces[arrayRequired.GetString(j)]);
            }

            list.Add(formable);
        }
        return list;
    }

    public static Dictionary<string, Focus> ParseFocus()
    {

        string json = Resources.Load<TextAsset>("JSON/focus").text;

        JSON obj = JSON.ParseString(json);

        Dictionary<string, Focus> list = new Dictionary<string, Focus>();

        JArray array = obj.GetJArray("focus");
        JSON focusJSON;
        JArray arrayExclusive;
        for (int i = 0; i < array.Length; i++)
        {
            focusJSON = array.GetJSON(i);
            Focus focus = new Focus();
            focus.id = focusJSON.GetString("id");
            focus.focusName = focusJSON.GetString("name");
            focus.desc = focusJSON.GetString("desc");
            focus.required = focusJSON.GetString("required");
            focus.effect = focusJSON.GetString("effect");

            arrayExclusive = focusJSON.GetJArray("exclusive");
            focus.exclusive = new List<string>();
            for (int j = 0; j < arrayExclusive.Length; j++)
            {
                focus.exclusive.Add(arrayExclusive.GetString(j));
            }


            list.Add(focus.id, focus);
        }
        return list;
    }
}
