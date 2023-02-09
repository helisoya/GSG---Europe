using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusMenu : MonoBehaviour
{
    public Text focusText;

    public Transform buttonParent;
    public GameObject prefabButton;

    public Manager manager;


    void Start()
    {
        manager = Manager.instance;
    }

    public void ShowFocusMenu()
    {
        Pays country = manager.GetCountry(manager.player);
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        if (country.currentFocus != "NONE")
        {
            focusText.text = "Current : " + manager.focus[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
        }
        else
        {
            focusText.text = "Current : None";
            List<Focus> available = country.GetAvailableFocus();
            foreach (Focus focus in available)
            {
                Instantiate(prefabButton, buttonParent).GetComponent<FocusButton>().Init(focus, this);
            }
        }
    }

    public void SelectFocus(string focus)
    {
        Pays country = manager.GetCountry(manager.player);
        country.ChangeFocus(focus);
        ShowFocusMenu();
    }
}
