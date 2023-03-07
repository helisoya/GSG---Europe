using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public Vector3 target;

    public Pays country = null;

    public float HP = 100;

    private int Max;

    private GameObject HP_Bar;

    private Manager manager;

    public GameObject selected_bar;

    private GameObject land_part;

    private GameObject sea_part;

    private float timeToRegen = 2;

    public GameObject evasion_prefab;

    public GameObject flag;

    private bool isOnWater = false;

    bool disabled = false;

    List<MeshRenderer> renderers = new List<MeshRenderer>();


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


        target = transform.position;
        manager = Manager.instance;
        selected_bar = transform.Find("Selected").gameObject;

        land_part = transform.Find("Land").gameObject;
        sea_part = transform.Find("Sea").gameObject;

        HP_Bar = transform.Find("Bar").gameObject;

        UpdateFlag();

        if (manager.player == country)
        {
            HP_Bar.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 255);
        }

        HP = country.unit_hp;
        Max = ((int)HP);

        RefreshHp();
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
        if (isOnWater) return country.unit_navalDamage;
        return country.unit_damage;
    }

    public bool IsOnCountryTerritory(string id)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down, out hit);
        Province prov = hit.transform.gameObject.GetComponent<Province>();
        if (prov == null) return false;
        return prov.owner.Equals(id);
    }

    void Update()
    {




        if (country.provinces.Count <= 0)
        {
            TakeDamage(Max + 1);
        }

        Vector3 lastPos = transform.position;
        if (!(target == transform.position))
        {
            transform.position = Vector3.MoveTowards(transform.position, target, country.unit_speed * Time.deltaTime);
        }

        transform.LookAt(target);



        HP_Bar.transform.LookAt(new Vector3(HP_Bar.transform.position.x, HP_Bar.transform.position.y, HP_Bar.transform.position.z - 50));


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

                if (country.atWarWith.ContainsKey(prov.controller.ID)
                    && (prov.owner == prov.controller || prov.owner == country))
                {

                    prov.SetController(country);
                    prov.RefreshColor();


                    if (prov.owner != manager.player && prov.owner.units.Count > 0)
                    {
                        prov.owner.units[Random.Range(0, prov.owner.units.Count)].GetComponent<Unit>().target = transform.position;
                    }

                }
                else if (prov.owner != country && !(country.lord == prov.owner || prov.owner.lord == country))
                {
                    if (country == manager.player)
                    {
                        transform.position = lastPos;
                    }
                    else
                    {
                        prov.SetController(country);
                        prov.RefreshColor();
                        country.DeclareWarOnCountry(prov.owner);
                    }
                    target = transform.position;
                }
            }
            else if (hit.transform.tag == "Border")
            { // Cas Bordure
                target = lastPos;
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
                    target = lastPos;
                    transform.position = lastPos;
                }
            }
        }
        else
        {
            target = lastPos;
            transform.position = lastPos;
        }

        if (Random.Range(0, 20) != 0)
        {
            return;
        }

        bool canRegen = true;

        foreach (string key in country.atWarWith.Keys)
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
                    target = transform.position;
                    unit.target = unit.transform.position;
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
