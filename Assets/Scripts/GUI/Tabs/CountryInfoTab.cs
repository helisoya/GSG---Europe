using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountryInfoTab : GUITab
{
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoGovernement;
    [SerializeField] private TextMeshProUGUI infoFocus;
    [SerializeField] private Image infoFocusFill;
    [SerializeField] private Image infoFlag;
    [SerializeField] private TextMeshProUGUI infoRelations;

    [Header("Leader Tab")]
    [SerializeField] private GameObject leaderTabRoot;
    [SerializeField] private TextMeshProUGUI infoLeader;
    [SerializeField] private Image leaderHead;
    [SerializeField] private Image leaderHair;
    [SerializeField] private Image leaderBody;


    [Header("Diplomacy")]
    [SerializeField] private GameObject diploRoot;
    [SerializeField] private GameObject buttonImproveRelations;
    [SerializeField] private GameObject buttonDecreaseRelations;
    [SerializeField] private GameObject buttonCreateWargoal;
    [SerializeField] private GameObject buttonDeclareWar;
    [SerializeField] private GameObject buttonPeaceDeal;
    [SerializeField] private GameObject buttonCreateFederation;
    [SerializeField] private GameObject buttonJoinFederation;
    [SerializeField] private GameObject buttonInviteFederation;
    [SerializeField] private GameObject buttonKickFederation;
    [SerializeField] private GameObject buttonDeclareIndependance;
    [SerializeField] private GameObject buttonPlayAs;



    private Pays current_countryinfo = null;




    public void Show_CountryInfo(Pays NEW)
    {
        Manager manager = Manager.instance;

        if (!isOpen)
        {
            OpenTab();
        }

        current_countryinfo = NEW;
        UpdateInfo(NEW);

        diploRoot.SetActive(NEW != manager.player);
        if (NEW != manager.player)
        { // Cas nation IA
            buttonImproveRelations.SetActive(!NEW.relations[manager.player.ID].atWar);
            buttonDecreaseRelations.SetActive(!NEW.relations[manager.player.ID].atWar);

            buttonCreateWargoal.SetActive(
                !(NEW.federation != null && manager.player.federation == NEW.federation) &&
                !NEW.relations[manager.player.ID].atWar &&
                NEW != manager.player.lord &&
                !NEW.relations[manager.player.ID].wargoals.Contains(manager.player.ID));
            buttonDeclareWar.SetActive(
                !(NEW.federation != null && manager.player.federation == NEW.federation) &&
                NEW.relations[manager.player.ID].wargoals.Contains(manager.player.ID) &&
                NEW != manager.player.lord &&
                !NEW.relations[manager.player.ID].atWar);
            buttonPeaceDeal.SetActive(NEW.relations[manager.player.ID].atWar);

            buttonCreateFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation == null &&
                manager.player.federation == null);
            buttonKickFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation != null &&
                manager.player.federation == NEW.federation &&
                manager.player.federation.leader == manager.player);
            buttonJoinFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation != null &&
                manager.player.federation == null);
            buttonInviteFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation == null &&
                manager.player.federation != null &&
                manager.player.federation.leader == manager.player);

            buttonDeclareIndependance.SetActive(NEW == manager.player.lord);
            buttonPlayAs.SetActive(NEW.lord == manager.player);
        }
    }

    public void UpdateInfo()
    {
        UpdateInfo(current_countryinfo);
    }

    public void UpdateInfo(Pays country)
    {
        infoName.text = country.nom;
        infoGovernement.text = Manager.instance.GetGovernementName(country.Government_Form);

        infoFocus.text = (country.currentFocus.Equals("NONE") ? "Doing Nothing" : country.focusTree[country.currentFocus].focusName);
        infoFocusFill.fillAmount = (country.currentFocus.Equals("NONE") ? 0 : country.currentFocusTime / (float)country.maxFocusTime);
        infoFlag.GetComponent<Image>().sprite = country.currentFlag;

        infoLeader.text = country.leader.prenom + " " + country.leader.nom + "\n" + country.leader.age.ToString() + " years old";
        leaderHead.sprite = Resources.Load<Sprite>("Characters/Head/" + country.leader.headGFX);
        leaderHead.color = country.leader.headColor;
        leaderBody.sprite = Resources.Load<Sprite>("Characters/Body/" + country.leader.bodyGFX);
        leaderBody.color = country.leader.bodyColor;
        leaderHair.sprite = Resources.Load<Sprite>("Characters/Hair/" + country.leader.hairGFX);
        leaderHair.color = country.leader.hairColor;

        if (country == Manager.instance.player)
        {
            infoRelations.text = "";
        }
        else
        {
            infoRelations.text = "Relations : " + country.relations[Manager.instance.player.ID].relationScore.ToString();
        }

    }

    public void ToggleLeadeTab()
    {
        leaderTabRoot.SetActive(!leaderTabRoot.activeInHierarchy);
    }

    public void Event_IncreaseRelation()
    {
        Pays player = Manager.instance.player;
        if (player.DP >= 5)
        {
            player.DP -= 5;
            player.relations[current_countryinfo.ID].AddScore(10);
            CanvasWorker.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
        }
    }

    public void Event_DecreaseRelation()
    {
        Pays player = Manager.instance.player;
        if (player.DP >= 5)
        {
            player.DP -= 5;
            player.relations[current_countryinfo.ID].AddScore(-10);
            CanvasWorker.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
        }
    }

    public void Event_CreateWarGoal()
    {
        Pays player = Manager.instance.player;
        if (player.DP >= 20 && !player.relations[current_countryinfo.ID].wargoals.Contains(player.ID))
        {
            player.DP -= 20;
            player.relations[current_countryinfo.ID].wargoals.Add(player.ID);
            CanvasWorker.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
            Tooltip.instance.HideInfo();
        }
    }

    public void Event_DeclareWar()
    {
        CanvasWorker.instance.UpdateRelations_ShortCut(Manager.instance.player, current_countryinfo, 1);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
    }

    public void Event_PeaceDeal()
    {
        CanvasWorker.instance.OpenPeaceDealTab(Manager.instance.player.ID, current_countryinfo.ID);
        Tooltip.instance.HideInfo();
    }

    public void Event_CreateFederation()
    {
        if (Manager.instance.player.DP < 15 || Manager.instance.player.relations[current_countryinfo.ID].relationScore < 50) return;

        Manager.instance.player.DP -= 15;
        Federation federation = new Federation();
        federation.AddMember(Manager.instance.player);
        federation.AddMember(current_countryinfo);
        federation.leader = Manager.instance.player;
        Manager.instance.federations.Add(federation);
        CanvasWorker.instance.RefreshUtilityBar();
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();

        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            current_countryinfo.RefreshProvinces();
            Manager.instance.player.RefreshProvinces();
        }
    }

    public void Event_JoinFederation()
    {
        if (Manager.instance.player.relations[current_countryinfo.ID].relationScore < 50) return;
        current_countryinfo.federation.AddMember(Manager.instance.player);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            Manager.instance.player.RefreshProvinces();
        }
    }

    public void Event_InviteFederation()
    {
        if (Manager.instance.player.relations[current_countryinfo.ID].relationScore < 50) return;
        Manager.instance.player.federation.AddMember(current_countryinfo);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            current_countryinfo.RefreshProvinces();
        }
    }

    public void Event_KickFederation()
    {
        current_countryinfo.federation.RemoveMember(current_countryinfo);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            current_countryinfo.RefreshProvinces();
        }
    }

    public void Event_DeclareIndependance()
    {
        Manager.instance.player.lord = null;
        CanvasWorker.instance.UpdateRelations_ShortCut(Manager.instance.player, current_countryinfo, 1);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
    }

    public void Event_PlayAs()
    {
        Pays old = Manager.instance.player;
        Manager.instance.player = current_countryinfo;
        old.RefreshProvinces();
        current_countryinfo.RefreshProvinces();
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
    }

}
