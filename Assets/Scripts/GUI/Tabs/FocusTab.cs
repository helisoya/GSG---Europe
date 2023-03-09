using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FocusTab : GUITab
{
    [SerializeField] protected GameObject focusMenuRoot;
    [SerializeField] protected TextMeshProUGUI focusText;
    [SerializeField] protected Transform graphParent;
    [SerializeField] protected Transform lineParent;
    [SerializeField] protected GameObject prefabButton;

    [SerializeField] protected Scrollbar barX;
    [SerializeField] protected Scrollbar barY;

    [SerializeField] protected GameObject linePrefab;

    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        ShowFocusMenu();
    }

    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        CanvasWorker.instance.UpdateInfo();
    }


    public void ShowFocusMenu()
    {
        if (!isOpen) return;

        Manager manager = Manager.instance;
        Pays country = manager.player;

        Dictionary<string, Focus> focusTree = country.focusTree;

        barX.value = 0;
        barY.value = 1;


        foreach (Transform child in graphParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in lineParent)
        {
            Destroy(child.gameObject);
        }


        if (country.currentFocus != "NONE")
        {
            focusText.text = "Current : " + focusTree[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
        }
        else
        {
            focusText.text = "Current : None";
        }

        Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();

        foreach (Focus focus in country.focusTree.Values)
        {
            GameObject obj = Instantiate(prefabButton, graphParent);
            obj.GetComponent<RectTransform>().position = new Vector3(500 + 500 * focus.x, 600 - 300 * focus.y, 0);
            obj.GetComponent<FocusButton>().Init(focus, FocusCase(focus), this);

            dic.Add(focus.id, obj);
        }

        List<Focus> exclusiveDone = new List<Focus>();

        foreach (Focus focus in focusTree.Values)
        {
            foreach (string requirement in focus.required)
            {
                Instantiate(linePrefab, lineParent).GetComponent<LineGUI>().SetObjects(dic[focus.id], dic[requirement], Color.white);
            }
            if (focus.exclusive.Count > 0 && !exclusiveDone.Contains(focus))
            {
                List<Focus> workWith = new List<Focus>();
                workWith.Add(focus);
                exclusiveDone.Add(focus);
                foreach (string exlcusive in focus.exclusive)
                {
                    workWith.Add(focusTree[exlcusive]);
                    exclusiveDone.Add(focusTree[exlcusive]);
                }

                workWith.Sort((e1, e2) =>
                {
                    return e1.x.CompareTo(e2.x);
                });

                for (int i = 0; i < workWith.Count - 1; i++)
                {
                    Instantiate(linePrefab, lineParent).GetComponent<LineGUI>().SetObjects(dic[workWith[i].id], dic[workWith[i + 1].id], Color.red);
                }
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
        focusText.text = "Current : " + country.focusTree[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
    }
}
