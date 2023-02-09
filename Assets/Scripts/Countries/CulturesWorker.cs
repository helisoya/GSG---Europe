using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CulturesWorker : MonoBehaviour
{

    public List<Culture> cultures = new List<Culture>();

    public Culture GetCulture(string culture)
    {
        foreach (Culture cult in cultures)
        {
            if (cult.culture == culture)
            {
                return cult;
            }
        }
        return null;
    }

    public string GetRandom_Nom(string culture)
    {
        foreach (Culture cult in cultures)
        {
            if (cult.culture == culture)
            {
                return cult.noms[Random.Range(0, cult.noms.Length)];
            }
        }
        return "Ramos";
    }

    public string GetRandom_Prenom(string culture)
    {
        foreach (Culture cult in cultures)
        {
            if (cult.culture == culture)
            {
                return cult.prenoms[Random.Range(0, cult.prenoms.Length)];
            }
        }
        return "Harold";
    }

    public AudioClip GetRandom_Musique(string culture)
    {
        foreach (Culture cult in cultures)
        {
            if (cult.culture == culture)
            {
                if (cult.musiques.Length == 0)
                {
                    return null;
                }
                return cult.musiques[Random.Range(0, cult.musiques.Length)];
            }
        }
        return null;
    }
}

[System.Serializable]
public class Culture
{
    public string culture;
    public string[] noms;

    public string[] prenoms;

    public AudioClip[] musiques;

    public GameObject prefabTank;
}