using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryPicker : MonoBehaviour
{
    public string current = "FRA";

    private Manager manager;

    [SerializeField] private Image flag;
    [SerializeField] private Text text;

    void Start()
    {
        manager = Manager.instance;
        UpdateCountry(current);
    }


    public void UpdateCountry(string c)
    {
        current = c;
        flag.sprite = manager.GetCountry(c).currentFlag;
        text.text = manager.GetCountry(c).nom;
    }

    public void LaunchGame()
    {
        manager.player = current;
        manager.StartGame();
        gameObject.SetActive(false);
    }

}
