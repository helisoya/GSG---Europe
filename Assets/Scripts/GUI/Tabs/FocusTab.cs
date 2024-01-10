using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A tab that shows focus trees
/// </summary>
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
    private Dictionary<string, GameObject> dic;


    private bool first = true;

    /// <summary>
    /// Resets if focus tree must be created
    /// </summary>
    public void Reset()
    {
        first = true;
    }

    /// <summary>
    /// Opens focus tree tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();
        ShowFocusMenu();
    }

    /// <summary>
    /// Closes focus tree tab
    /// </summary>
    public override void CloseTab()
    {
        base.CloseTab();
        Timer.instance.ResumeTime();
        GameGUI.instance.UpdateInfo();
    }

    /// <summary>
    /// Refresh the focus tree tab
    /// </summary>
    public void ShowFocusMenu()
    {
        if (!isOpen) return;

        Manager manager = Manager.instance;
        Country country = manager.player;

        Dictionary<string, Focus> focusTree = country.focusTree;



        if (first)
        {
            InitTree();
        }


        if (country.currentFocus != "NONE")
        {
            focusText.text = "Current : " + focusTree[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
        }
        else
        {
            focusText.text = "Current : None";
        }


        foreach (string id in dic.Keys)
        {
            dic[id].GetComponent<FocusButton>().Init(focusTree[id], FocusCase(focusTree[id]), this);
        }
    }

    /// <summary>
    /// Find the status index of the focus
    /// </summary>
    /// <param name="focus">The focus</param>
    /// <returns>Return the status index</returns>
    int FocusCase(Focus focus)
    {
        if (Manager.instance.player.focusDone.Contains(focus.id)) return 0;
        if (Manager.instance.player.currentFocus.Equals(focus.id)) return 1;
        if (Manager.instance.player.PrerequistDone(focus)) return 2;
        return 3;
    }

    /// <summary>
    /// Select a new focys
    /// </summary>
    /// <param name="focus">The focus ID</param>
    public void SelectFocus(string focus)
    {
        Manager manager = Manager.instance;
        Country country = manager.player;
        country.ChangeFocus(focus);
        focusText.text = "Current : " + country.focusTree[country.currentFocus].focusName + " (" + (country.maxFocusTime - country.currentFocusTime) + "/" + country.maxFocusTime + ")";
        ShowFocusMenu();
    }

    /// <summary>
    /// Initialize the focus tree
    /// </summary>
    void InitTree()
    {
        Country country = Manager.instance.player;
        Dictionary<string, Focus> focusTree = country.focusTree;

        barX.value = 0;
        barY.value = 1;
        first = false;
        dic = new Dictionary<string, GameObject>();

        foreach (Transform child in lineParent)
        {
            Destroy(child.parent);
        }

        foreach (Transform child in graphParent)
        {
            Destroy(child.parent);
        }


        foreach (Focus focus in country.focusTree.Values)
        {
            GameObject obj = Instantiate(prefabButton, graphParent);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(100 + 200 * focus.x, -100 - 100 * focus.y, 0);


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
}
