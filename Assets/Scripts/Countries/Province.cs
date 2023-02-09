using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Province : MonoBehaviour
{

    public string controller = "000";
    public string owner = "000";
    public string Province_Name;


    private CanvasWorker canvas;

    private Manager manager;

    public Vector3 center;

    private bool hover;

    public void ComputeCenter(Vector3[] vecs)
    {
        Vector3 pos = Vector3.zero;
        foreach (Vector3 t in vecs)
        {
            pos += new Vector3(t.x, t.y, t.z);
        }
        center = pos / vecs.Length;
    }

    public void SpawnUnitAtCity()
    {
        Vector3 pos = (Vector3)Random.insideUnitCircle * 2;
        pos.z = pos.y;
        manager.GetCountry(owner).CreateUnit(pos + center);
    }

    public void Click_Event()
    {
        if (owner == manager.player && manager.picked)
        {
            GameObject.Find("Canvas").GetComponent<CanvasWorker>().ShowBuyUnit(this);
        }
    }

    public void Init(Vector3[] vecs)
    {
        hover = false;
        ComputeCenter(vecs);

        manager = GameObject.Find("Manager").GetComponent<Manager>();

        canvas = GameObject.Find("Canvas").GetComponent<CanvasWorker>();

        if (owner == "")
        {
            owner = "000";
        }

        // Initialise la creation de la province en polygone

        GetComponent<MeshFilter>().mesh = MeshGenerator.GenerateMesh(vecs);

        gameObject.AddComponent(typeof(MeshCollider));

    }


    public void SetColor(Color col1, Color col2)
    {
        if (hover)
        {
            col1 = new Color(
            Mathf.Clamp(col1.r + 0.1f, 0, 1),
            Mathf.Clamp(col1.g + 0.1f, 0, 1),
            Mathf.Clamp(col1.b + 0.1f, 0, 1));

            col2 = new Color(
            Mathf.Clamp(col2.r + 0.1f, 0, 1),
            Mathf.Clamp(col2.g + 0.1f, 0, 1),
            Mathf.Clamp(col2.b + 0.1f, 0, 1));
        }
        GetComponent<Renderer>().material.SetColor("_Color1", col1);
        GetComponent<Renderer>().material.SetColor("_Color2", col2);
    }


    public void RefreshColor()
    {
        Pays cOwner = Manager.instance.GetCountry(owner);
        Pays cController = Manager.instance.GetCountry(controller);

        if (manager.inPeaceDeal)
        {
            if (cOwner.ID != manager.peaceDealSide1 && cOwner.ID != manager.peaceDealSide2)
            {
                SetColor(manager.colors_formable[2], manager.colors_formable[2]);
            }
            else if (manager.provincesToBeTakenInPeaceDeal.Contains(this))
            {
                SetColor(cController.color, cController.color);
            }
            else
            {
                SetColor(cOwner.color, cController.color);
            }
            return;
        }



        if (manager.currentMapMode == Manager.MAPMODE.POLITICAL)
        {
            SetColor(cOwner.color, cController.color);
            return;
        }


        int indexOwner = 0;
        int indexController = 0;

        if (manager.currentMapMode == Manager.MAPMODE.IDEOLOGICAL)
        {
            indexOwner = cOwner.Ideologie();
            indexController = cController.Ideologie();
            SetColor(manager.colors_ideologie[indexOwner], manager.colors_ideologie[indexController]);
        }
        else if (manager.currentMapMode == Manager.MAPMODE.DIPLOMATIC)
        {
            if (owner == manager.player)
            {
                indexOwner = 4;
            }
            else
            {
                indexOwner = cOwner.relations[manager.player];
                if (indexOwner == 3)
                {
                    indexOwner = 2;
                }
                else if (indexOwner == 2)
                {
                    indexOwner = 3;
                }
            }

            if (controller == manager.player)
            {
                indexController = 4;
            }
            else
            {
                indexController = cController.relations[manager.player];
                if (indexController == 3)
                {
                    indexController = 2;
                }
                else if (indexController == 2)
                {
                    indexController = 3;
                }
            }
            SetColor(manager.colors_relations[indexOwner], manager.colors_relations[indexController]);

        }
        else if (manager.currentMapMode == Manager.MAPMODE.FORMABLE)
        {
            if (!manager.currentFormable.required.Contains(this))
            {
                SetColor(manager.colors_formable[2], manager.colors_formable[2]);
                return;
            }

            indexOwner = owner == manager.player ? 0 : 1;
            indexController = controller == manager.player ? 0 : 1;
            SetColor(manager.colors_formable[indexOwner], manager.colors_formable[indexController]);
        }
    }

    void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (manager.inPeaceDeal && manager.peaceDealSide2 == owner && manager.player == controller)
            {
                CanvasWorker.instance.PeaceDealProvinceSelection(this);
                return;
            }


            if (manager.picked)
            {
                canvas.Show_CountryInfo(owner);
            }
            else
            {
                canvas.transform.Find("Picker").GetComponent<CountryPicker>().UpdateCountry(owner);
            }

            if (owner == manager.player)
            {
                GameObject.Find("Canvas").GetComponent<CanvasWorker>().ShowBuyUnit(this);
            }

        }

    }

    void OnMouseEnter()
    {

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        hover = true;
        RefreshColor();
    }

    void OnMouseExit()
    {
        hover = false;
        RefreshColor();
    }

}