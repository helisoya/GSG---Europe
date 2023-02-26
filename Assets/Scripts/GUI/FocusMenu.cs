using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FocusMenu : MonoBehaviour
{
    public Text focusText;

    public Transform graphParent;
    public Transform lineParent;
    public GameObject prefabButton;

    public Scrollbar barX;
    public Scrollbar barY;

    public GameObject linePrefab;

    public void ShowFocusMenu()
    {
        Manager manager = Manager.instance;
        Pays country = manager.player;

        barX.value = 0;
        barY.value = 1;


        foreach (Transform child in graphParent)
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
        }

        Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

        foreach (Focus focus in manager.focus.Values)
        {
            GameObject obj = Instantiate(prefabButton, graphParent);
            obj.GetComponent<RectTransform>().position = new Vector3(500 + 500 * focus.x, 600 - 300 * focus.y, 0);
            obj.GetComponent<FocusButton>().Init(focus, FocusCase(focus), this);

            dic.Add(focus.id, obj);
        }

        foreach (Focus focus in manager.focus.Values)
        {
            foreach (string requirement in focus.required)
            {
                // Draw Line between the two

                Instantiate(linePrefab, lineParent).GetComponent<LineGUI>().SetObjects(dic[focus.id], dic[requirement]);

            }
        }
    }


    int FocusCase(Focus focus)
    {
        if (Manager.instance.player.focusDone.Contains(focus.id)) return 0;
        if (Manager.instance.player.currentFocus.Equals(focus.id)) return 1;
        if (Manager.instance.player.PrerequistDone(focus)) return 2;
        return 3;
    }

    public void SelectFocus(string focus)
    {
        Manager manager = Manager.instance;
        Pays country = manager.player;
        country.ChangeFocus(focus);
        focusText.text = "Current : " + manager.focus[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
    }
}
