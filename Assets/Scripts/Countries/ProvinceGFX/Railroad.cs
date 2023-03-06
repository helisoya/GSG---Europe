using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railroad : MonoBehaviour
{
    [SerializeField] private GameObject root;
    public void Init(string prov1, string prov2)
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


    public void UpdateIsSeen(bool value)
    {
        root.SetActive(value);
    }
}
