using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        if (infoRoot.activeInHierarchy)
        {
            infoRoot.transform.position = Input.mousePosition;
        }
    }


    public void ShowInfo(string info)
    {
        infoRoot.SetActive(true);
        infoText.text = info;
    }

    public void HideInfo()
    {
        infoRoot.SetActive(false);
    }
}
