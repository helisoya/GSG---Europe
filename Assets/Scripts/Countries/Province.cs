using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// A province's type
/// </summary>
public enum ProvinceType
{
    NORMAL,
    COSTAL,
    SEA
}

/// <summary>
/// A province on the map
/// </summary>
public class Province : MonoBehaviour
{
    public int id;
    public string Province_Name;
    public ProvinceType type;
    private Pays _controller = null;
    private Pays _owner = null;
    public Vector3 center;
    public Province[] adjacencies;
    public bool hasRailroad;
    private bool hover;
    private List<Unit> _units;

    [Header("Canvas")]
    [SerializeField] private GameObject canvasRoot;
    [SerializeField] private GameObject prefabUnitGFX;
    [SerializeField] private Transform unitGfxRoot;
    private Dictionary<Pays, ProvinceCountryUnitGFX> unitgfxs;

    public List<Unit> units
    {
        get
        {
            return _units;
        }
    }

    public Pays controller
    {
        get
        {
            return _controller;
        }
    }

    public Pays owner
    {
        get
        {
            return _owner;
        }
    }

    [SerializeField] private Color seaColor;

    /// <summary>
    /// Changes the province's owner
    /// </summary>
    /// <param name="pays">The new owner</param>
    public void SetOwner(Pays pays)
    {
        if (type == ProvinceType.SEA) return;
        _owner = pays;
    }

    /// <summary>
    /// Changes the province's controller
    /// </summary>
    /// <param name="pays">The new controller</param>
    public void SetController(Pays pays)
    {
        if (type == ProvinceType.SEA) return;
        _controller = pays;
    }

    /// <summary>
    /// Add a unit to the province
    /// </summary>
    /// <param name="unit">The new unit</param>
    public void AddUnit(Unit unit)
    {
        units.Add(unit);

        if (!unitgfxs.ContainsKey(unit.country))
        {
            unitgfxs.Add(unit.country, new ProvinceCountryUnitGFX(prefabUnitGFX, unitGfxRoot));
        }
        unitgfxs[unit.country].AddUnit(unit);
    }

    /// <summary>
    /// Remove a unit from the province
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);

        unitgfxs[unit.country].RemoveUnit(unit);
    }

    /// <summary>
    /// Refresh the GFX for a country's unitType
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RefreshUnitGFX(Unit unit)
    {
        unitgfxs[unit.country].RefreshUnit(unit.info.type);
    }

    /// <summary>
    /// Refresh the country's flag of a country's units
    /// </summary>
    /// <param name="pays">The target country</param>
    public void RefreshCountryFlag(Pays pays)
    {
        if (unitgfxs.ContainsKey(pays))
        {
            unitgfxs[pays].RefreshFlag(pays.currentFlag);
        }
    }

    /// <summary>
    /// Compute the center of the province
    /// </summary>
    /// <param name="vecs">The vertices of the province</param>
    public void ComputeCenter(Vector3[] vecs)
    {
        center = Vector3.zero;

        float x = 0, y = 0, area = 0, k;
        Vector3 a, b = vecs[vecs.Length - 1];

        for (int i = 0; i < vecs.Length; i++)
        {
            a = vecs[i];

            k = a.z * b.x - a.x * b.z;
            area += k;
            x += (a.x + b.x) * k;
            y += (a.z + b.z) * k;

            b = a;
        }
        area *= 3;

        center.x = x / area;
        center.z = y / area;
        center.y = 0.3f;
    }


    /// <summary>
    /// Changes the province to a sea province
    /// </summary>
    public void SetAsSeaProvince()
    {
        type = ProvinceType.SEA;
        _controller = null;
        _owner = null;
        SetColor(seaColor, seaColor);
    }

    /// <summary>
    /// Spawns a unit inside the province
    /// </summary>
    public void SpawnUnitAtCity()
    {
        if (type == ProvinceType.SEA) return;
        owner.CreateUnit(this);
    }

    /// <summary>
    /// Initialize the country
    /// </summary>
    /// <param name="vecs">The vertices</param>
    public void Init(Vector3[] vecs)
    {
        hover = false;
        _units = new List<Unit>();
        ComputeCenter(vecs);

        // Initialise la creation de la province en polygone

        GetComponent<MeshFilter>().mesh = MeshGenerator.GenerateMesh(vecs);

        gameObject.AddComponent(typeof(MeshCollider));

        if (type == ProvinceType.SEA)
        {
            SetColor(seaColor, seaColor);
        }

        canvasRoot.transform.position = center + Vector3.up * 2;
        canvasRoot.GetComponent<Canvas>().worldCamera = Camera.main;
        unitgfxs = new Dictionary<Pays, ProvinceCountryUnitGFX>();
    }

    /// <summary>
    /// Changes the color of the province
    /// </summary>
    /// <param name="col1">Owner's color</param>
    /// <param name="col2">Controller's color</param>
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

    /// <summary>
    /// Refresh the province's color
    /// </summary>
    public void RefreshColor()
    {

        if (type == ProvinceType.SEA)
        {
            SetColor(seaColor, seaColor);
            return;
        }

        Manager manager = Manager.instance;
        if (manager.inPeaceDeal)
        {
            if (owner != manager.peaceDealSide1 && owner != manager.peaceDealSide2)
            {
                SetColor(MapModes.colors_grayed, MapModes.colors_grayed);
            }
            else if (manager.provincesToBeTakenInPeaceDeal.Contains(this))
            {
                SetColor(controller.color, controller.color);
            }
            else
            {
                SetColor(owner.color, controller.color);
            }
            return;
        }

        if (MapModes.currentMapMode == MapModes.MAPMODE.FEDERATION)
        {
            SetColor(
                owner.federation != null ? owner.federation.color : MapModes.colors_grayed,
                controller.federation != null ? controller.federation.color : MapModes.colors_grayed
                );
            return;
        }



        if (MapModes.currentMapMode == MapModes.MAPMODE.POLITICAL)
        {
            SetColor(owner.color, controller.color);
            return;
        }


        int indexOwner = 0;
        int indexController = 0;

        if (MapModes.currentMapMode == MapModes.MAPMODE.IDEOLOGICAL)
        {
            SetColor(MapModes.colors_ideologie[owner.Ideologie()], MapModes.colors_ideologie[controller.Ideologie()]);
        }
        else if (MapModes.currentMapMode == MapModes.MAPMODE.DIPLOMATIC)
        {
            if (owner == manager.player)
            {
                indexOwner = 4;
            }
            else
            {
                indexOwner = DetermineRelationToPlayer(owner);
            }

            if (controller == manager.player)
            {
                indexController = 4;
            }
            else
            {
                indexController = DetermineRelationToPlayer(controller);
            }

            Color ColOwner = MapModes.colors_relations[indexOwner];
            Color ColController = MapModes.colors_relations[indexController];
            if (indexOwner == 0)
            {
                int score = owner.relations[manager.player.ID].relationScore;
                ColOwner = Color.Lerp(Color.red, Color.green, (score + 100) / 200f);
            }
            if (indexController == 0)
            {
                int score = controller.relations[manager.player.ID].relationScore;
                ColController = Color.Lerp(Color.red, Color.green, (score + 100) / 200f);
            }
            SetColor(ColOwner, ColController);

        }
        else if (MapModes.currentMapMode == MapModes.MAPMODE.FORMABLE)
        {
            if (!manager.currentFormable.required.Contains(this))
            {
                SetColor(MapModes.colors_grayed, MapModes.colors_grayed);
                return;
            }

            indexOwner = owner == manager.player ? 0 : 1;
            indexController = controller == manager.player ? 0 : 1;
            SetColor(MapModes.colors_formable[indexOwner], MapModes.colors_formable[indexController]);
        }
    }

    /// <summary>
    /// Determine the Relation Index of a country toward the player
    /// </summary>
    /// <param name="p">The country</param>
    /// <returns>The relation index</returns>
    int DetermineRelationToPlayer(Pays p)
    {
        if (type == ProvinceType.SEA) return 0;

        if (p.lord == Manager.instance.player) return 2;
        if (Manager.instance.player.lord == p) return 3;
        if (p.relations[Manager.instance.player.ID].atWar) return 1;
        return 0;
    }

    /// <summary>
    /// OnClick Event
    /// </summary>
    void OnMouseDown()
    {
        if (type == ProvinceType.SEA) return;

        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            if (Manager.instance.inPeaceDeal && Manager.instance.peaceDealSide2 == owner && Manager.instance.player == controller)
            {
                CanvasWorker.instance.PeaceDealProvinceSelection(this);
                return;
            }
            else if (Manager.instance.inPeaceDeal)
            {
                return;
            }


            if (Manager.instance.picked)
            {
                CanvasWorker.instance.ShowBuyUnit(this);
            }
            else
            {
                CountryPicker.instance.UpdateCountry(owner);
            }



        }

    }


    /// <summary>
    /// On mouse enter event
    /// </summary>
    void OnMouseEnter()
    {

        if (Manager.instance.isLoading || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        hover = true;
        RefreshColor();
    }

    /// <summary>
    /// On mouse exit event
    /// </summary>
    void OnMouseExit()
    {
        if (Manager.instance.isLoading) return;
        hover = false;
        RefreshColor();
    }

}