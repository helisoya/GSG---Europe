using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulturesWorker : MonoBehaviour
{

    private Dictionary<string, Culture> cultures = new Dictionary<string, Culture>();

    public void SetCultures(Dictionary<string, Culture> newVal)
    {
        cultures = newVal;
    }

    public Culture GetCulture(string id)
    {
        return cultures[id];
    }

    public string GetRandom_Nom(string id)
    {

        return cultures[id].noms[Random.Range(0, cultures[id].noms.Length)];
    }

    public string GetRandom_Prenom(string id)
    {
        return cultures[id].prenoms[Random.Range(0, cultures[id].prenoms.Length)];
    }

    public string GetRandom_Musique(string id)
    {
        return cultures[id].audios[Random.Range(0, cultures[id].audios.Length)];
    }
}

[System.Serializable]
public class Culture
{
    public string id;
    public string[] noms;

    public string[] prenoms;
    public string[] audios;

    public GameObject prefabTank;
    public Dictionary<string, Focus> focusTree;

}