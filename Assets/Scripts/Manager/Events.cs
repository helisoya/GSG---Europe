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
        CanvasWorker.instance.Event("Governement Type", "We have changed our governement type. What should we do ?");
        Pays c = manager.GetCountry(ID);

        button1.onClick.AddListener(() =>
        {
            c.Government_Form = 0;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "Parlimentary Republic";
        button1.gameObject.SetActive(true);

        button2.onClick.AddListener(() =>
        {
            c.Government_Form = 1;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button2.transform.Find("Text").GetComponent<Text>().text = "Mixed Republic";
        button2.gameObject.SetActive(true);

        button3.onClick.AddListener(() =>
        {
            c.Government_Form = 2;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button3.transform.Find("Text").GetComponent<Text>().text = "Presidential Republic";
        button3.gameObject.SetActive(true);
    }


    public void ChoixType_Mon(string ID)
    {
        CanvasWorker.instance.Event("Governement Type", "We have changed our governement type. What should we do ?");
        Pays c = manager.GetCountry(ID);

        button1.onClick.AddListener(() =>
        {
            c.Government_Form = 3;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "Absolute Monarchy";
        button1.gameObject.SetActive(true);

        button2.onClick.AddListener(() =>
        {
            c.Government_Form = 4;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button2.transform.Find("Text").GetComponent<Text>().text = "Elective Monarchy";
        button2.gameObject.SetActive(true);

        button3.onClick.AddListener(() =>
        {
            c.Government_Form = 5;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button3.transform.Find("Text").GetComponent<Text>().text = "Parlimentary Monarchy";
        button3.gameObject.SetActive(true);
    }


    public void ChoixType_Com(string ID)
    {
        CanvasWorker.instance.Event("Governement Type", "We have changed our governement type. What should we do ?");
        Pays c = manager.GetCountry(ID);

        button1.onClick.AddListener(() =>
        {
            c.Government_Form = 6;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "Soviet Republic";
        button1.gameObject.SetActive(true);

        button2.onClick.AddListener(() =>
        {
            c.Government_Form = 7;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button2.transform.Find("Text").GetComponent<Text>().text = "People's Republic";
        button2.gameObject.SetActive(true);

        button3.onClick.AddListener(() =>
        {
            c.Government_Form = 8;
            c.Reset_Flag();
            c.Reset_Elections();
            CanvasWorker.instance.Hide_Event();
        });
        button3.transform.Find("Text").GetComponent<Text>().text = "Popular Union";
        button3.gameObject.SetActive(true);
    }


    public void ChoixType_Fas(string ID)
    {
        CanvasWorker.instance.Event("Governement Type", "We have changed our governement type. What should we do ?");
        Pays c = manager.GetCountry(ID);

        button1.onClick.AddListener(() =>
        {
            c.Government_Form = 9;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "New Reich";
        button1.gameObject.SetActive(true);

        button2.onClick.AddListener(() =>
        {
            c.Government_Form = 10;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button2.transform.Find("Text").GetComponent<Text>().text = "Social Republic";
        button2.gameObject.SetActive(true);

        button3.onClick.AddListener(() =>
        {
            c.Government_Form = 11;
            c.Reset_Flag();
            c.date_elections = -1;
            CanvasWorker.instance.Hide_Event();
        });
        button3.transform.Find("Text").GetComponent<Text>().text = "Military Junta";
        button3.gameObject.SetActive(true);
    }




    public void Elections(string ID, int index)
    {
        Pays c = manager.GetCountry(ID);
        CanvasWorker.instance.Event("Elections", "The election is finished. The new party in power is : " + c.parties[index].partyName + ". What should we do ?");
        GameObject holder = GameObject.Find("Canvas").transform.Find("Event").gameObject;


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
            button1.onClick.AddListener(() =>
            { // Rien Faire
                CanvasWorker.instance.Hide_Event();
            });
            button1.transform.Find("Text").GetComponent<Text>().text = "Perfect";
            button1.gameObject.SetActive(true);

            if (index == 1)
            { //Republique
                button2.onClick.AddListener(() =>
                {
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(1);
                    c.RandomizeLeader();
                });
                button2.transform.Find("Text").GetComponent<Text>().text = "Establish a republic";
                button2.gameObject.SetActive(true);
            }
            else if (index == 0)
            { //Communisme
                button2.onClick.AddListener(() =>
                {
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(3);
                    c.RandomizeLeader();
                });
                button2.transform.Find("Text").GetComponent<Text>().text = "Establish a soviet";
                button2.gameObject.SetActive(true);
            }
        }
        else
        {
            button1.onClick.AddListener(() =>
            { // Garder President
                c.reelected = true;
                CanvasWorker.instance.Hide_Event();
            });
            button1.transform.Find("Text").GetComponent<Text>().text = "Keep the president";
            button1.gameObject.SetActive(true);

            button2.onClick.AddListener(() =>
            { // Changer President
                c.reelected = false;
                c.RandomizeLeader();
                CanvasWorker.instance.Hide_Event();
            });
            button2.transform.Find("Text").GetComponent<Text>().text = "Get a new president";
            button2.gameObject.SetActive(true);

            if (c.reelected)
            { // Fonder Monarchie
                button3.onClick.AddListener(() =>
                {
                    c.reelected = false;
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(2);
                });
                button3.transform.Find("Text").GetComponent<Text>().text = "Establish a monarchy";
                button3.gameObject.SetActive(true);
            }
            else if (c.Government_Form >= 7 && index == 3)
            { // Communisme => Republique
                button3.onClick.AddListener(() =>
                {
                    c.reelected = false;
                    c.RandomizeLeader();
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(1);
                });
                button3.transform.Find("Text").GetComponent<Text>().text = "Establish a republic";
                button3.gameObject.SetActive(true);
            }
            else if (c.Government_Form >= 7 && index == 4)
            { // Communisme => Fascisme
                button3.onClick.AddListener(() =>
                {
                    c.reelected = false;
                    c.RandomizeLeader();
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(4);
                });
                button3.transform.Find("Text").GetComponent<Text>().text = "Establish fascism";
                button3.gameObject.SetActive(true);
            }
            else if (c.Government_Form <= 2 && index == 0)
            { // Republique => Communisme
                button3.onClick.AddListener(() =>
                {
                    c.reelected = false;
                    c.RandomizeLeader();
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(3);
                });
                button3.transform.Find("Text").GetComponent<Text>().text = "Establish a soviet";
                button3.gameObject.SetActive(true);
            }
            else if (c.Government_Form <= 2 && index == 4)
            { // Republique => Fascisme
                button3.onClick.AddListener(() =>
                {
                    c.reelected = false;
                    c.RandomizeLeader();
                    CanvasWorker.instance.Hide_Event();
                    c.Choix_Type(4);
                });
                button3.transform.Find("Text").GetComponent<Text>().text = "Establish fascism";
                button3.gameObject.SetActive(true);
            }

        }

    }


    public void DeathLeader_Normal(string ID)
    {
        Pays c = manager.GetCountry(ID);
        CanvasWorker.instance.Event("Ruler", "Our ruler is dead. A new one has been chosent : " + c.leader.prenom + " " + c.leader.nom + ".");
        GameObject holder = GameObject.Find("Canvas").transform.Find("Event").gameObject;
        button1.onClick.AddListener(() =>
        {
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "Perfect";
        button1.gameObject.SetActive(true);
    }

    public void DeathLeader_Monarchy(string ID)
    {
        Pays c = manager.GetCountry(ID);
        CanvasWorker.instance.Event("Monarch", "The monarch is dead, his child : " + c.leader.prenom + " will be the monarch now.");
        GameObject holder = GameObject.Find("Canvas").transform.Find("Event").gameObject;
        button1.onClick.AddListener(() =>
        {
            CanvasWorker.instance.Hide_Event();
        });
        button1.transform.Find("Text").GetComponent<Text>().text = "Long live the Monarch !";
        button1.gameObject.SetActive(true);
    }

}
