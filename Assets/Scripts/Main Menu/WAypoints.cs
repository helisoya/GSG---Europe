using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WAypoints : MonoBehaviour
{
    public float speed;

    public Transform start;

    public Transform end;

    void Start()
    {
        transform.position = start.position;
    }

    
    void Update()
    {
        if(transform.position!=end.position){
            transform.position = Vector3.MoveTowards(transform.position,end.position,Time.deltaTime*speed);
        }else{
            transform.position = start.position;
        }
    }
}
