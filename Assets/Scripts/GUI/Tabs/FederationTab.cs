using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        UpdateFederationTab();
    }

    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        Tooltip.instance.HideInfo();
        CanvasWorker.instance.RefreshUtilityBar();
    }

    public void AddAPBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.APBonus++;
        UpdateFederationTab();
    }

    public void AddDPBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.DPBonus++;
        UpdateFederationTab();
    }


    public void AddUCBonus()
    {
        if (Manager.instance.player.DP < 10) return;

        Manager.instance.player.DP -= 10;
        Manager.instance.player.federation.UCBonus++;
        UpdateFederationTab();
    }

    public void SetLeaderFrozen()
    {
        if (Manager.instance.player.DP < 20) return;

        Manager.instance.player.DP -= 20;
        Manager.instance.player.federation.leaderFrozen = true;
        UpdateFederationTab();
    }

    public void SetMembersVassal()
    {
        if (Manager.instance.player.DP < 25) return;

        Manager.instance.player.DP -= 25;
        Manager.instance.player.federation.EnableVassals();
        UpdateFederationTab();
    }

    public void Unite()
    {
        if (Manager.instance.player.DP < 30) return;

        Manager.instance.player.DP -= 30;
        Manager.instance.player.federation.Unite();
        CloseTab();
    }

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

        foreach (Transform child in membersRoot)
        {
            Destroy(child.gameObject);
        }
        float x = membersRoot.GetComponent<RectTransform>().sizeDelta.x;
        membersRoot.GetComponent<RectTransform>().sizeDelta = new Vector2(x, 35 * federation.members.Count);

        foreach (Pays pays in federation.members)
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
