using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles Camera movements and zoom
/// </summary>
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
        if (!GameGUI.instance.canMoveCamera) return;

        // Camera Zoom

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float y = Mathf.Clamp(transform.position.y + (-Input.GetAxis("Mouse ScrollWheel")) * speed_zoom * Time.unscaledDeltaTime, min_fov, max_fov);
            float angle;
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


            // Hide mapGFX if too far away
            if (lastYPos <= 250 && y > 250)
            {
                Manager.instance.UpdateMapGFXSeen(false);
            }
            else if (lastYPos >= 250 && y < 250)
            {
                Manager.instance.UpdateMapGFXSeen(true);
            }
        }


        // Camera Movements

        Vector3 move = new Vector3(0, 0, 0);

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            // Movement via keyboard

            move.x = Input.GetAxisRaw("Horizontal");
            move.z = Input.GetAxisRaw("Vertical");
        }
        /**
        else
        {
            // Movement via mouse

            Vector3 mousePos = Input.mousePosition;
            if (mousePos.x <= 25)
            {
                move.x = -1;
            }
            else if (mousePos.x >= Screen.width - 25)
            {
                move.x = 1;
            }
            if (mousePos.y <= 25)
            {
                move.z = -1;
            }
            else if (mousePos.y >= Screen.height - 25)
            {
                move.z = 1;
            }
        }

        if (move.x != 0 || move.z != 0)
        {
            transform.position += move * speed * Time.unscaledDeltaTime;
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, border_left.transform.position.x + border_left.transform.localScale.x / 2, border_right.transform.position.x - border_right.transform.localScale.x / 2),
                transform.position.y,
                Mathf.Clamp(transform.position.z, border_down.transform.position.z + border_down.transform.localScale.z / 2, border_up.transform.position.z - border_up.transform.localScale.z / 2));
        }
        **/
    }


}
