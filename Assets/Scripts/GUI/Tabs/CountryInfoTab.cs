using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A tab that shows informations about countries
/// </summary>
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

    [Header("Other Tabs")]
    [SerializeField] private UnitsTab unitsTab;



    private Country current_countryinfo = null;

    /// <summary>
    /// Opens the Country Info Tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        unitsTab.CloseTab();
    }

    /// <summary>
    /// Show a country's informations
    /// </summary>
    /// <param name="NEW">The country</param>
    public void Show_CountryInfo(Country NEW)
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
        { // Case AI nation
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
                manager.player.federation.leader == manager.player &&
                !manager.player.federation.vassalized);
            buttonJoinFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation != null &&
                manager.player.federation == null &&
                !NEW.federation.vassalized);
            buttonInviteFederation.SetActive(
                !NEW.relations[manager.player.ID].atWar &&
                NEW.federation == null &&
                manager.player.federation != null &&
                manager.player.federation.leader == manager.player &&
                !manager.player.federation.vassalized);

            buttonDeclareIndependance.SetActive(NEW == manager.player.lord);
            buttonPlayAs.SetActive(NEW.lord == manager.player);
        }
    }

    /// <summary>
    /// Updates the informations of the current Country
    /// </summary>
    public void UpdateInfo()
    {
        UpdateInfo(current_countryinfo);
    }

    /// <summary>
    /// Updates the informations of a country
    /// </summary>
    /// <param name="country">The country</param>
    public void UpdateInfo(Country country)
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

    /// <summary>
    /// Toggles the Leader tab
    /// </summary>
    public void ToggleLeadeTab()
    {
        leaderTabRoot.SetActive(!leaderTabRoot.activeInHierarchy);
    }

    /// <summary>
    /// Increases relations with a country
    /// </summary>
    public void Event_IncreaseRelation()
    {
        Country player = Manager.instance.player;
        if (player.DP >= 5)
        {
            player.DP -= 5;
            player.relations[current_countryinfo.ID].AddScore(10);
            GameGUI.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
            current_countryinfo.RefreshProvinces();
        }
    }

    /// <summary>
    /// Decreases relations with a country
    /// </summary>
    public void Event_DecreaseRelation()
    {
        Country player = Manager.instance.player;
        if (player.DP >= 5)
        {
            player.DP -= 5;
            player.relations[current_countryinfo.ID].AddScore(-10);
            GameGUI.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
            current_countryinfo.RefreshProvinces();
        }
    }

    /// <summary>
    /// Creates a wargoal with a country
    /// </summary>
    public void Event_CreateWarGoal()
    {
        Country player = Manager.instance.player;
        if (player.DP >= 20 && !player.relations[current_countryinfo.ID].wargoals.Contains(player.ID))
        {
            player.DP -= 20;
            player.relations[current_countryinfo.ID].wargoals.Add(player.ID);
            GameGUI.instance.RefreshUtilityBar();
            Show_CountryInfo(current_countryinfo);
            Tooltip.instance.HideInfo();
        }
    }


    /// <summary>
    /// Declare war on a country
    /// </summary>
    public void Event_DeclareWar()
    {
        GameGUI.instance.UpdateRelations_ShortCut(Manager.instance.player, current_countryinfo, 1);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
    }


    /// <summary>
    /// Show the peace deal tab
    /// </summary>
    public void Event_PeaceDeal()
    {
        GameGUI.instance.OpenPeaceDealTab(Manager.instance.player.ID, current_countryinfo.ID);
        Tooltip.instance.HideInfo();
    }


    /// <summary>
    /// Creates a federation with a country
    /// </summary>
    public void Event_CreateFederation()
    {
        if (Manager.instance.player.DP < 15 || Manager.instance.player.relations[current_countryinfo.ID].relationScore < 50) return;

        Manager.instance.player.DP -= 15;
        Federation federation = new Federation();
        federation.AddMember(Manager.instance.player);
        federation.AddMember(current_countryinfo);
        federation.leader = Manager.instance.player;
        Manager.instance.federations.Add(federation);
        GameGUI.instance.RefreshUtilityBar();
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();

        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            current_countryinfo.RefreshProvinces();
            Manager.instance.player.RefreshProvinces();
        }
    }

    /// <summary>
    /// Join a country's federation
    /// </summary>
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

    /// <summary>
    /// Invite a country to the player's federation
    /// </summary>
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

    /// <summary>
    /// Kick a country for the player's federation
    /// </summary>
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

    /// <summary>
    /// Declare independance from the country
    /// </summary>
    public void Event_DeclareIndependance()
    {
        Manager.instance.player.lord = null;
        GameGUI.instance.UpdateRelations_ShortCut(Manager.instance.player, current_countryinfo, 1);
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
    }


    /// <summary>
    /// Play as the country
    /// </summary>
    public void Event_PlayAs()
    {
        Country old = Manager.instance.player;
        Manager.instance.player = current_countryinfo;
        old.RefreshProvinces();
        current_countryinfo.RefreshProvinces();
        Show_CountryInfo(current_countryinfo);
        Tooltip.instance.HideInfo();
        GameGUI.instance.ResetFocusTree();
    }

}
