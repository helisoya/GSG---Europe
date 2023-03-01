using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Culture
{
    public string id;
    public string[] noms;

    public string[] prenoms;
    public string[] audios;

    public GameObject prefabTank;
    public Dictionary<string, Focus> focusTree;

    public string GetRandom_Nom()
    {

        return noms[Random.Range(0, noms.Length)];
    }

    public string GetRandom_Prenom()
    {
        return prenoms[Random.Range(0, prenoms.Length)];
    }

    public string GetRandom_Musique()
    {
        return audios[Random.Range(0, audios.Length)];
    }

}