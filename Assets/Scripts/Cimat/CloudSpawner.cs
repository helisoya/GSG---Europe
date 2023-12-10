using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The CloudSpawner handles the creation of clouds
/// </summary>
public class CloudSpawner : MonoBehaviour
{

    [SerializeField]
    private GameObject cloudTemplate;

    [SerializeField] private int maxClouds = 20;
    private int currentClouds;

    void Update()
    {

        // Spawn a cloud randomly if the limit permits it

        if (Random.Range(1, 50) == 1 && maxClouds > currentClouds)
        {

            Vector3 vec;
            Vector3 target;

            if (Random.Range(1, 5) == 1)
            { // West
                vec = new Vector3(-550, 30, Random.Range(-430, 430));
                target = new Vector3(577, 30, Random.Range(-430, 430));
            }
            else if (Random.Range(1, 4) == 1)
            { // East
                vec = new Vector3(577, 30, Random.Range(-430, 430));
                target = new Vector3(-550, 30, Random.Range(-430, 430));
            }
            else if (Random.Range(1, 3) == 1)
            { // South
                vec = new Vector3(Random.Range(-430, 430), 30, -640);
                target = new Vector3(Random.Range(-430, 430), 30, 535);
            }
            else
            { // North
                vec = new Vector3(Random.Range(-430, 430), 30, 535);
                target = new Vector3(Random.Range(-430, 430), 30, -640);
            }

            Cloud cloud = Instantiate(cloudTemplate, vec, new Quaternion(0, 0, 0, 0), gameObject.transform).GetComponent<Cloud>();
            cloud.Init(target, Random.Range(1, 4));

            if (Random.Range(1, 3) == 1) // 33.33% chance to be not rainy
            {
                cloud.RemoveRain();
            }

            currentClouds++;
        }

    }
}
