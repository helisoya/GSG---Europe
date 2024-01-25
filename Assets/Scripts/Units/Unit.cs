using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;

public enum UnitType
{
    INFANTRY,
    TANK
}

/// <summary>
/// A unit is owned by a country and is used to conquer land
/// </summary>
public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public UnitTypeInfo info;
    private float HP;
    [SerializeField] private float timeToRegen = 2;
    private float travelStart;
    private bool selected;

    public float maxHp
    {
        get
        {
            return info.baseHP * country.bonusHP;
        }
    }

    public float currentHp
    {
        get
        {
            return HP;
        }
    }

    public float travelSpeed
    {
        get
        {
            return info.baseTravelSpeed * country.bonusSpeed;
        }
    }

    public float damage
    {
        get
        {
            return info.baseAttack * country.bonusDamage;
        }
    }

    public float defense
    {
        get
        {
            return info.baseDefense * country.bonusDefense;
        }
    }

    public float navalDamage
    {
        get
        {
            return info.baseNavelAttack * country.bonusNaval;
        }
    }


    [Header("Components")]
    [SerializeField] private GameObject selected_bar;
    [SerializeField] private GameObject land_part;
    [SerializeField] private GameObject sea_part;
    [SerializeField] private LineRenderer linePath;


    private Province currentProvince;
    private List<Renderer> renderers = new List<Renderer>();
    private GraphPath path;
    private int currentPathIndex;

    public Country country = null;
    private Manager manager;

    /// <summary>
    /// Initialize a unit
    /// </summary>
    /// <param name="startProv">Starting province</param>
    /// <param name="owner">Unit's owner</param>
    public void Init(Province startProv, Country owner)
    {
        country = owner;
        currentProvince = startProv;
        transform.position = startProv.center;


        manager = Manager.instance;
        manager.RegisterUnit(this);

        renderers = new List<Renderer>();

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
        UpdateFlag();

        HP = maxHp;

        currentProvince.AddUnit(this);
    }

    /// <summary>
    /// Set new target using pathfinding
    /// </summary>
    /// <param name="prov">Target province</param>
    /// <param name="additive">Add path to current path ?</param>
    public void SetNewTarget(Province prov, bool additive)
    {
        GraphPath newPath;

        if (additive && path.length > 0)
        {
            newPath = country.StartPathfinding(path.nodes[path.length - 1], prov);
            if (newPath == null) newPath = new GraphPath();

            foreach (Province node in newPath.nodes)
            {
                path.nodes.Add(node);
            }
            path.Bake();
        }
        else
        {
            newPath = country.StartPathfinding(currentProvince, prov);
            if (newPath == null) newPath = new GraphPath();

            path = newPath;

            if (path.length > 0)
            {
                transform.position = currentProvince.center;
                currentPathIndex = 1;
                travelStart = Time.time;
            }
            else
            {
                currentPathIndex = 0;
            }
        }
        RedrawLinePath();
    }

    /// <summary>
    /// Refreshs Unit GFX
    /// </summary>
    public void RefreshGFX()
    {
        currentProvince.RefreshUnitGFX(this);
    }

    /// <summary>
    /// Redraws the line path of the unit
    /// </summary>
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


    /// <summary>
    /// Refresh all renderers to show the unit or not
    /// </summary>
    /// <param name="value">Show the unit ?</param>
    public void UpdateIsSeen(bool value)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer) renderer.enabled = value;
        }
        currentProvince.SetUnitsVisible(value);
    }

    /// <summary>
    /// Gets the unit current damage
    /// </summary>
    /// <returns>The unit damage</returns>
    public float GetDmg()
    {
        if (currentProvince.type == ProvinceType.SEA) return navalDamage;
        return damage;
    }


    /// <summary>
    /// Gets if the unit is not moving
    /// </summary>
    /// <returns>Is the unit on standby ?</returns>
    public bool StandBy()
    {
        return path == null || path.length == 0 || currentPathIndex >= path.length;
    }

    /// <summary>
    /// Teleports a unit to a province
    /// </summary>
    /// <param name="prov">Target province</param>
    public void Teleport(Province prov)
    {
        ResetPath();
        currentProvince.RemoveUnit(this);
        currentProvince = prov;
        currentProvince.AddUnit(this);
        transform.position = currentProvince.center;
    }

    /// <summary>
    /// Checks if the unit is on a country's province
    /// </summary>
    /// <param name="id">Target country's ID</param>
    /// <returns>Is the unit on the country's territory ?</returns>
    public bool IsOnCountryTerritory(string id)
    {
        if (currentProvince == null || currentProvince.type == ProvinceType.SEA) return false;
        return currentProvince.owner.ID.Equals(id);
    }

    /// <summary>
    /// Resets the path
    /// </summary>
    public void ResetPath()
    {
        path = new GraphPath();
        currentPathIndex = 0;
    }

    /// <summary>
    /// Swap between sea and land graphics
    /// </summary>
    /// <param name="atSea">Is the unit at sea</param>
    void SwapParts(bool atSea)
    {
        sea_part.SetActive(atSea);
        land_part.SetActive(!atSea);
    }

    /// <summary>
    /// Unit logic
    /// </summary>
    void Update()
    {
        if (country.provinces.Count <= 0)
        {
            TakeDamage(maxHp + 1, 50);
        }

        bool canRegen = true;

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

                    if (!unit.TakeDamage(GetDmg(), 5))
                    {
                        if (TakeDamage(unit.GetDmg(), 2.5f)) return;
                    }
                    idx++;
                }
            }
            else if (ennemiesInNextProvince.Count == 0 && Time.time - travelStart >= travelSpeed)
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


        if (canRegen && HP < maxHp)
        {
            if (timeToRegen <= 0)
            {
                TakeDamage(-5, 1);
                timeToRegen = 2;
            }
            else
            {
                timeToRegen -= Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Takes damage
    /// </summary>
    /// <param name="dmg">Damage value</param>
    /// <param name="dmgMult">Damage multiplier</param>
    /// <returns>Is the unit dead ?</returns>
    public bool TakeDamage(float dmg, float dmgMult)
    {
        if (dmg > 0)
        {
            dmg = Mathf.Clamp(dmg - defense, 0, dmg) * Time.deltaTime * dmgMult;
            HP = Mathf.Clamp(HP - dmg, 0f, maxHp);

        }
        else if (dmg <= 0)
        {
            HP = Mathf.Clamp(HP - dmg, 0, maxHp);
        }

        currentProvince.RefreshUnitGFX(this);


        if (HP <= 0)
        {
            country.RemoveUnit(this);
            currentProvince.RemoveUnit(this);
            manager.UnRegisterUnit(this);
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Click Event
    /// </summary>
    public void Click_Event()
    {
        if (country == manager.player && !selected)
        {
            selected = true;
            selected_bar.SetActive(true);
            manager.selected_unit.Add(this);
        }
    }

    /// <summary>
    /// Unselect the unit
    /// </summary>
    public void UnSelect()
    {
        if (selected_bar == null) return;
        selected = false;
        selected_bar.SetActive(false);
    }

    /// <summary>
    /// Updates the unit flag
    /// </summary>
    public void UpdateFlag()
    {
        currentProvince.RefreshCountryFlag(country);
    }

    /// <summary>
    /// Find ennemy units on a province
    /// </summary>
    /// <param name="province">The target province</param>
    /// <returns>The list of ennemy units</returns>
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
