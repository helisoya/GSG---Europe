using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles a Unit type GFX, inside a province
/// </summary>
public class ProvinceUnitGfx : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image hpFill;
    [SerializeField] private Image unitTypeIcon;
    [SerializeField] private TextMeshProUGUI unitCountText;
    private List<Unit> units;
    private int _count = 0;

    public int Count
    {
        get
        {
            return _count;
        }
    }


    /// <summary>
    /// Initialize the ProvinceUnitGFX
    /// </summary>
    /// <param name="type">Unit type</param>
    /// <param name="pays">Parent country</param>
    public void Init(UnitType type, Country pays)
    {
        units = new List<Unit>();
        unitCountText.text = "0";
        hpFill.fillAmount = 1;
        _count = 0;

        unitTypeIcon.sprite = pays.currentFlag;
        hpFill.color = (pays == Manager.instance.player) ? Color.green : Color.red;

    }

    /// <summary>
    /// Add a unit to the GFX
    /// </summary>
    /// <param name="unit">The unit</param>
    public void AddUnit(Unit unit)
    {
        _count++;
        units.Add(unit);
        RefreshGFX();
    }

    /// <summary>
    /// Remove a unit from the GFX
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RemoveUnit(Unit unit)
    {
        _count--;
        units.Remove(unit);
        RefreshGFX();
    }


    /// <summary>
    /// Refresh the GFX informations
    /// </summary>
    public void RefreshGFX()
    {
        unitCountText.text = units.Count.ToString();

        float current = 0;
        float total = 0;
        foreach (Unit unit in units)
        {
            current += unit.currentHp;
            total += unit.maxHp;
        }


        hpFill.fillAmount = current / total;
    }

    /// <summary>
    /// Change the flag
    /// </summary>
    /// <param name="sprite">New flag</param>
    public void RefreshFlag(Sprite sprite)
    {
        unitTypeIcon.sprite = sprite;
    }


    /// <summary>
    /// Selects Units when clicked on
    /// </summary>
    public void Click()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            foreach (Unit unit in Manager.instance.selected_unit)
            {
                unit?.UnSelect();
            }

            Manager.instance.selected_unit = new List<Unit>();
        }

        foreach (Unit unit in units)
        {
            unit.Click_Event();
        }
    }
}
