using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles all the GUI
/// </summary>
public class CanvasWorker : MonoBehaviour
{
    [HideInInspector] public Manager manager;

    public static CanvasWorker instance;


    [Space]
    [Header("Events")]
    [SerializeField] private GameObject eventsRoot;
    [SerializeField] private Text eventsTitle;
    [SerializeField] private Text eventsDesc;
    [SerializeField] private Button[] eventsButtons;
    private GameEvent currentEvent = null;

    [Space]
    [Header("Peace Deal")]
    [SerializeField] private GameObject peaceDealRoot;
    [SerializeField] private Toggle peaceDealVassal;
    [SerializeField] private Image peaceDealCountry1Img;
    [SerializeField] private TextMeshProUGUI peaceDealCountry1Score;
    [SerializeField] private Image peaceDealCountry2Img;
    [SerializeField] private TextMeshProUGUI peaceDealCountry2Score;
    [SerializeField] private TextMeshProUGUI peaceDealAcceptance;

    [Space]
    [Header("Other Menus")]
    [SerializeField] private PauseTab pauseTab;
    [SerializeField] private FocusTab focusMenu;
    [SerializeField] private PoliticalTab politicalTab;
    [SerializeField] private CountryInfoTab countryInfoTab;
    [SerializeField] private ProvinceTab provinceTab;
    [SerializeField] private UtilityTab utilityTab;
    [SerializeField] private FederationTab federationTab;


    void Awake()
    {
        instance = this;
    }


    /// <summary>
    /// Opens the settings menu
    /// </summary>
    public void OpenSettingsMenu()
    {
        if (pauseTab.isOpen) return;
        HideEverything();
        pauseTab.OpenTab();
    }

    /// <summary>
    /// Open the peace deal tab
    /// </summary>
    /// <param name="side1">Side 1 Country's ID</param>
    /// <param name="side2">Side 2 Country's ID</param>
    public void OpenPeaceDealTab(string side1, string side2)
    {
        HideEverything();
        Timer.instance.StopTime();
        peaceDealVassal.isOn = false;
        manager.inPeaceDeal = true;
        manager.peaceDealSide1 = manager.GetCountry(side1);
        manager.peaceDealSide2 = manager.GetCountry(side2);
        manager.provincesToBeTakenInPeaceDeal = new List<Province>();

        peaceDealCountry1Img.sprite = manager.peaceDealSide1.currentFlag;
        peaceDealCountry2Img.sprite = manager.peaceDealSide2.currentFlag;
        peaceDealCountry1Score.text = manager.peaceDealSide1.relations[manager.peaceDealSide2.ID].warScores[manager.peaceDealSide1.ID].ToString();
        peaceDealCountry2Score.text = manager.peaceDealSide1.relations[manager.peaceDealSide2.ID].warScores[manager.peaceDealSide2.ID].ToString();

        PeaceDealRefreshAcceptance();
        peaceDealRoot.SetActive(true);
        manager.RefreshMap();
    }

    /// <summary>
    /// Refresh peace deal acceptance
    /// </summary>
    public void PeaceDealRefreshAcceptance()
    {
        peaceDealAcceptance.text = PeaceDealAcceptanceScore().ToString();
    }

    /// <summary>
    /// Closes the peace deal tab
    /// </summary>
    public void HidePeaceDeal()
    {
        HideEverything();
        ShowDefault();

        manager.inPeaceDeal = false;
        manager.RefreshMap();
        Show_CountryInfo(manager.player);
        Timer.instance.ResumeTime();
    }


    /// <summary>
    /// Computes the peace deal acceptance score
    /// </summary>
    /// <returns>The acceptance score</returns>
    int PeaceDealAcceptanceScore()
    {
        Relation relation = manager.peaceDealSide1.relations[manager.peaceDealSide2.ID];
        int vassalCost = peaceDealVassal.isOn ? Mathf.CeilToInt(manager.peaceDealSide2.provinces.Count / 2f) * 10 : 0;
        return relation.warScores[manager.peaceDealSide1.ID] - manager.provincesToBeTakenInPeaceDeal.Count * 10 - vassalCost;
    }

    /// <summary>
    /// End the peace deal
    /// </summary>
    public void EndPeaceDeal()
    {
        if (PeaceDealAcceptanceScore() < 0) return;
        HideEverything();
        ShowDefault();
        manager.EndPeaceDeal(peaceDealVassal.isOn);
    }

    /// <summary>
    /// Select a province for the peace deal
    /// </summary>
    /// <param name="prov">The province</param>
    public void PeaceDealProvinceSelection(Province prov)
    {
        if (!manager.provincesToBeTakenInPeaceDeal.Remove(prov))
        {
            manager.provincesToBeTakenInPeaceDeal.Add(prov);
        }
        PeaceDealRefreshAcceptance();
        prov.RefreshColor();
    }

    /// <summary>
    /// Shows the player's country informations
    /// </summary>
    public void Show_CountryInfoPlayer()
    {
        countryInfoTab.Show_CountryInfo(manager.player);
    }

    /// <summary>
    /// Show a country's informations
    /// </summary>
    /// <param name="country">The target country</param>
    public void Show_CountryInfo(Pays country)
    {
        countryInfoTab.Show_CountryInfo(country);
    }


    /// <summary>
    /// Updates the current country informations
    /// </summary>
    public void UpdateInfo()
    {
        countryInfoTab.UpdateInfo();
    }


    /// <summary>
    /// Shows an event
    /// </summary>
    /// <param name="eventId">The event ID</param>
    public void OpenEvent(string eventId)
    {
        GameEvent gameEvent = manager.events[eventId];
        HideEverything();
        Timer.instance.StopTime();
        eventsRoot.SetActive(true);
        eventsTitle.text = gameEvent.title;

        Pays player = Manager.instance.player;

        eventsDesc.text = gameEvent.description.Replace("$currentRuler", player.leader.prenom + " " + player.leader.nom).Replace("$currentParty", player.parties[player.currentParty].partyName);
        currentEvent = gameEvent;

        for (int i = 0; i < 4; i++)
        {
            GameEvent.EventButton button = gameEvent.buttons[i];
            if (button == null) continue;

            eventsButtons[i].gameObject.SetActive(true);
            eventsButtons[i].GetComponentInChildren<Text>().text = button.label;
        }
    }

    /// <summary>
    /// Select the event outcome
    /// </summary>
    /// <param name="index">outcome index</param>
    public void ChoseEventOutcome(int index)
    {
        if (currentEvent == null) return;

        string[] effects = currentEvent.buttons[index].effects;
        string[] separators = { "(", ")" };

        bool hideEvent = true;
        foreach (string effect in effects)
        {
            string[] split = effect.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            switch (split[0])
            {
                case "CHANGE_GOVERNEMENT":
                    manager.player.reelected = false;
                    manager.player.Government_Form = int.Parse(split[1]);
                    manager.player.ResetFlag();
                    manager.player.ResetElections();
                    break;
                case "EVENT":
                    hideEvent = false;
                    OpenEvent(split[1]);
                    break;
                case "KEEPLEADER":
                    manager.player.reelected = true;
                    break;
                case "NEWLEADER":
                    manager.player.RandomizeLeader();
                    manager.player.reelected = false;
                    break;
            }
        }
        if (hideEvent)
        {
            Hide_Event();
        }
    }

    /// <summary>
    /// Hide the event
    /// </summary>
    public void Hide_Event()
    {
        Timer.instance.ResumeTime();

        manager.player.ResetFlag();
        countryInfoTab.Show_CountryInfo(manager.player);
        eventsRoot.SetActive(false);
        ShowDefault();
    }


    /// <summary>
    /// Updates relations between two countries
    /// </summary>
    /// <param name="A">Country A</param>
    /// <param name="B">Country B</param>
    /// <param name="st">Relation Index (0 = Peace, 1 = War, 2 = A vassalize B, 3 = B vassalize A)</param>
    public void UpdateRelations_ShortCut(Pays A, Pays B, int st)
    { //Met a jour A selon st, puis B en logique

        List<string> keys;
        switch (st)
        {
            case 0:
                A.MakePeaceWithCountry(B);
                if (B.provinces.Count == 0)
                {
                    countryInfoTab.Show_CountryInfo(A);
                }
                break;

            case 1:
                A.DeclareWarOnCountry(B);
                break;

            case 2:

                keys = new List<string>(B.atWarWith);
                foreach (string pays in keys)
                {
                    B.MakePeaceWithCountry(manager.GetCountry(pays));
                }

                B.lord = A;
                B.CopyCat(A);
                B.MimicColor(A);
                B.AddToTraversalOptions(A);
                A.AddToTraversalOptions(B);
                break;

            case 3:
                keys = new List<string>(A.atWarWith);
                foreach (string pays in keys)
                {
                    A.MakePeaceWithCountry(manager.GetCountry(pays));
                }

                A.lord = B;
                A.CopyCat(B);
                A.MimicColor(B);
                B.AddToTraversalOptions(A);
                A.AddToTraversalOptions(B);
                break;
        }

    }

    /// <summary>
    /// Show the province info tab
    /// </summary>
    /// <param name="c">The target province</param>
    public void ShowBuyUnit(Province c)
    {
        provinceTab.ShowProvinceDetails(c);
    }



    /// <summary>
    /// Updates the focus tree tab if it's open
    /// </summary>
    public void UpdateFocus()
    {
        if (focusMenu.isOpen)
        {
            focusMenu.ShowFocusMenu();
        }
    }

    /// <summary>
    /// Resets the focus tree tab
    /// </summary>
    public void ResetFocusTree()
    {
        focusMenu.Reset();
    }


    /// <summary>
    /// Open the focus tree tab
    /// </summary>
    public void OpenFocusTab()
    {
        focusMenu.OpenTab();
    }

    /// <summary>
    /// Hide every tabs 
    /// </summary>
    public void HideEverything()
    {
        if (countryInfoTab.isOpen) countryInfoTab.CloseTab();
        if (utilityTab.isOpen) utilityTab.CloseTab();
        if (provinceTab.isOpen) provinceTab.CloseTab();
        eventsRoot.SetActive(false);

        if (pauseTab.isOpen) pauseTab.CloseTab();
        if (focusMenu.isOpen) focusMenu.CloseTab();
        if (politicalTab.isOpen) politicalTab.CloseTab();
        if (federationTab.isOpen) federationTab.CloseTab();
        peaceDealRoot.SetActive(false);
    }

    /// <summary>
    /// Show the default tabs
    /// </summary>
    public void ShowDefault()
    {
        utilityTab.OpenTab();
    }

    /// <summary>
    /// Refresh the utility tab
    /// </summary>
    public void RefreshUtilityBar()
    {
        utilityTab.RefreshBar();
    }

}


