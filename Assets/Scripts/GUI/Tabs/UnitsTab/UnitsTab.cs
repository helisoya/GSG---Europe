using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Class representing the Units Tab
/// </summary>
public class UnitsTab : GUITab
{
    [SerializeField] private Transform unitCardsRoot;
    [SerializeField] private GameObject unitCardPrefab;
    [SerializeField] private CountryInfoTab countryInfoTab;
    [SerializeField] private ProvinceTab provinceTab;
    private UnitsTabUnitCard currentCard;

    /// <summary>
    /// Opens the units tab
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        provinceTab.CloseTab();
        countryInfoTab.CloseTab();
        DestroyCards();
        currentCard = null;
        GenerateCards(Manager.instance.player);
    }

    /// <summary>
    /// Closes the units tab
    /// </summary>
    public override void CloseTab()
    {
        base.CloseTab();
        DestroyCards();
        if (MapModes.currentMapMode == MapModes.MAPMODE.UNITPLACEMENT) MapModes.ChangeMapMode(MapModes.MAPMODE.POLITICAL);
    }

    /// <summary>
    /// Generates the unit cards for a country
    /// </summary>
    /// <param name="country">The country</param>
    void GenerateCards(Country country)
    {
        foreach (UnitType typeUnlocked in country.unitTypesUnlocked)
        {
            Instantiate(unitCardPrefab, unitCardsRoot).GetComponent<UnitsTabUnitCard>().Init(
                Resources.Load<UnitTypeInfo>("Units/Types/" + typeUnlocked.ToString()),
                this
            );
        }
    }

    /// <summary>
    /// Selects a new card
    /// </summary>
    /// <param name="card"></param>
    public void Select(UnitsTabUnitCard card)
    {
        if (Manager.instance.player.AP < card.price) return;
        if (currentCard != null)
        {
            currentCard.SetHighlighted(false);
        }

        currentCard = card;
        currentCard.SetHighlighted(true);
        MapModes.ChangeMapMode(MapModes.MAPMODE.UNITPLACEMENT);
    }

    /// <summary>
    /// Spawns a unit at the selected province
    /// </summary>
    /// <param name="province">The province</param>
    public void Select(Province province)
    {
        if (currentCard != null && Manager.instance.player.AP >= currentCard.price)
        {
            Manager.instance.player.AP -= (int)currentCard.price;
            GameGUI.instance.RefreshUtilityBar();
            Manager.instance.player.CreateUnit(province, currentCard.type);
            currentCard.SetHighlighted(false);
            currentCard = null;
        }

        MapModes.ChangeMapMode(MapModes.MAPMODE.POLITICAL);
    }

    /// <summary>
    /// Destroys the current unit cards
    /// </summary>
    void DestroyCards()
    {
        foreach (Transform child in unitCardsRoot)
        {
            Destroy(child.gameObject);
        }
    }
}


