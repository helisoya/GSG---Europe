using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryPicker : MonoBehaviour
{
    public Pays current = null;

    [SerializeField] private Image flag;
    [SerializeField] private TextMeshProUGUI text;

    public static CountryPicker instance;

    void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        UpdateCountry(Manager.instance.GetCountry("FRA"));
    }


    public void UpdateCountry(Pays c)
    {
        current = c;
        flag.sprite = c.currentFlag;
        text.text = c.nom;
    }

    public void LaunchGame()
    {
        Manager.instance.StartGame(current);
        gameObject.SetActive(false);
    }

}
