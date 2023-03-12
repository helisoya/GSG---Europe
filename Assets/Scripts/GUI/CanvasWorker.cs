using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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



    public void OpenSettingsMenu()
    {
        if (pauseTab.isOpen) return;
        HideEverything();
        pauseTab.OpenTab();
    }

    public void OpenPeaceDealTab(string side1, string side2)
    {
        HideEverything();
        Timer.instance.StopTime();
        manager.inPeaceDeal = true;
        manager.peaceDealSide1 = side1;
        manager.peaceDealSide2 = side2;
        manager.provincesToBeTakenInPeaceDeal = new List<Province>();
        peaceDealRoot.SetActive(true);
        manager.RefreshMap();
    }

    public void EndPeaceDeal()
    {
        HideEverything();
        ShowDefault();
        manager.EndPeaceDeal(peaceDealVassal.isOn);
    }

    public void PeaceDealProvinceSelection(Province prov)
    {
        if (!manager.provincesToBeTakenInPeaceDeal.Remove(prov))
        {
            manager.provincesToBeTakenInPeaceDeal.Add(prov);
        }
        prov.RefreshColor();
    }

    public void Show_CountryInfoPlayer()
    {
        countryInfoTab.Show_CountryInfo(manager.player);
    }

    public void Show_CountryInfo(Pays country)
    {
        countryInfoTab.Show_CountryInfo(country);
    }



    public void UpdateInfo()
    {
        countryInfoTab.UpdateInfo();
    }







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
                    manager.player.Reset_Flag();
                    manager.player.Reset_Elections();
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

    public void Hide_Event()
    {
        Timer.instance.ResumeTime();

        manager.player.Reset_Flag();
        countryInfoTab.Show_CountryInfo(manager.player);
        eventsRoot.SetActive(false);
        ShowDefault();
    }



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

                keys = new List<string>(B.atWarWith.Keys);
                foreach (string pays in keys)
                {
                    B.MakePeaceWithCountry(manager.GetCountry(pays));
                }

                B.lord = A;
                B.CopyCat(A);
                B.MimicColor(A);
                break;

            case 3:
                keys = new List<string>(A.atWarWith.Keys);
                foreach (string pays in keys)
                {
                    A.MakePeaceWithCountry(manager.GetCountry(pays));
                }

                A.lord = B;
                A.CopyCat(B);
                A.MimicColor(B);
                break;
        }

    }


    public void ShowBuyUnit(Province c)
    {
        provinceTab.ShowProvinceDetails(c);
    }




    public void UpdateFocus()
    {
        if (focusMenu.isOpen)
        {
            focusMenu.ShowFocusMenu();
        }
    }

    public void OpenFocusTab()
    {
        focusMenu.OpenTab();
    }

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

    public void ShowDefault()
    {
        utilityTab.OpenTab();
    }


    public void RefreshUtilityBar()
    {
        utilityTab.RefreshBar();
    }

}


