using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour
{
    public string nom;
    public Province region;

    private Manager manager;

    void Awake()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();

        region = transform.parent.GetComponent<Province>();

    }


    public void Click_Event()
    {
        if (region.owner == manager.player && manager.picked)
        {
            GameObject.Find("Canvas").GetComponent<CanvasWorker>().ShowBuyUnit(region);
        }
    }

}
