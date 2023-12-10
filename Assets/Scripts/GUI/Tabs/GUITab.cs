using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The parent class of all tabs
/// </summary>
public class GUITab : MonoBehaviour
{
    [SerializeField] protected GameObject tabRoot;

    public bool isOpen
    {
        get { return tabRoot.activeInHierarchy; }
    }

    /// <summary>
    /// Opens the tab
    /// </summary>
    public virtual void OpenTab()
    {
        tabRoot.SetActive(true);
    }

    /// <summary>
    /// Closes the tab
    /// </summary>
    public virtual void CloseTab()
    {
        tabRoot.SetActive(false);
    }
}
