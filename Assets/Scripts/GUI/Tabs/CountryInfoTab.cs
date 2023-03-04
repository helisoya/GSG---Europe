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
    [SerializeField] private GameObject infoDiploRoot;
    [SerializeField] private Button infoDiploWar;
    [SerializeField] private Button infoDiploVassal;
    [SerializeField] private Button infoDiploPeace;


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

        infoDiploRoot.SetActive(false);

        if (NEW != manager.player)
        { // Cas nation IA
            infoDiploRoot.SetActive(true);
            infoDiploPeace.onClick.RemoveAllListeners();
            infoDiploWar.onClick.RemoveAllListeners();
            infoDiploVassal.onClick.RemoveAllListeners();

            infoDiploPeace.onClick.AddListener(() =>
            {
                CanvasWorker.instance.OpenPeaceDealTab(manager.player.ID, NEW.ID);
                //UpdateRelations_ShortCut(manager.player, NEW, 0);
                //Show_CountryInfo(NEW);
            });
            infoDiploWar.onClick.AddListener(() =>
            {
                CanvasWorker.instance.UpdateRelations_ShortCut(manager.player, NEW, 1);
                Show_CountryInfo(NEW);
            });
            infoDiploVassal.onClick.AddListener(() =>
            {
                Pays old = manager.player;
                manager.player = NEW;
                old.RefreshProvinces();
                NEW.RefreshProvinces();
                Show_CountryInfo(NEW);
            });

            if (manager.player.relations[NEW.ID] == 1)
            { // Pays en guerre
                infoDiploPeace.gameObject.SetActive(true);
                infoDiploWar.gameObject.SetActive(false);
                infoDiploVassal.gameObject.SetActive(false);
            }
            else if (manager.player.relations[NEW.ID] == 2)
            { // Cas Vassal
                infoDiploPeace.gameObject.SetActive(false);
                infoDiploWar.gameObject.SetActive(false);
                infoDiploVassal.gameObject.SetActive(true);
            }
            else
            {
                infoDiploPeace.gameObject.SetActive(false);
                infoDiploWar.gameObject.SetActive(true);
                infoDiploVassal.gameObject.SetActive(false);
            }
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
}
