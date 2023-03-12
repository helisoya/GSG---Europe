using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountryPanel : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countryName;
    [SerializeField] private Image countryFlag;


    public void UpdateInfo(Pays pays)
    {
        countryName.text = pays.nom;
        countryFlag.sprite = pays.currentFlag;
    }
}
