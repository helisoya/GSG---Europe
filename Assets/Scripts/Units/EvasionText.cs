using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionText : MonoBehaviour
{
    private GameObject cam;

    private TMPro.TextMeshPro text;

    private Vector3 up;

    public float decaySpeed = 0.1f;

    void Start()
    {
        up = new Vector3(0, 1, 0);
        cam = Camera.main.gameObject;
        text = GetComponent<TMPro.TextMeshPro>();
        transform.LookAt(cam.transform.position);
    }


    void Update()
    {
        if (text.color.a > 0)
        {
            transform.Translate(up * Time.deltaTime);
            text.color -= new Color(0, 0, 0, Time.deltaTime * decaySpeed);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
