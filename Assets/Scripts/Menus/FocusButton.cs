using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FocusButton : MonoBehaviour
{
    private Focus focusName;

    private FocusMenu menu;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI desc;

    public void Init(Focus focus, FocusMenu focusMenu)
    {
        focusName = focus;
        menu = focusMenu;
        text.text = focus.focusName;
        desc.text = focus.desc;
    }

    public void OnClick()
    {
        menu.SelectFocus(focusName.id);
    }
}
