using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Selection : MonoBehaviour
{

    [SerializeField] private LayerMask uiMask;

    RaycastHit hit;

    bool dragSelect;

    //Collider variables
    //=======================================================//

    MeshCollider selectionBox;
    Mesh selectionMesh;

    Vector3 p1;
    Vector3 p2;

    //the corners of our 2d selection box
    Vector2[] corners;

    //the vertices of our meshcollider
    Vector3[] verts;
    Vector3[] vecs;

    // Start is called before the first frame update
    void Start()
    {
        dragSelect = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Manager.instance.picked) return;
        //1. when left mouse button clicked (but not released)
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            p1 = Input.mousePosition;
        }

        //2. while left mouse button held
        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if ((p1 - Input.mousePosition).magnitude > 40)
            {
                dragSelect = true;
            }
        }

        //3. when mouse button comes up
        if (Input.GetMouseButtonUp(0))
        {
            if (dragSelect == false) //single select
            {
                /**
                Ray ray = Camera.main.ScreenPointToRay(p1);

                if (Physics.Raycast(ray, out hit, 50000.0f))
                {
                    if (Input.GetKey(KeyCode.LeftShift)) //inclusive select
                    {
                        if (hit.transform.gameObject.tag == "Unit")
                        {
                            hit.transform.gameObject.GetComponent<Unit>().Click_Event();
                        }

                    }
                    else if (hit.transform.gameObject.layer != uiMask)
                    {
                        UnselectUnits();

                        if (hit.transform.gameObject.tag == "Unit")
                        {
                            hit.transform.gameObject.GetComponent<Unit>().Click_Event();
                        }
                        else if (hit.transform.tag == "Province")
                        { // Click sur Ville
                            hit.transform.GetComponent<Province>().Click_Event();
                        }
                    }
                }
                else //if we didnt hit something
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        //do nothing
                    }
                    else
                    {
                        UnselectUnits();
                    }
                }**/
            }
            else //marquee select
            {
                verts = new Vector3[4];
                vecs = new Vector3[4];
                p2 = Input.mousePosition;
                corners = getBoundingBox(p1, p2);



                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    UnselectUnits();
                }

                foreach (Unit unit in Manager.instance.player.units)
                {
                    Vector3 pos = Camera.main.WorldToScreenPoint(unit.transform.position);
                    if (corners[0].x <= pos.x && pos.x <= corners[3].x &&
                    corners[0].y >= pos.y && pos.y >= corners[3].y)
                    {
                        unit.Click_Event();
                    }
                }

                Destroy(selectionBox, 0.02f);

            }//end marquee select

            dragSelect = false;

        }

        if (Input.GetMouseButtonDown(1))
        { // Deplacement Unité

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Province prov = hit.transform.GetComponent<Province>();

                if (prov != null && Manager.instance.selected_unit != null)
                {
                    foreach (Unit unit in Manager.instance.selected_unit)
                    {
                        unit?.SetNewTarget(prov);
                    }
                }
            }
        }

    }

    void UnselectUnits()
    {
        foreach (Unit unit in Manager.instance.selected_unit)
        {
            unit?.UnSelect();
        }

        Manager.instance.selected_unit = new List<Unit>();
    }

    private void OnGUI()
    {
        if (dragSelect == true)
        {
            var rect = Utils.GetScreenRect(p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] getBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }

    //generate a mesh from the 4 bottom points
    Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Unit")
        {
            other.gameObject.GetComponent<Unit>().Click_Event();
        }
    }

}
