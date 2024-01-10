using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the game's events
/// </summary>
public class Events : MonoBehaviour
{
    private Manager manager;

    [SerializeField] private GameObject holder;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;

    void Start()
    {
        manager = GetComponent<Manager>();
    }

    /// <summary>
    /// Show an event
    /// </summary>
    /// <param name="ID">the event ID</param>
    public void ShowEvent(string ID)
    {
        GameGUI.instance.OpenEvent(ID);
    }

    /// <summary>
    /// Shows the Republic's subtypes events
    /// </summary>
    public void ChoixType_Rep()
    {
        GameGUI.instance.OpenEvent("CHANGE_GOVERNEMENT_REPUBLIC");
    }

    /// <summary>
    /// Shows the Monarchy's subtypes events
    /// </summary>
    public void ChoixType_Mon()
    {
        GameGUI.instance.OpenEvent("CHANGE_GOVERNEMENT_MONARCHY");
    }

    /// <summary>
    /// Shows the Communist's subtypes events
    /// </summary>
    public void ChoixType_Com()
    {
        GameGUI.instance.OpenEvent("CHANGE_GOVERNEMENT_COMMUNISM");
    }

    /// <summary>
    /// Shows the Fascist's subtypes events
    /// </summary>
    public void ChoixType_Fas()
    {
        GameGUI.instance.OpenEvent("CHANGE_GOVERNEMENT_FASCISM");
    }



    /// <summary>
    /// Show the election event
    /// </summary>
    /// <param name="ID">Target country's ID</param>
    /// <param name="index">Election winner's index</param>
    public void Elections(string ID, int index)
    {
        Country c = manager.GetCountry(ID);

        // Si monarchie
        // Parti Gauche/Extr.Gauche peut renverser le gouvernement
        // Sinon Rien
        //
        // Si Republique
        // Parti Extr peuvent renverser le gouvernement
        //
        // Si Communiste
        // Parti de Droite peuvent renverser le gouvernement

        if (c.Government_Form == 5)
        { // Monarchie Parlementaire

            if (index == (int)PartyType.COMMUNIST)
            {
                GameGUI.instance.OpenEvent("ELECTIONS_MONARCHY_COMMUNISM");
            }
            else if (index == (int)PartyType.SOCIALIST)
            {
                GameGUI.instance.OpenEvent("ELECTIONS_MONARCHY_SOCIALISM");
            }
            else
            {
                GameGUI.instance.OpenEvent("ELECTIONS_NORMAL");
            }
        }
        else
        {
            if (c.Government_Form <= 2)
            {
                if (index == (int)PartyType.COMMUNIST)
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_REPUBLIC_COMMUNISM");
                }
                else if (index == (int)PartyType.FASCIST)
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_REPUBLIC_FASCISM");
                }
                else if (c.reelected)
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_REPUBLIC_REELECTED");
                }
                else
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_REPUBLIC_NORMAL");
                }
            }
            else if (c.Government_Form <= 8)
            {
                if (index == (int)PartyType.CONSERVATIVE)
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_COMMUNISM_REPUBLIC");
                }
                else if (index == (int)PartyType.FASCIST)
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_COMMUNISM_FASCISM");
                }
                else
                {
                    GameGUI.instance.OpenEvent("ELECTIONS_NORMAL");
                }
            }
        }

    }

    /// <summary>
    /// Show leader's death event (normal)
    /// </summary>
    public void DeathLeader_Normal()
    {
        GameGUI.instance.OpenEvent("DEATH_LEADER_NORMAL");
    }


    /// <summary>
    /// Show leader's death event (monarchy)
    /// </summary>
    public void DeathLeader_Monarchy()
    {
        GameGUI.instance.OpenEvent("DEATH_LEADER_MONARCH");
    }

}
