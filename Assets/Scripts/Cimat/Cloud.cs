using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private Vector3 target = new Vector3();
    private int speed = 2;

    [Header("Rain")]
    [SerializeField] private GameObject rainRoot;

    /// <summary>
    /// Initialize the cloud
    /// </summary>
    /// <param name="target">The cloud's target</param>
    /// <param name="speed">The cloud's speed</param>
    public void Init(Vector3 target, int speed)
    {
        this.target = target;
        this.speed = speed;
    }

    /// <summary>
    /// Removes the rain effects from the cloud
    /// </summary>
    public void RemoveRain()
    {
        Destroy(rainRoot);
    }

    void Update()
    {
        // Moves the cloud towards its target and deletes it if it has arrived

        if (!(target == transform.position))
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
        }
        else
        {
            Destroy(gameObject);
        }

    }
}
