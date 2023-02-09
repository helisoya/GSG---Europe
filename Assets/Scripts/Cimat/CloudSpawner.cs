using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject cloud_template;

    [SerializeField]
    private int max_clouds = 20;
   
    void Update()
    {
        if(Random.Range(1,50)==1 && max_clouds>gameObject.transform.childCount){

            Vector3 vec;
            Vector3 target;

            if(Random.Range(1,5)==1){ // Ouest
                vec = new Vector3(-550,30,Random.Range(-430,430));
                target = new Vector3(577,30,Random.Range(-430,430));
            }else if (Random.Range(1,4)==1){ // Est
                vec = new Vector3(577,30,Random.Range(-430,430));
                target = new Vector3(-550,30,Random.Range(-430,430));
            }else if (Random.Range(1,3)==1){ // Sud
                vec = new Vector3(Random.Range(-430,430),30,-640);
                target = new Vector3(Random.Range(-430,430),30,535);
            }else{ // Nord
                vec = new Vector3(Random.Range(-430,430),30,535);
                target = new Vector3(Random.Range(-430,430),30,-640);
            }

            GameObject cloud = Instantiate(cloud_template,vec,new Quaternion(0,0,0,0),gameObject.transform);
            cloud.GetComponent<Cloud>().target=target;
            cloud.GetComponent<Cloud>().speed = Random.Range(1,4);
            if(Random.Range(1,3)==1){
                Destroy(cloud.transform.Find("Rain").gameObject);
                Destroy(cloud.transform.Find("Puddle").gameObject);
            }
        }

    }
}
