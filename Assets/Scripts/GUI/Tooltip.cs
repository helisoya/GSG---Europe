using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// A toottip used to provide informations on certain actions
/// </summary>
public class Tooltip : MonoBehaviour
{
    public static Tooltip instance;

    [SerializeField] private GameObject infoRoot;

    [SerializeField] private TextMeshProUGUI infoText;




    void Awake()
    {
        instance = this;
        HideInfo();
    }

    void Update()
    {
        // If the tooltip is active, move it to the mouse position
        if (infoRoot.activeInHierarchy)
        {
            infoRoot.transform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// Shows the tooltip
    /// </summary>
    /// <param name="info">The text to show</param>
    public void ShowInfo(string info)
    {
        infoRoot.SetActive(true);
        infoText.text = info;
    }

    /// <summary>
    /// Hide the tooltip
    /// </summary>
    public void HideInfo()
    {
        infoRoot.SetActive(false);
    }
}
