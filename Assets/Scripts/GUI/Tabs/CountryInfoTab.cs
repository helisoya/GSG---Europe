using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountryInfoTab : GUITab
{
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoGovernement;
    [SerializeField] private TextMeshProUGUI infoLeader;
    [SerializeField] private TextMeshProUGUI infoFocus;
    [SerializeField] private Image infoFocusFill;
    [SerializeField] private Image infoFlag;


    [SerializeField] private GameObject diploRoot;
    [SerializeField] private GameObject buttonImproveRelations;
    [SerializeField] private GameObject buttonDecreaseRelations;
    [SerializeField] private GameObject buttonCreateWargoal;
    [SerializeField] private GameObject buttonDeclareWar;
    [SerializeField] private GameObject buttonPeaceDeal;
    [SerializeField] private GameObject buttonFederation;
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

            buttonCreateWargoal.SetActive(!NEW.relations[manager.player.ID].atWar && NEW != manager.player.lord && !NEW.relations[manager.player.ID].wargoals.Contains(manager.player.ID));
            buttonDeclareWar.SetActive(NEW.relations[manager.player.ID].wargoals.Contains(manager.player.ID) && NEW != manager.player.lord && !NEW.relations[manager.player.ID].atWar);
            buttonPeaceDeal.SetActive(NEW.relations[manager.player.ID].atWar);

            buttonFederation.SetActive(!NEW.relations[manager.player.ID].atWar);

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
        infoLeader.text = country.leader.prenom + " " + country.leader.nom + "\n" + country.leader.age.ToString() + " years old";
        infoFocus.text = (country.currentFocus.Equals("NONE") ? "Doing Nothing" : country.focusTree[country.currentFocus].focusName);
        infoFocusFill.fillAmount = (country.currentFocus.Equals("NONE") ? 0 : country.currentFocusTime / (float)country.maxFocusTime);
        infoFlag.GetComponent<Image>().sprite = country.currentFlag;
    }

    public void Event_IncreaseRelation()
    {
        Pays player = Manager.instance.player;
        if (player.DP >= 5)
        {
            player.DP -= 5;
            player.relations[current_countryinfo.ID].AddScore(10);
            CanvasWorker.instance.RefreshUtilityBar();
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

    public void Event_Federation()
    {
        Pays player = Manager.instance.player;

        if (player.federation == null && current_countryinfo.federation == null)
        {
            Federation federation = new Federation();
            federation.AddMember(player);
            federation.AddMember(current_countryinfo);
            Manager.instance.federations.Add(federation);
        }
        else if (player.federation != null && current_countryinfo.federation == null)
        {
            player.federation.AddMember(current_countryinfo);
        }
        else if (player.federation == null && current_countryinfo.federation != null)
        {
            current_countryinfo.federation.AddMember(player);
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
