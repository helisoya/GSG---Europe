using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public float speed = 50;

    public int min_fov;

    public int middle;

    public int max_fov;

    public float speed_zoom;



    public GameObject border_up;
    public GameObject border_down;
    public GameObject border_left;
    public GameObject border_right;

    private float lastYPos;

    void Start()
    {
        lastYPos = transform.position.y;
    }

    void Update()
    {

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float y = Mathf.Clamp(transform.position.y + (-Input.GetAxis("Mouse ScrollWheel")) * speed_zoom * Time.unscaledDeltaTime, min_fov, max_fov);
            float angle = 0;
            if (y >= middle)
            {
                angle = 80;
            }
            else
            {
                angle = y * 80 / middle;
            }
            lastYPos = transform.position.y;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
            transform.eulerAngles = new Vector3(angle, transform.eulerAngles.y, transform.eulerAngles.z);

            if (lastYPos <= 250 && y > 250)
            {
                Manager.instance.UpdateMapGFXSeen(false);
            }
            else if (lastYPos >= 250 && y < 250)
            {
                Manager.instance.UpdateMapGFXSeen(true);
            }


        }

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            Vector3 Move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            transform.position += Move * speed * Time.unscaledDeltaTime;
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, border_left.transform.position.x + border_left.transform.localScale.x / 2, border_right.transform.position.x - border_right.transform.localScale.x / 2),
                transform.position.y,
                Mathf.Clamp(transform.position.z, border_down.transform.position.z + border_down.transform.localScale.z / 2, border_up.transform.position.z - border_up.transform.localScale.z / 2));
        }


    }
}
