using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Used in the Federation Tab to show memberw
/// </summary>
public class CountryPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countryName;
    [SerializeField] private Image countryFlag;


    /// <summary>
    /// Updates the country info panel
    /// </summary>
    /// <param name="pays"></param>
    public void UpdateInfo(Pays pays)
    {
        countryName.text = pays.nom;
        countryFlag.sprite = pays.currentFlag;
    }
}
