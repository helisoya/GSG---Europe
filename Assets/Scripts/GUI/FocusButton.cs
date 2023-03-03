using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FocusButton : MonoBehaviour
{
    private Focus focusName;

    private FocusTab menu;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI desc;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonGraphics;

    private int focusCase;

    public void Init(Focus focus, int focusCase, FocusTab focusMenu)
    {
        // Cases
        // 0 = Done
        // 1 = In progress
        // 2 = Can be done
        // 3 = Not available

        focusName = focus;
        menu = focusMenu;
        this.focusCase = focusCase;
        text.text = focus.focusName;
        desc.text = focus.desc;

        if (focusCase != 2 || !Manager.instance.player.currentFocus.Equals("NONE"))
        {
            button.interactable = false;
        }

        switch (focusCase)
        {
            case 0:
                buttonGraphics.color = Color.green;
                break;
            case 1:
                buttonGraphics.color = Color.yellow;
                break;
        }

    }

    public void OnClick()
    {
        if (focusCase == 2)
        {
            buttonGraphics.color = Color.yellow;
            menu.SelectFocus(focusName.id);
        }

    }
}
