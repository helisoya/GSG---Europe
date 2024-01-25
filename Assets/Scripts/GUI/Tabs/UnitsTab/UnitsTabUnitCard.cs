using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class representing a Unit Card on the UnitsTab
/// </summary>
public class UnitsTabUnitCard : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image unitImg;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenceText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Image rootImg;

    private Color defaultRootColor;
    private UnitTypeInfo unitTypeInfo;
    private UnitsTab tab;

    public UnitType type
    {
        get
        {
            return unitTypeInfo.type;
        }
    }

    public float price
    {
        get
        {
            return unitTypeInfo.basePrice;
        }
    }


    /// <summary>
    /// Initialize the card for a certain unit type
    /// </summary>
    /// <param name="unitType">The card's unit type</param>
    /// /// <param name="tab">The root tab</param>
    public void Init(UnitTypeInfo unitType, UnitsTab tab)
    {
        defaultRootColor = rootImg.color;
        this.tab = tab;
        unitTypeInfo = unitType;
        unitImg.sprite = unitType.unitSprite;
        attackText.text = unitType.baseAttack + " Atk";
        defenceText.text = unitType.baseDefense + " Def";
        hpText.text = unitType.baseHP + " HP";
        priceText.text = unitType.basePrice + "<sprite=12>";
    }

    /// <summary>
    /// Handles the OnClick event
    /// </summary>
    public void Click()
    {
        tab.Select(this);
    }

    /// <summary>
    /// Sets the card to be highlighted
    /// </summary>
    /// <param name="value">Is the card highlighted ?</param>
    public void SetHighlighted(bool value)
    {
        rootImg.color = value ? Color.yellow : defaultRootColor;
    }
}
