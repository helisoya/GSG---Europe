using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float HP = 100;
    [SerializeField] private float travelTime;
    [SerializeField] private float timeToRegen = 2;
    private float Max;
    private float travelStart;

    [Header("Components")]
    [SerializeField] private GameObject HP_Bar;
    [SerializeField] private GameObject selected_bar;
    [SerializeField] private GameObject land_part;
    [SerializeField] private GameObject sea_part;
    [SerializeField] private GameObject evasion_prefab;
    [SerializeField] private Renderer flag;
    [SerializeField] private LineRenderer linePath;
    bool disabled = false;

    private Province currentProvince;
    private List<Renderer> renderers = new List<Renderer>();
    private GraphPath path;
    private int currentPathIndex;

    private Pays country = null;
    private Manager manager;

    public void Init(Province startProv, Pays owner)
    {
        country = owner;
        currentProvince = startProv;
        transform.position = startProv.center;
        currentProvince.AddUnit(this);
    }

    public void SetSelectedBarActive(bool value)
    {
        selected_bar.SetActive(value);
    }

    void Start()
    {
        renderers = new List<Renderer>(GetComponents<Renderer>());

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (!renderers.Contains(renderer))
            {
                renderers.Add(renderer);
            }
        }

        if (Camera.main.transform.position.y > 250)
        {
            UpdateIsSeen(false);
        }


        ResetPath();

        manager = Manager.instance;

        UpdateFlag();

        if (manager.player == country)
        {
            HP_Bar.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 255);
        }

        HP = country.unit_hp;
        Max = HP;

        RefreshHp();
    }


    public void SetNewTarget(Province prov)
    {
        path = country.StartPathfinding(currentProvince, prov);
        if (path == null) path = new GraphPath();

        string aff = "";
        foreach (Province p in path.nodes)
        {
            aff += p.Province_Name + " -> ";
        }
        print(aff);


        if (path.nodes.Count > 0)
        {
            transform.position = currentProvince.center;
            currentPathIndex = 1;
            travelStart = Time.time;
        }
        else
        {
            currentPathIndex = 0;
        }

        RedrawLinePath();

    }

    public void RedrawLinePath()
    {

        if (StandBy())
        {
            linePath.positionCount = 0;
            linePath.SetPositions(new Vector3[0]);
            return;
        }

        Vector3[] pos = new Vector3[path.length - currentPathIndex + 1];
        linePath.positionCount = pos.Length;
        int posIdx = 0;

        for (int i = currentPathIndex - 1; i < path.length; i++)
        {
            pos[posIdx] = path.nodes[i].center;
            posIdx++;
        }

        linePath.SetPositions(pos);
    }

    public void RefreshHp()
    {
        RefreshHealthBar();
    }

    void UpdateAllRenderers(bool val)
    {
        foreach (MeshRenderer renderer in renderers)
        {
            renderer.enabled = val;
        }
    }

    public void UpdateIsSeen(bool value)
    {
        disabled = !value;
        UpdateAllRenderers(value);
    }

    public int GetDmg()
    {
        if (currentProvince.type == ProvinceType.SEA) return country.unit_navalDamage;
        return country.unit_damage;
    }

    public bool StandBy()
    {
        return path == null || path.length == 0 || currentPathIndex >= path.length;
    }

    public void Teleport(Province prov)
    {
        ResetPath();
        currentProvince = prov;
        transform.position = currentProvince.center;
    }

    public bool IsOnCountryTerritory(string id)
    {
        if (currentProvince == null) return false;
        return currentProvince.owner.ID.Equals(id);
    }

    public void ResetPath()
    {
        path = new GraphPath();
        currentPathIndex = 0;
    }

    void SwapParts(bool atSea)
    {
        sea_part.SetActive(atSea);
        land_part.SetActive(!atSea);
    }

    void Update()
    {
        if (country.provinces.Count <= 0)
        {
            TakeDamage(Max + 1);
        }

        bool canRegen = true;

        Vector3 lastPos = transform.position;
        if (!StandBy())
        {
            transform.LookAt(path.nodes[currentPathIndex].center);
            List<Unit> ennemiesInNextProvince = GetEnnemiesInProvince(path.nodes[currentPathIndex]);

            if (ennemiesInNextProvince.Count > 0)
            {
                canRegen = false;
                int idx = 0;
                while (idx < ennemiesInNextProvince.Count)
                {
                    Unit unit = ennemiesInNextProvince[idx];
                    unit.ResetPath();
                    float dmg = GetDmg() * Time.deltaTime * 10;

                    unit.TakeDamage(dmg);
                    idx++;
                }
            }
            else if (Time.time - travelStart >= travelTime)
            {

                currentProvince.RemoveUnit(this);
                currentProvince = path.nodes[currentPathIndex];
                currentProvince.AddUnit(this);
                transform.position = currentProvince.center;


                if (currentProvince.type == ProvinceType.SEA)
                {
                    SwapParts(true);
                }
                else
                {
                    SwapParts(false);
                    if (country.atWarWith.Contains(currentProvince.controller.ID) &&
                    (currentProvince.owner == currentProvince.controller || currentProvince.owner == country))
                    {
                        country.relations[currentProvince.controller.ID].warScores[country.ID] += 10;
                        country.relations[currentProvince.controller.ID].warScores[currentProvince.controller.ID] -= 10;
                        currentProvince.SetController(country);
                        currentProvince.RefreshColor();
                    }
                }

                travelStart = Time.time;
                currentPathIndex++;

                RedrawLinePath();
            }

        }

        HP_Bar.transform.LookAt(new Vector3(HP_Bar.transform.position.x, HP_Bar.transform.position.y, HP_Bar.transform.position.z - 50));


        if (Random.Range(0, 20) != 0)
        {
            return;
        }

        if (canRegen)
        {
            if (timeToRegen <= 0)
            {
                TakeDamage(-5);
                timeToRegen = 2;
            }
            else
            {
                timeToRegen -= Time.deltaTime;
            }
        }
        else
        {
            return;
        }
    }

    public void RefreshHealthBar()
    {
        float amount = (HP * 7) / Max;
        if (amount < 0)
        {
            return;
        }
        if (amount != HP_Bar.transform.localScale.x)
        {
            HP_Bar.transform.localScale = new Vector3(amount, HP_Bar.transform.localScale.y, HP_Bar.transform.localScale.z);
        }
    }

    public bool TakeDamage(float dmg)
    {
        if (dmg < 0 || Random.Range(0, 101) > country.unit_evasion)
        {
            HP = Mathf.Clamp(HP - dmg, 0f, Max);
        }
        else if (dmg >= 0)
        {
            GameObject obj = Instantiate(evasion_prefab, transform);
            obj.transform.position = transform.position;
        }


        if (HP <= 0)
        {
            country.RemoveUnit(this);
            currentProvince.RemoveUnit(this);
            Destroy(gameObject);
            return true;
        }
        else
        {
            RefreshHealthBar();
            return false;
        }
    }

    public void Click_Event()
    {
        if (country == manager.player)
        {
            selected_bar.SetActive(true);
            manager.selected_unit.Add(gameObject);
        }
    }


    public void UpdateFlag()
    {
        if (flag != null)
        {
            flag.material.mainTexture = country.currentFlag.texture;
        }

    }

    public List<Unit> GetEnnemiesInProvince(Province province)
    {
        List<Unit> units = new List<Unit>();

        foreach (Unit unit in province.units)
        {
            if (country.atWarWith.Contains(unit.country.ID))
            {
                units.Add(unit);
            }
        }

        return units;
    }

}
