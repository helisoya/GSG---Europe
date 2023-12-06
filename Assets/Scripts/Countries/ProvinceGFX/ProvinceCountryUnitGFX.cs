using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvinceCountryUnitGFX
{
    private Dictionary<UnitType, ProvinceUnitGfx> unitgfxs;
    private GameObject prefab;
    private Transform root;

    public ProvinceCountryUnitGFX(GameObject prefab, Transform root)
    {
        this.prefab = prefab;
        this.root = root;
        unitgfxs = new Dictionary<UnitType, ProvinceUnitGfx>();
    }

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

    public void RemoveUnit(Unit unit)
    {

        unitgfxs[unit.info.type].RemoveUnit(unit);

        if (unitgfxs[unit.info.type].Count == 0)
        {
            Object.Destroy(unitgfxs[unit.info.type].gameObject);
            unitgfxs.Remove(unit.info.type);
        }
    }

    public void RefreshUnit(UnitType type)
    {
        unitgfxs[type].RefreshGFX();
    }


}
