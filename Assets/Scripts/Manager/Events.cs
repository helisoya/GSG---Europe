using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void ChoixType_Rep(string ID)
    {
        CanvasWorker.instance.OpenEvent("CHANGE_GOVERNEMENT_REPUBLIC");
    }


    public void ChoixType_Mon(string ID)
    {
        CanvasWorker.instance.OpenEvent("CHANGE_GOVERNEMENT_MONARCHY");
    }


    public void ChoixType_Com(string ID)
    {
        CanvasWorker.instance.OpenEvent("CHANGE_GOVERNEMENT_COMMUNISM");
    }


    public void ChoixType_Fas(string ID)
    {
        CanvasWorker.instance.OpenEvent("CHANGE_GOVERNEMENT_FASCISM");
    }




    public void Elections(string ID, int index)
    {
        Pays c = manager.GetCountry(ID);

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

            if (index == 0)
            {
                CanvasWorker.instance.OpenEvent("ELECTIONS_MONARCHY_COMMUNISM");
            }
            else if (index == 1)
            {
                CanvasWorker.instance.OpenEvent("ELECTIONS_MONARCHY_SOCIALISM");
            }
            else
            {
                CanvasWorker.instance.OpenEvent("ELECTIONS_NORMAL");
            }
        }
        else
        {
            if (c.Government_Form <= 2)
            {
                if (index == 0)
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_REPUBLIC_COMMUNISM");
                }
                else if (index == 5)
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_REPUBLIC_FASCISM");
                }
                else if (c.reelected)
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_REPUBLIC_REELECTED");
                }
                else
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_REPUBLIC_NORMAL");
                }
            }
            else if (c.Government_Form <= 8)
            {
                if (index == 4)
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_COMMUNISM_REPUBLIC");
                }
                else if (index == 5)
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_COMMUNISM_FASCISM");
                }
                else
                {
                    CanvasWorker.instance.OpenEvent("ELECTIONS_NORMAL");
                }
            }
        }

    }


    public void DeathLeader_Normal(string ID)
    {
        Pays c = manager.GetCountry(ID);
        CanvasWorker.instance.OpenEvent("DEATH_LEADER_NORMAL");
    }

    public void DeathLeader_Monarchy(string ID)
    {
        Pays c = manager.GetCountry(ID);
        CanvasWorker.instance.OpenEvent("DEATH_LEADER_MONARCH");
    }

}
