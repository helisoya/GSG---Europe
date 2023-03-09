using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UtilityTab : GUITab
{
    [SerializeField] private TextMeshProUGUI utilityDate;
    [SerializeField] private TextMeshProUGUI utilityAP;
    [SerializeField] private TextMeshProUGUI utilitySpeed;
    [SerializeField] private TextMeshProUGUI utilityUC;
    [SerializeField] private TextMeshProUGUI utilityDP;



    public override void OpenTab()
    {
        base.OpenTab();
        RefreshBar();
    }

    public void RefreshBar()
    {
        utilitySpeed.text = "Speed : " + ((int)(Time.timeScale)).ToString();
        utilityDate.text = Manager.instance.GetDate();
        Pays player = Manager.instance.player;

        utilityAP.text = player.AP.ToString();
        utilityDP.text = player.DP.ToString();
        utilityUC.text = player.units.Count + "/" + player.unitCap;
    }
}
