using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railroad : MonoBehaviour
{
    [SerializeField] private string prov1;
    [SerializeField] private string prov2;

    void Start()
    {

        Vector3 pos1 = Manager.instance.GetProvince(prov1).center + Vector3.up;
        Vector3 pos2 = Manager.instance.GetProvince(prov2).center + Vector3.up;
        float distance = Vector3.Distance(pos1, pos2);

        transform.localScale = new Vector3(1, distance, 1);

        transform.position = (pos1 + pos2) / 2f;
        transform.up = (pos2 - pos1);
    }

}
