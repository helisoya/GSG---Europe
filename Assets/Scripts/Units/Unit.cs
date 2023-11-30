using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{


    public Pays country = null;
    public float HP = 100;
    private int Max;
    private Manager manager;

    [SerializeField] private GameObject HP_Bar;
    [SerializeField] private GameObject selected_bar;
    [SerializeField] private GameObject land_part;
    [SerializeField] private GameObject sea_part;
    private float timeToRegen = 2;
    [SerializeField] private GameObject evasion_prefab;
    [SerializeField] private GameObject flag;
    bool disabled = false;

    private Province currentProvince;
    List<MeshRenderer> renderers = new List<MeshRenderer>();
    private GraphPath path;
    private int currentPathIndex;

    public void Init(Province startProv, Pays owner)
    {
        country = owner;
        currentProvince = startProv;
        transform.position = startProv.center;
    }

    public void SetSelectedBarActive(bool value)
    {
        selected_bar.SetActive(value);
    }

    void Start()
    {
        foreach (MeshRenderer renderer in GetComponents<MeshRenderer>())
        {
            if (!renderers.Contains(renderer))
            {
                renderers.Add(renderer);
            }
        }

        foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
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
        Max = (int)HP;

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

        currentPathIndex = 0;
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

        Vector3 lastPos = transform.position;
        if (!StandBy())
        {
            Vector3 target = path.nodes[currentPathIndex].center;

            if (!(target == transform.position))
            {
                transform.position = Vector3.MoveTowards(transform.position, target, country.unit_speed * Time.deltaTime);
            }
            else
            {
                currentProvince = path.nodes[currentPathIndex];

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



                currentPathIndex++;
            }

            transform.LookAt(target);
        }




        HP_Bar.transform.LookAt(new Vector3(HP_Bar.transform.position.x, HP_Bar.transform.position.y, HP_Bar.transform.position.z - 50));

        /*
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Province prov = hit.transform.gameObject.GetComponent<Province>();
            if (prov != null)
            { // Cas sur province

                if (land_part.activeInHierarchy == false)
                {
                    sea_part.SetActive(false);
                    land_part.SetActive(true);
                    isOnWater = false;
                }

                if (prov.controller == country) return;

                if (country.atWarWith.Contains(prov.controller.ID)
                    && (prov.owner == prov.controller || prov.owner == country))
                {
                    country.relations[prov.controller.ID].warScores[country.ID] += 10;
                    country.relations[prov.controller.ID].warScores[prov.controller.ID] -= 10;
                    prov.SetController(country);
                    prov.RefreshColor();


                    if (prov.owner != manager.player && prov.owner.units.Count > 0)
                    {
                        prov.owner.units[Random.Range(0, prov.owner.units.Count)].GetComponent<Unit>().SetNewTarget(prov);
                    }

                }
                else if (prov.owner != country && !(country.lord == prov.owner || prov.owner.lord == country))
                {
                    if (country == manager.player)
                    {
                        transform.position = lastPos;
                        ResetPath();
                    }
                    else
                    {
                        prov.SetController(country);
                        prov.RefreshColor();
                        country.DeclareWarOnCountry(prov.owner);
                        country.relations[prov.owner.ID].warScores[country.ID] += 10;
                    }
                }
            }
            else if (hit.transform.tag == "Border")
            { // Cas Bordure
                ResetPath();
                transform.position = lastPos;
            }
            else
            { // Cas hors province (Sur mer)
                if (hit.transform.gameObject.tag == "Map" && sea_part.activeInHierarchy == false && country.hasTech_Naval)
                {
                    sea_part.SetActive(true);
                    land_part.SetActive(false);
                    isOnWater = true;
                }
                else if (hit.transform.gameObject.tag == "Map" && !country.hasTech_Naval)
                {
                    ResetPath();
                    transform.position = lastPos;
                }
            }
        }
        else
        {
            ResetPath();
            transform.position = lastPos;
        }
        */

        if (Random.Range(0, 20) != 0)
        {
            return;
        }

        bool canRegen = true;

        foreach (string key in country.atWarWith)
        {
            foreach (Unit unit in manager.GetCountry(key).units)
            {
                float dist = Mathf.Sqrt(
                    Mathf.Pow((transform.position.x - unit.transform.position.x), 2) +
                    Mathf.Pow((transform.position.y - unit.transform.position.y), 2) +
                    Mathf.Pow((transform.position.z - unit.transform.position.z), 2)
                );

                if (dist <= 3)
                {
                    canRegen = false;
                    ResetPath();
                    unit.ResetPath();
                    float dmg_b = unit.GetDmg() * Time.deltaTime * 10;
                    TakeDamage(dmg_b);
                    if (HP <= 0)
                    {
                        return;
                    }
                }
            }
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
        if (HP_Bar == null)
        {
            HP_Bar = transform.Find("Bar").gameObject;
        }
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

    public void TakeDamage(float dmg)
    {
        if (dmg < 0 || Random.Range(0, 101) > country.unit_evasion)
        {
            HP = Mathf.Clamp(HP - dmg, 0, Max);
        }
        else if (dmg >= 0)
        {
            GameObject obj = Instantiate(evasion_prefab, transform);
            obj.transform.position = transform.position;
        }


        if (HP <= 0)
        {
            country.RemoveUnit(this);
            Destroy(gameObject);
        }
        else
        {
            RefreshHealthBar();
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
            flag.GetComponent<Renderer>().material.mainTexture = country.currentFlag.texture;
        }

    }

}
