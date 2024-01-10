using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows general informations about the player
/// </summary>
public class UtilityTab : GUITab
{
    [SerializeField] private TextMeshProUGUI utilityDate;
    [SerializeField] private TextMeshProUGUI utilityAP;
    [SerializeField] private TextMeshProUGUI utilitySpeed;
    [SerializeField] private TextMeshProUGUI utilityUC;
    [SerializeField] private TextMeshProUGUI utilityDP;
    [SerializeField] private GameObject utilityFederation;


    /// <summary>
    /// Opens the Utility tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        RefreshBar();
    }

    /// <summary>
    /// Refresh the utility tab
    /// </summary>
    public void RefreshBar()
    {
        utilitySpeed.text = "Speed : " + ((int)(Time.timeScale)).ToString();
        utilityDate.text = Manager.instance.GetDate();
        Country player = Manager.instance.player;

        utilityAP.text = player.AP.ToString();
        utilityDP.text = player.DP.ToString();
        utilityUC.text = player.units.Count + "/" + player.unitCap;

        utilityFederation.SetActive(player.federation != null);
    }
}
