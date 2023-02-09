using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public Vector3 target = new Vector3();
    public int speed = 2;


    void Update()
    {
        if(!(target==transform.position)){
            transform.position= Vector3.MoveTowards(transform.position,target,Time.deltaTime*speed);
        }else{
            Destroy(gameObject);
        }
        
    }
}
