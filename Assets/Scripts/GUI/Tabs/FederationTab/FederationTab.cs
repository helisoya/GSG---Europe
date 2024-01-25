using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A tab that shows informations about a federation
/// </summary>
public class FederationTab : GUITab
{
    [SerializeField] private CountryPanel leaderPanel;
    [SerializeField] private Transform membersRoot;
    [SerializeField] private GameObject prefabMember;
    [SerializeField] private GameObject rootLeader;
    [SerializeField] private GameObject rootNotLeader;
    [SerializeField] private Button buttonAP;
    [SerializeField] private TextMeshProUGUI textAP;
    [SerializeField] private Button buttonDP;
    [SerializeField] private TextMeshProUGUI textDP;
    [SerializeField] private Button buttonUC;
    [SerializeField] private TextMeshProUGUI textUC;
    [SerializeField] private Button buttonVassalize;
    [SerializeField] private Button buttonUnite;
    [SerializeField] private Button buttonLeader;
    [SerializeField] private TextMeshProUGUI textPlayerDP;

    /// <summary>
    /// Opens the federation tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        UpdateFederationTab();
    }

    /// <summary>
    /// Close the federation tab
    /// </summary>
    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        Tooltip.instance.HideInfo();
        GameGUI.instance.RefreshUtilityBar();
    }

    /// <summary>
    /// Refresh the Diplomatic Power Text
    /// </summary>
    void RefreshDPText()
    {
        textPlayerDP.text = "Diplomatic Power : " + Manager.instance.player.DP.ToString() + "<sprite=13>";
    }


    /// <summary>
    /// Add Administrative Power Bonus
    /// </summary>
    public void AddAPBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.APBonus++;
        UpdateFederationTab();
    }

    /// <summary>
    /// Add Diplomatic Power bonus
    /// </summary>
    public void AddDPBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.DPBonus++;
        UpdateFederationTab();
    }

    /// <summary>
    /// Add Unit cap bonus
    /// </summary>
    public void AddUCBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.UCBonus++;
        UpdateFederationTab();
    }

    /// <summary>
    /// Freeze the federation leader
    /// </summary>
    public void SetLeaderFrozen()
    {
        if (Manager.instance.player.DP < 20) return;

        Manager.instance.player.DP -= 20;
        Manager.instance.player.federation.leaderFrozen = true;
        UpdateFederationTab();
    }

    /// <summary>
    /// Vassalize the members of a federation
    /// </summary>
    public void SetMembersVassal()
    {
        if (Manager.instance.player.DP < 25) return;

        Manager.instance.player.DP -= 25;
        Manager.instance.player.federation.EnableVassals();
        UpdateFederationTab();
    }

    /// <summary>
    /// Unite a federation
    /// </summary>
    public void Unite()
    {
        if (Manager.instance.player.DP < 30) return;

        Manager.instance.player.DP -= 30;
        Manager.instance.player.federation.Unite();
        CloseTab();
    }

    /// <summary>
    /// Refresh the federation ta
    /// </summary>
    void UpdateFederationTab()
    {
        Tooltip.instance.HideInfo();
        Federation federation = Manager.instance.player.federation;
        if (federation == null)
        {
            CloseTab();
            return;
        }
        leaderPanel.UpdateInfo(federation.leader);
        RefreshDPText();

        foreach (Transform child in membersRoot)
        {
            Destroy(child.gameObject);
        }
        float x = membersRoot.GetComponent<RectTransform>().sizeDelta.x;
        membersRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(x, 35 * federation.members.Count);

        foreach (Country pays in federation.members)
        {
            Instantiate(prefabMember, membersRoot).GetComponent<CountryPanel>().UpdateInfo(pays);
        }


        if (Manager.instance.player == federation.leader)
        {
            rootNotLeader.SetActive(false);
            rootLeader.SetActive(true);

            textAP.text = "+" + (10 * federation.APBonus).ToString() + " per turn";
            textUC.text = "+" + (3 * federation.UCBonus).ToString() + " per turn";
            textDP.text = "+" + (federation.DPBonus).ToString() + " per turn";

            buttonAP.enabled = federation.APBonus < 2;
            buttonAP.GetComponent<TooltipTrigger>().enabled = buttonAP.enabled;

            buttonDP.enabled = federation.DPBonus < 2;
            buttonDP.GetComponent<TooltipTrigger>().enabled = buttonDP.enabled;

            buttonUC.enabled = federation.UCBonus < 2;
            buttonUC.GetComponent<TooltipTrigger>().enabled = buttonUC.enabled;

            bool hasAllBonuses = (federation.APBonus + federation.DPBonus + federation.UCBonus) == 6;
            buttonLeader.enabled = hasAllBonuses && !federation.leaderFrozen;
            buttonLeader.GetComponent<TooltipTrigger>().enabled = buttonLeader.enabled;

            buttonVassalize.enabled = hasAllBonuses && !federation.vassalized && federation.leaderFrozen;
            buttonVassalize.GetComponent<TooltipTrigger>().enabled = buttonVassalize.enabled;

            buttonUnite.enabled = hasAllBonuses && federation.vassalized && federation.leaderFrozen;
            buttonUnite.GetComponent<TooltipTrigger>().enabled = buttonUnite.enabled;
        }
        else
        {
            rootNotLeader.SetActive(true);
            rootLeader.SetActive(false);
        }
    }



}
