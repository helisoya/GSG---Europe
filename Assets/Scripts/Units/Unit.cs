using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public Vector3 target;

    public string country = "000";

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

    private Pays pays;


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


        target = transform.position;
        manager = Manager.instance;
        selected_bar = transform.Find("Selected").gameObject;

        land_part = transform.Find("Land").gameObject;
        sea_part = transform.Find("Sea").gameObject;

        HP_Bar = transform.Find("Bar").gameObject;

        UpdateFlag();
        pays = manager.GetCountry(country);

        if (manager.player == country)
        {
            HP_Bar.GetComponent<Renderer>().material.color = new Color32(0, 255, 0, 255);
        }

        HP = pays.unit_hp;
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

    public void Check()
    {
        if (isSeen())
        {
            if (disabled)
            {
                disabled = false;
                UpdateAllRenderers(true);
            }
        }
        else if (!disabled)
        {
            disabled = true;
            UpdateAllRenderers(false);
        }
    }

    public int GetDmg()
    {
        if (isOnWater) return pays.unit_navalDamage;
        return pays.unit_damage;
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




        if (manager.GetCountry(country).provinces.Count <= 0)
        {
            TakeDamage(Max + 1);
        }

        Vector3 lastPos = transform.position;
        if (!(target == transform.position))
        {
            transform.position = Vector3.MoveTowards(transform.position, target, pays.unit_speed * Time.deltaTime);
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

                if (pays.atWarWith.ContainsKey(prov.controller)
                    && (prov.owner == prov.controller || prov.owner == country))
                {

                    prov.controller = country;
                    prov.RefreshColor();


                    if (prov.owner != manager.player && manager.GetCountry(prov.owner).units.Count > 0)
                    {
                        manager.GetCountry(prov.owner).units[Random.Range(0, manager.GetCountry(prov.owner).units.Count)].GetComponent<Unit>().target = transform.position;
                    }

                }
                else if (prov.owner != country && !(pays.relations[prov.owner] >= 2))
                {
                    if (country == manager.player)
                    {
                        transform.position = lastPos;
                    }
                    else
                    {
                        prov.controller = country;
                        prov.RefreshColor();
                        pays.DeclareWarOnCountry(prov.owner);
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
                if (hit.transform.gameObject.tag == "Map" && sea_part.activeInHierarchy == false && pays.hasTech_Naval)
                {
                    sea_part.SetActive(true);
                    land_part.SetActive(false);
                    isOnWater = true;
                }
                else if (hit.transform.gameObject.tag == "Map" && !pays.hasTech_Naval)
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

        foreach (string key in pays.atWarWith.Keys)
        {
            foreach (GameObject unit in manager.GetCountry(key).units)
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
                    unit.GetComponent<Unit>().target = unit.transform.position;
                    float dmg_b = unit.GetComponent<Unit>().GetDmg() * Time.deltaTime * 10;
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
        if (dmg < 0 || Random.Range(0, 101) > pays.unit_evasion)
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
            manager.GetCountry(country).RemoveUnit(gameObject);
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
            flag.GetComponent<Renderer>().material.mainTexture = manager.GetCountry(country).currentFlag.texture;
        }

    }

    bool isSeen()
    {
        return Camera.main.transform.position.y <= 250;
    }


}
