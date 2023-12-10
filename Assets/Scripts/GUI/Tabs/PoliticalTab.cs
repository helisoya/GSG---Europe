using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A tab that shows political informations about a country, as well as it's formables
/// </summary>
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


    /// <summary>
    /// Open the political tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        InitTab();

    }

    /// <summary>
    /// Closes the political tab
    /// </summary>
    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        CanvasWorker.instance.UpdateInfo();
        CanvasWorker.instance.RefreshUtilityBar();
    }

    /// <summary>
    /// Initialize the tab
    /// </summary>
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


    /// <summary>
    /// Creates the parties pie chart
    /// </summary>
    /// <param name="values"></param>
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

    /// <summary>
    /// Add popularity to a party
    /// </summary>
    /// <param name="index">Party index</param>
    public void Add_Popularity_ShortCut(int index)
    {
        Manager manager = Manager.instance;
        if (manager.player.AP < 10)
        {
            return;
        }
        manager.player.AP -= 10;
        politicalAP.text = "Admin Power : " + manager.player.AP.ToString() + " <sprite=12>";

        manager.player.AddPopularity(index, 5);

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