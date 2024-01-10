using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// The country picker is used to select to select the starting country at the start of the game
/// </summary>
public class CountryPicker : MonoBehaviour
{
    public Country current = null;

    [Header("Interresting Contries")]
    [SerializeField] private GameObject interrestingCountiesTab;

    [Header("All Contries")]
    [SerializeField] private GameObject pickerTab;
    [SerializeField] private Image currentCountryFlag;
    [SerializeField] private TextMeshProUGUI currentContryName;

    public static CountryPicker instance;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Event for selecting an interresting country
    /// </summary>
    /// <param name="ID">The country's ID</param>
    public void Click_SelectCountry(string ID)
    {
        ShowInterrestingCountriesTab(false);
        UpdateCountry(Manager.instance.GetCountry(ID));
    }

    /// <summary>
    /// Show interresting countries
    /// </summary>
    /// <param name="shown">Is shown ?</param>
    public void ShowInterrestingCountriesTab(bool shown)
    {
        interrestingCountiesTab.SetActive(shown);
        pickerTab.SetActive(!shown);
    }

    /// <summary>
    /// Initialize the country picker
    /// </summary>
    public void Init()
    {
        UpdateCountry(Manager.instance.GetCountry("FRA"));
    }

    /// <summary>
    /// Selects a country and refresh the UI
    /// </summary>
    /// <param name="c">The target country</param>
    public void UpdateCountry(Country c)
    {
        current = c;
        currentCountryFlag.sprite = c.currentFlag;
        currentContryName.text = c.nom;
    }

    /// <summary>
    /// Launch the game and play as the currently selected country
    /// </summary>
    public void LaunchGame()
    {
        Manager.instance.StartGame(current);
        gameObject.SetActive(false);
    }

}
