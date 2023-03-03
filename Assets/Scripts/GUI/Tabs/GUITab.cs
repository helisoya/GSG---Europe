using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITab : MonoBehaviour
{
    [SerializeField] protected GameObject tabRoot;

    public bool isOpen
    {
        get { return tabRoot.activeInHierarchy; }
    }

    public virtual void OpenTab()
    {
        tabRoot.SetActive(true);
    }


    public virtual void CloseTab()
    {
        tabRoot.SetActive(false);
    }
}
