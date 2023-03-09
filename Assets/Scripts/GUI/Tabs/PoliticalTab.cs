using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PoliticalTab : GUITab
{
    [SerializeField] private Transform politicalGraph;
    [SerializeField] private PartyGUI[] politicalPartiesGUI;
    [SerializeField] private TextMeshProUGUI politicalElections;
    [SerializeField] private TextMeshProUGUI politicalAP;
    [SerializeField] private TextMeshProUGUI politicalGovernement;
    [SerializeField] private TextMeshProUGUI politicalGovernementDesc;
    [SerializeField] private Dropdown_Formable_Manager politicalFormables;
    [SerializeField] private Color32[] piechart_colors;

    [SerializeField] private Image piechart_prefab;


    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        InitTab();

    }

    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        CanvasWorker.instance.UpdateInfo();
        CanvasWorker.instance.RefreshUtilityBar();
    }


    public void InitTab()
    {
        Manager manager = Manager.instance;


        for (int i = 0; i < politicalGraph.childCount; i++)
        {
            Destroy(politicalGraph.GetChild(i).gameObject);
        }

        MakeGraph(manager.player.parties);

        for (int i = 0; i < piechart_colors.Length; i++)
        {
            politicalPartiesGUI[i].image.color = piechart_colors[i];
            politicalPartiesGUI[i].text.text = manager.player.parties[i].partyName;
        }

        if (manager.player.date_elections == -1)
        {
            politicalElections.text = "Next Elections : Never";
        }
        else
        {
            politicalElections.text = "Next Elections : " + manager.player.date_elections.ToString();
        }
        politicalAP.text = "Admin Power : " + manager.player.AP.ToString() + " <sprite=12>";
        politicalGovernement.text = manager.GetGovernementName(manager.player.Government_Form);
        politicalGovernementDesc.text = manager.GetGovernementDesc(manager.player.Government_Form);
        politicalFormables.RefreshDropdown(manager.player);
    }



    public void MakeGraph(Party[] values)
    {
        float total = 0f;
        float zrotation = 0f;
        for (int i = 0; i < values.Length; i++)
        {
            total += values[i].popularity;
        }
        for (int i = 0; i < values.Length; i++)
        {
            Image newWedge = Instantiate(piechart_prefab) as Image;
            newWedge.transform.SetParent(politicalGraph.transform, false);
            newWedge.color = piechart_colors[i];
            newWedge.fillAmount = values[i].popularity / total;
            newWedge.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, zrotation));
            zrotation -= newWedge.fillAmount * 360f;
        }
    }


    public void Add_Popularity_ShortCut(int index)
    {
        Manager manager = Manager.instance;
        if (manager.player.AP < 10)
        {
            return;
        }
        manager.player.AP -= 10;
        politicalAP.text = "Admin Power : " + manager.player.AP.ToString();

        manager.player.Add_Popularity(index, 5);

        for (int i = 0; i < politicalGraph.childCount; i++)
        {
            Destroy(politicalGraph.GetChild(i).gameObject);
        }
        MakeGraph(manager.player.parties);
    }

}



[System.Serializable]
public class PartyGUI
{
    public Image image;
    public TextMeshProUGUI text;
}