using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Railroads are GFX that appears between two provinces (WIP)
/// </summary>
public class Railroad : MonoBehaviour
{
    [SerializeField] private GameObject root;


    /// <summary>
    /// Initialize a railroad between two provinces
    /// </summary>
    /// <param name="prov1">Province 1</param>
    /// <param name="prov2">Province 2</param>
    public void Init(int prov1, int prov2)
    {
        Vector3 pos1 = Manager.instance.GetProvince(prov1).center + new Vector3(0, 0.02f, 0);
        Vector3 pos2 = Manager.instance.GetProvince(prov2).center + new Vector3(0, 0.02f, 0);
        float distance = Vector3.Distance(pos1, pos2);

        transform.localScale = new Vector3(1, 1, distance);

        transform.position = (pos1 + pos2) / 2f;
        transform.LookAt(pos1);


        if (Camera.main.transform.position.y > 250)
        {
            UpdateIsSeen(false);
        }
    }

    /// <summary>
    /// Hide the Railroad
    /// </summary>
    /// <param name="value">Can be seen</param>
    public void UpdateIsSeen(bool value)
    {
        root.SetActive(value);
    }
}
