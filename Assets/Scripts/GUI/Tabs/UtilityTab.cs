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


    public void UpdateSpeedText()
    {
        utilitySpeed.text = "Speed : " + ((int)(Time.timeScale)).ToString();
    }

    public void UpdateUtilityUnitCap()
    {
        utilityUC.text = Manager.instance.player.units.Count + "/" + Manager.instance.player.unitCap;
    }

    public void Update_Date()
    {
        utilityDate.text = Manager.instance.GetDate();
    }


    public void UpdateAPBar()
    {
        utilityAP.text = Manager.instance.player.AP.ToString();
    }

    public override void OpenTab()
    {
        base.OpenTab();
        RefreshBar();
    }

    public void RefreshBar()
    {
        UpdateSpeedText();
        UpdateUtilityUnitCap();
        Update_Date();
        UpdateAPBar();
    }
}
