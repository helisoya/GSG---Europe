using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Units GFX from a Country, inside a province
/// </summary>
public class ProvinceCountryUnitGFX
{
    private Dictionary<UnitType, ProvinceUnitGfx> unitgfxs;
    private GameObject prefab;
    private Transform root;


    /// <summary>
    /// Creates a ProvinceCountryUnitGFX
    /// </summary>
    /// <param name="prefab">Unit row prefab</param>
    /// <param name="root">Where to add the rows</param>
    public ProvinceCountryUnitGFX(GameObject prefab, Transform root)
    {
        this.prefab = prefab;
        this.root = root;
        unitgfxs = new Dictionary<UnitType, ProvinceUnitGfx>();
    }


    /// <summary>
    /// Add a new unit
    /// </summary>
    /// <param name="unit">The new unit</param>
    public void AddUnit(Unit unit)
    {

        if (!unitgfxs.ContainsKey(unit.info.type))
        {
            ProvinceUnitGfx gfx = Object.Instantiate(prefab, root).GetComponent<ProvinceUnitGfx>();
            gfx.Init(unit.info.type, unit.country);
            gfx.AddUnit(unit);
            unitgfxs.Add(unit.info.type, gfx);
        }
        else
        {
            unitgfxs[unit.info.type].AddUnit(unit);
        }
    }

    /// <summary>
    /// Removes a unit
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RemoveUnit(Unit unit)
    {

        unitgfxs[unit.info.type].RemoveUnit(unit);

        if (unitgfxs[unit.info.type].Count == 0)
        {
            Object.Destroy(unitgfxs[unit.info.type].gameObject);
            unitgfxs.Remove(unit.info.type);
        }
    }

    /// <summary>
    /// Refresh a certain unitType's GFX
    /// </summary>
    /// <param name="type">The UnitType</param>
    public void RefreshUnit(UnitType type)
    {
        unitgfxs[type].RefreshGFX();
    }


    /// <summary>
    /// Refresh the flags of all GFX
    /// </summary>
    /// <param name="newSprite">The new flag</param>
    public void RefreshFlag(Sprite newSprite)
    {
        foreach (ProvinceUnitGfx unit in unitgfxs.Values)
        {
            unit.RefreshFlag(newSprite);
        }
    }


}
