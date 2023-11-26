using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryPicker : MonoBehaviour
{
    public Pays current = null;

    [Header("Interresting Contries")]
    [SerializeField] private GameObject interrestingCountiesTab;

    [Header("All Contries")]
    [SerializeField] private GameObject pickerTab;
    [SerializeField] private Image currentCountryFlag;
    [SerializeField] private TextMeshProUGUI currentContryName;

    public static CountryPicker instance;


    public void Click_SelectCountry(string ID)
    {
        ShowInterrestingCountriesTab(false);
        UpdateCountry(Manager.instance.GetCountry(ID));
    }

    public void ShowInterrestingCountriesTab(bool shown)
    {
        interrestingCountiesTab.SetActive(shown);
        pickerTab.SetActive(!shown);
    }


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
        currentCountryFlag.sprite = c.currentFlag;
        currentContryName.text = c.nom;
    }

    public void LaunchGame()
    {
        Manager.instance.StartGame(current);
        gameObject.SetActive(false);
    }

}
