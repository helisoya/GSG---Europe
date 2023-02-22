using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSelectTarget : MonoBehaviour
{
    private Camera cam;

    private Manager manager;

    public RectTransform selectionbox;

    Vector2 starpos;


    void Start()
    {
        cam = Camera.main;
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectionbox.gameObject.activeInHierarchy)
        {
            float width = Input.mousePosition.x - starpos.x;
            float height = Input.mousePosition.y - starpos.y;

            selectionbox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectionbox.anchoredPosition = starpos + new Vector2(width / 2, height / 2);

        }

        if (Input.GetMouseButtonDown(0))
        {

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            if (manager.picked)
            {
                selectionbox.gameObject.SetActive(true);
                starpos = Input.mousePosition;
                selectionbox.sizeDelta = Vector2.zero;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Unit")
                { // Click sur Unité
                    foreach (GameObject unit in manager.selected_unit)
                    {
                        if (unit != null)
                        {
                            unit.GetComponent<Unit>().selected_bar.SetActive(false);
                        }
                    }
                    manager.selected_unit = new List<GameObject>();
                    hit.transform.GetComponent<Unit>().Click_Event();
                }
                else if (hit.transform.tag == "Province")
                { // Click sur Ville
                    hit.transform.GetComponent<Province>().Click_Event();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {

            selectionbox.gameObject.SetActive(false);

            if (selectionbox.sizeDelta == Vector2.zero)
            {
                return;
            }

            foreach (GameObject unit in manager.selected_unit)
            {
                if (unit != null)
                {
                    unit.GetComponent<Unit>().selected_bar.SetActive(false);
                }
            }

            manager.selected_unit = new List<GameObject>();

            foreach (Unit unit in manager.player.units)
            {
                Vector3 pos = cam.WorldToScreenPoint(unit.transform.position);
                Vector2 min = selectionbox.anchoredPosition - (selectionbox.sizeDelta / 2);
                Vector2 max = selectionbox.anchoredPosition + (selectionbox.sizeDelta / 2);

                if (min.x <= pos.x && pos.x <= max.x &&
                min.y <= pos.y && pos.y <= max.y)
                {
                    unit.GetComponent<Unit>().Click_Event();
                }
            }


        }


        if (Input.GetMouseButtonDown(1))
        { // Deplacement Unité

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (manager.selected_unit != null)
                {
                    Vector3 pos = new Vector3(hit.point.x, 0.3f, hit.point.z);
                    foreach (GameObject unit in manager.selected_unit)
                    {
                        unit.GetComponent<Unit>().target = pos;
                    }
                }
            }
        }
    }
}
