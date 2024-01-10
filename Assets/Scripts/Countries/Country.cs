using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A country
/// </summary>
public class Country
{

    public string ID = "000";
    public Color32 color;
    public string nom;
    public Culture culture;
    public List<Province> provinces;
    public int Government_Form = 0;
    public int currentParty = 2;
    public Leader leader;
    public int current_flag; // Drapeaus + Selectioné
    private static string[] flag_names = {
        "republic",
        "monarchy",
        "communism",
        "fascism"
    };

    public string cosmeticID;
    public Sprite currentFlag
    {
        get
        {
            Sprite result = Resources.Load<Sprite>("Flags/" + cosmeticID + "_" + flag_names[current_flag]);
            if (result != null) return result;
            return Resources.Load<Sprite>("Flags/" + cosmeticID);
        }
    }

    public Party[] parties;

    public int AP = 100;
    public int AP_PerMonth { get { return 10 + bonusAP + (federation != null ? 10 * federation.APBonus : 0); } }

    public int DP = 10;
    public int DP_PerMonth { get { return 2 + bonusDP + (federation != null ? federation.DPBonus : 0); } }

    private int bonusMilCap = 0;
    private int bonusAP = 0;
    private int bonusDP = 0;

    public int unitCap { get { return 5 + (provinces.Count / 2) + bonusMilCap + (federation != null ? 3 * federation.UCBonus : 0); } }


    public int date_elections = -1;
    private Manager manager;
    private Events events;
    public bool reelected = false;

    public List<Unit> units;

    public float bonusDefense;
    public float bonusNaval;
    public float bonusDamage;
    public float bonusHP;
    public float bonusSpeed;
    public bool hasTech_Naval = false;


    public Country lord;
    public Federation federation;
    public Dictionary<string, Relation> relations;
    public List<Province> cores;
    public List<string> atWarWith;


    public List<string> focusDone;
    public string currentFocus;
    public int maxFocusTime;
    public int currentFocusTime;
    public bool DestroyIfNotSelected = false;


    public List<string> AI_MARKFORWAR;
    public List<Country> AI_NEIGHBOORS;


    public bool canBuyUnit
    {
        get { return units.Count < unitCap; }
    }

    public Dictionary<string, Focus> focusTree
    {
        get { return culture.focusTree; }
    }

    private List<Country> canTraverse;


    /// <summary>
    /// Initialize a country
    /// </summary>
    public Country()
    {
        maxFocusTime = 10;
        currentFocus = "NONE";
        focusDone = new List<string>();
        atWarWith = new List<string>();
        units = new List<Unit>();
        leader = new Leader();
        relations = new Dictionary<string, Relation>();
        manager = Manager.instance;
        events = manager.GetComponent<Events>();

        AI_MARKFORWAR = new List<string>();
        AI_NEIGHBOORS = new List<Country>();

        canTraverse = new List<Country>();

        ResetElections();
        ResetFlag();


        bonusNaval = 1f;
        bonusDamage = 1f;
        bonusHP = 1f;
        bonusSpeed = 1f;
        bonusDefense = 1f;
    }

    /// <summary>
    /// Starts pathfinding between 2 provinces
    /// </summary>
    /// <param name="from">Start province</param>
    /// <param name="to">Target province</param>
    /// <returns>The path between the 2 provinces</returns>
    public GraphPath StartPathfinding(Province from, Province to)
    {
        return manager.movementGraph.GetShortestPath(from, to, canTraverse, hasTech_Naval);
    }

    /// <summary>
    /// Adds a country to traversal options
    /// </summary>
    /// <param name="pays">The country to add</param>
    public void RemoveFromTraversalOptions(Country pays)
    {
        canTraverse.Remove(pays);

    }


    /// <summary>
    /// Removes a country from traversal options
    /// </summary>
    /// <param name="pays">The country to remove</param>
    public void AddToTraversalOptions(Country pays)
    {
        if (!canTraverse.Contains(pays))
        {
            canTraverse.Add(pays);
        }
    }


    /// <summary>
    /// Increment the current focus
    /// </summary>
    public void IncrementFocus()
    {
        if (currentFocus.Equals("NONE")) return;

        currentFocusTime--;
        if (currentFocusTime <= 0)
        {
            focusDone.Add(currentFocus);

            bool refreshUnits = false;


            // Effects interpreter

            foreach (string effectNotSplited in focusTree[currentFocus].effect)
            {
                string[] effect = effectNotSplited.Split("(");
                effect[1] = effect[1].Split(")")[0];
                switch (effect[0])
                {
                    case "AP":
                        AP += int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "APBONUS":
                        bonusAP += int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "DP":
                        DP += int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "DPBONUS":
                        bonusDP += int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "SETGOVERNEMENT":
                        Government_Form = int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        ResetElections();
                        ResetFlag();
                        break;
                    case "MILCAP":
                        bonusMilCap += int.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "SPEED":
                        refreshUnits = true;
                        bonusSpeed -= float.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "HP":
                        refreshUnits = true;
                        bonusHP += float.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "DEFENSE":
                        refreshUnits = true;
                        bonusDefense += float.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "ATTACK":
                        refreshUnits = true;
                        bonusDamage += float.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "NAVALBOOST":
                        refreshUnits = true;
                        bonusNaval += float.Parse(effect[1], System.Globalization.NumberStyles.Any);
                        break;
                    case "ENABLE_NAVAL":
                        hasTech_Naval = true;
                        break;
                    case "PARTYPOP":
                        string[] split = effect[1].Split(",");
                        AddPopularity(int.Parse(split[0], System.Globalization.NumberStyles.Any), int.Parse(split[1], System.Globalization.NumberStyles.Any));
                        break;
                    case "FREE":
                        Country p = manager.GetCountry(effect[1]);
                        foreach (Province prov in p.cores)
                        {
                            if (provinces.Contains(prov))
                            {
                                RemoveProvince(prov);
                                p.AddProvince(prov);
                            }
                        }
                        break;
                    case "ANNEXPROVINCE":
                        string[] splited = effect[1].Split(",");
                        Country p1 = manager.GetCountry(splited[0]);
                        Province prov1 = manager.GetProvince(int.Parse(splited[1]));
                        prov1.owner.RemoveProvince(prov1);
                        p1.AddProvince(prov1);
                        break;
                    case "COSMETIC":
                        cosmeticID = effect[1];
                        break;
                    case "PUPPET":
                        Country p2 = manager.GetCountry(effect[1]);
                        CanvasWorker.instance.UpdateRelations_ShortCut(p2, this, 3);
                        break;
                    case "RANDOMFEDERATION":
                        List<Country> candidates = new List<Country>();
                        foreach (Country p3 in AI_NEIGHBOORS)
                        {
                            if (p3.federation == null || p3.federation != federation)
                            {
                                candidates.Add(p3);
                            }
                        }
                        if (candidates.Count != 0)
                        {
                            Country chosenCandidate = candidates[Random.Range(0, candidates.Count)];
                            if (chosenCandidate.federation != null)
                            {
                                chosenCandidate.federation.RemoveMember(chosenCandidate);
                            }

                            if (federation == null)
                            {
                                Federation federation = new Federation();
                                federation.AddMember(this);
                                federation.AddMember(chosenCandidate);
                                federation.SetLeader(this);
                                manager.federations.Add(federation);
                            }
                            else
                            {
                                federation.AddMember(chosenCandidate);
                            }
                        }
                        break;

                    case "FEDERATION":
                        Country federateWith = manager.GetCountry(effect[1]);
                        if (federateWith.federation != null)
                        {
                            federateWith.federation.RemoveMember(federateWith);
                        }
                        if (federation == null)
                        {
                            Federation federation = new Federation();
                            federation.AddMember(this);
                            federation.AddMember(federateWith);
                            federation.SetLeader(this);
                            manager.federations.Add(federation);
                        }
                        else
                        {
                            federation.AddMember(federateWith);
                        }
                        break;

                    case "ANNEX":
                        Country toAnnex = manager.GetCountry(effect[1]);
                        List<Province> provs = new List<Province>(toAnnex.provinces);
                        foreach (Province prov in provs)
                        {
                            toAnnex.RemoveProvince(prov);
                            AddProvince(prov);
                        }
                        break;
                }
            }
            currentFocus = "NONE";

            // Refresh Units if needed
            if (refreshUnits)
            {
                foreach (Unit unit in units)
                {
                    unit.RefreshGFX();
                }
            }

            // Refresh Canvas if needed
            if (this == manager.player)
            {
                CanvasWorker.instance.UpdateFocus();
            }
        }
    }


    /// <summary>
    /// Check if the prerequists for a Focus are one
    /// </summary>
    /// <param name="focus">The focus</param>
    /// <returns>The prerequists are done ?</returns>
    public bool PrerequistDone(Focus focus)
    {
        if (focus.required.Count == 0) return true;

        foreach (string exlcusive in focus.exclusive)
        {
            if (focusDone.Contains(exlcusive)) return false;
        }

        foreach (string prerequist in focus.required)
        {
            if (!focusDone.Contains(prerequist) && focus.requireAll) return false;
            if (focusDone.Contains(prerequist) && !focus.requireAll) return true;
        }

        return focus.requireAll;
    }

    /// <summary>
    /// Finds the available focuses
    /// </summary>
    /// <returns>A list of available focuses</returns>
    public List<Focus> GetAvailableFocus()
    {
        List<Focus> list = new List<Focus>();
        foreach (Focus focus in focusTree.Values)
        {
            if (focusDone.Contains(focus.id)) continue;
            if (!PrerequistDone(focus)) continue;
            list.Add(focus);
        }
        return list;
    }

    /// <summary>
    /// Cuts down the army to the size of the unit cap
    /// </summary>
    public void CutDownArmy()
    {
        if (units.Count <= unitCap) return;

        List<Unit> correctList = new List<Unit>();
        for (int i = 0; i < units.Count && i < unitCap; i++)
        {
            correctList.Add(units[i]);
        }

        for (int i = unitCap; i < units.Count; i++)
        {
            Object.Destroy(units[i].gameObject);
        }

        units = correctList;
    }

    /// <summary>
    /// Change the color of the country
    /// </summary>
    /// <param name="col">The new color</param>
    public void SetColor(Color col)
    {
        color = col;
    }

    /// <summary>
    /// Change the current focus
    /// </summary>
    /// <param name="newFocus">The new focus's ID</param>
    public void ChangeFocus(string newFocus)
    {
        currentFocus = newFocus;
        currentFocusTime = maxFocusTime;
    }


    /// <summary>
    /// Declare war on a country
    /// </summary>
    /// <param name="country">The target country</param>
    public void DeclareWarOnCountry(Country country)
    {
        if (relations[country.ID].atWar)
        {
            return;
        }
        if (lord == country)
        {
            lord = null;
        }
        else if (country.lord == this)
        {
            country.lord = null;
        }

        relations[country.ID].atWar = true;
        relations[country.ID].ResetWarScores();
        relations[country.ID].wargoals.Remove(ID);

        atWarWith.Add(country.ID);
        country.atWarWith.Add(ID);

        country.AddToTraversalOptions(this);
        AddToTraversalOptions(country);
    }

    /// <summary>
    /// Makes peace with a country
    /// </summary>
    /// <param name="other">The target country</param>
    public void MakePeaceWithCountry(Country other)
    {
        if (!relations[other.ID].atWar)
        {
            return;
        }

        relations[other.ID].atWar = false;
        relations[other.ID].relationScore = 0;

        atWarWith.Remove(other.ID);
        other.atWarWith.Remove(ID);

        Province[] copy = other.provinces.ToArray();

        foreach (Province p in copy)
        {
            if (p.controller.ID == ID)
            {
                AddProvince(p);
                other.RemoveProvince(p);
            }
        }
        other.CutDownArmy();
        CutDownArmy();

        if (this == manager.player)
        {
            CanvasWorker.instance.RefreshUtilityBar();
        }

        other.RemoveFromTraversalOptions(this);
        RemoveFromTraversalOptions(other);
    }

    /// <summary>
    /// Removes all units placed inside a country
    /// </summary>
    /// <param name="placeTo">Where to place the units if they are misplaced</param>
    /// <param name="country">The target country</param>
    public void RemoveUnitsFromCountry(Province placeTo, string country)
    {
        foreach (Unit unit in units)
        {
            if (unit.IsOnCountryTerritory(country))
            {
                unit.Teleport(placeTo);
            }
        }
    }

    /// <summary>
    /// Refreshs provinces
    /// </summary>
    public void RefreshProvinces()
    {
        for (int i = 0; i < provinces.Count; i++)
        {
            provinces[i].RefreshColor();
        }
    }

    /// <summary>
    /// Check if the country is completely occupied
    /// </summary>
    /// <returns>Is the country complely occupied ?</returns>
    public bool CompletelyOccupied()
    {
        foreach (Province province in provinces)
        {
            if (province.controller == this) return false;
        }
        return true;
    }

    /// <summary>
    /// Compute the effects of the end of the year for the country
    /// </summary>
    public void NewYear()
    {
        if (leader.Age(1))
        { // Leader Dead (Trigger Elections / Selection Successeur)
            if (3 == Government_Form || Government_Form == 5)
            { // Monarchy
                SameFamilyLeader();
                if (manager.player == this)
                {
                    events.DeathLeader_Monarchy();
                }
            }
            else
            { // Otherwise
                RandomizeLeader();
                if (manager.player == this)
                {
                    events.DeathLeader_Normal();
                }
            }
        }

        if (date_elections == manager.an)
        { // Trigger Elections
            TriggerElections();
        }
    }

    /// <summary>
    /// Add popularity to a party
    /// </summary>
    /// <param name="index">Party index</param>
    /// <param name="nb">popularity to add</param>
    public void AddPopularity(int index, float nb)
    {
        parties[index].popularity = Mathf.Clamp(parties[index].popularity + nb, 0, 100);

        int lg = 0;
        for (int i = 0; i < parties.Length; i++)
        {
            if (parties[i].popularity != 0 && i != index)
            {
                lg++;
            }
        }

        float other = nb / lg;
        for (int i = 0; i < parties.Length; i++)
        {
            if (i != index)
            {
                parties[i].popularity = Mathf.Clamp(parties[i].popularity - other, 0, 100);
            }
        }

        // Case governement change

        if (index == (int)PartyType.SOCIALIST && parties[index].popularity > parties[4].popularity + parties[3].popularity
        && (Government_Form == 3 || Government_Form == 4 || Government_Form >= 9))
        {
            RandomizeLeader();
            ChoiceType(1); // Left-wing establish a republic
        }
        else if (index == (int)PartyType.COMMUNIST && parties[index].popularity > parties[4].popularity + parties[3].popularity
        && (Government_Form == 3 || Government_Form == 4 || Government_Form >= 9))
        {
            RandomizeLeader();
            ChoiceType(3); // Far-Left establish a soviet republic
        }
        else if (index == (int)PartyType.CONSERVATIVE && parties[index].popularity > parties[0].popularity + parties[1].popularity
        && (Government_Form == 6))
        {
            RandomizeLeader();
            ChoiceType(1); // Right-wing establish a republic
        }
        else if (index == (int)PartyType.FASCIST && parties[index].popularity > parties[0].popularity + parties[1].popularity
        && (Government_Form == 6))
        {
            RandomizeLeader();
            ChoiceType(4); // Far-right establish fascism
        }
    }

    /// <summary>
    /// Randomize the leader
    /// </summary>
    public void RandomizeLeader()
    {
        leader.nom = culture.GetRandom_Nom();
        leader.prenom = culture.GetRandom_Prenom();
        leader.age = Random.Range(30, 70);
        leader.RandomizeLeaderGFX();
        leader.ResetDeath();
    }


    /// <summary>
    /// Randomize the leader but keeps his family name
    /// </summary>
    public void SameFamilyLeader()
    {
        leader.prenom = culture.GetRandom_Prenom();
        leader.age = Random.Range(30, 70);
        leader.RandomizeLeaderGFX();
        leader.ResetDeath();
    }

    /// <summary>
    /// Trigger new elections
    /// </summary>
    public void TriggerElections()
    {
        int index = 0;
        for (int i = 1; i < parties.Length; i++)
        {
            if (parties[i].popularity > parties[index].popularity)
            {
                index = i;
            }
        }
        AddPopularity(index, 5);
        currentParty = index;

        if (manager.player == this)
        {
            events.Elections(ID, index);
        }
        else
        {
            if (index == (int)PartyType.COMMUNIST && Ideologie() != 2)
            { // Case -> Communisme
                Government_Form = GetRandomGov(3);
                ResetFlag();
                RandomizeLeader();
            }
            else if (index == (int)PartyType.FASCIST && Ideologie() != 3)
            { // Case -> Fascism
                Government_Form = GetRandomGov(4);
                ResetFlag();
                RandomizeLeader();
            }
            else if ((index >= (int)PartyType.CENTRIST && Ideologie() == 2) ||
            (index <= (int)PartyType.CENTRIST && Ideologie() == 3) ||
            (index <= (int)PartyType.SOCIALIST && Ideologie() == 1))
            { // Case -> Republic
                Government_Form = GetRandomGov(4);
                ResetFlag();
                RandomizeLeader();
            }
            else if (reelected)
            { // Case -> Monarchy
                if (Random.Range(0, 100) < Random.Range(40, 60))
                {
                    reelected = false;
                    Government_Form = GetRandomGov(2);
                    ResetFlag();
                }
                else
                {
                    RandomizeLeader();
                }
                reelected = false;
            }
            else
            { // Other
                if (Random.Range(0, 100) < Random.Range(30, 70))
                {
                    reelected = true;
                }
                else
                {
                    RandomizeLeader();
                }
            }
        }

        ResetElections();
    }


    /// <summary>
    /// Resets the elections
    /// </summary>
    public void ResetElections()
    {
        switch (Government_Form)
        {
            case 0:
                date_elections = manager.an + 4;
                break;

            case 1:
                date_elections = manager.an + 5;
                break;

            case 2:
                date_elections = manager.an + 7;
                break;

            case 5:
                date_elections = manager.an + 8;
                break;
            case 7:
                date_elections = manager.an + 10;
                break;
            case 8:
                date_elections = manager.an + 20;
                break;
            default:
                date_elections = -1;
                break;
        }
    }

    /// <summary>
    /// Resets the current flag index
    /// </summary>
    public void ResetFlag()
    {
        if (Government_Form <= 2)
        {
            current_flag = 0;
        }
        else if (Government_Form <= 5)
        {
            current_flag = 1;
        }
        else if (Government_Form <= 8)
        {
            current_flag = 2;
        }
        else
        {
            current_flag = 3;
        }

        foreach (Unit unit in units)
        {
            unit.UpdateFlag();
        }
    }

    /// <summary>
    /// Return a random governement subtype
    /// </summary>
    /// <param name="type">Governement type</param>
    /// <returns>A random subtype</returns>
    int GetRandomGov(int type)
    {
        switch (type)
        {
            case 1:
                return Random.Range(0, 3);
            case 2:
                return Random.Range(3, 6);
            case 3:
                return Random.Range(6, 9);
            default:
                return Random.Range(9, 12);
        }
    }


    /// <summary>
    /// Shows subtype choice according to the new governement type
    /// </summary>
    /// <param name="groupe"></param>
    public void ChoiceType(int groupe)
    {

        if (this == manager.player)
        {
            if (groupe == 1)
            { // Republic
                events.ChoixType_Rep();

            }
            else if (groupe == 2)
            { // Monarchy
                events.ChoixType_Mon();

            }
            else if (groupe == 3)
            { // Communism
                events.ChoixType_Com();

            }
            else
            { // Fascism
                events.ChoixType_Fas();
            }
        }
        else
        {
            Government_Form = GetRandomGov(groupe);
            ResetElections();
            ResetFlag();
        }
    }

    /// <summary>
    /// Removes a unit from the country's units
    /// </summary>
    /// <param name="unit">The unit</param>
    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }

    /// <summary>
    /// Creates a unit inside a province
    /// </summary>
    /// <param name="prov">The starting province</param>
    public void CreateUnit(Province prov)
    {
        Unit obj = Object.Instantiate(culture.prefabTank, GameObject.Find("Units").transform).GetComponent<Unit>();
        obj.Init(prov, this);
        units.Add(obj);
        CanvasWorker.instance.RefreshUtilityBar();
    }



    /// <summary>
    /// Remove a province from owned provinces
    /// </summary>
    /// <param name="prov">The province</param>
    public void RemoveProvince(Province prov)
    {
        provinces.Remove(prov);
        if (provinces.Count == 0)
        {
            if (federation != null)
            {
                federation.RemoveMember(this);
            }
            foreach (Unit unit in units)
            {
                Object.Destroy(unit.gameObject);
            }
        }
        CheckNeighboors();
    }

    /// <summary>
    /// Add a province to owned provinces
    /// </summary>
    /// <param name="prov">The province</param>
    /// <param name="refresh">Should the province color be refreshed ?</param>
    public void AddProvince(Province prov, bool refresh = true)
    {
        provinces.Add(prov);
        prov.SetOwner(this);
        prov.SetController(this);
        if (refresh)
        {
            prov.RefreshColor();
        }
        CheckNeighboors();
    }

    /// <summary>
    /// Check the Country's neighbors (Only for AI)
    /// </summary>
    public void CheckNeighboors()
    {
        AI_NEIGHBOORS.Clear();
        foreach (Province province in provinces)
        {
            foreach (Province neighboor in province.adjacencies)
            {
                if (neighboor.type != ProvinceType.SEA && neighboor.owner != this && !AI_NEIGHBOORS.Contains(neighboor.owner))
                {
                    AI_NEIGHBOORS.Add(neighboor.owner);
                }
            }
        }
    }

    /// <summary>
    /// Copy a country's governement form
    /// </summary>
    /// <param name="c">The target country</param>
    public void CopyCat(Country c)
    {
        Government_Form = c.Government_Form;
        ResetElections();
        ResetFlag();
        RandomizeLeader();
    }

    /// <summary>
    /// Copy a country's color
    /// </summary>
    /// <param name="c">The target country</param>
    public void MimicColor(Country c)
    {
        color = Color32.Lerp(color, c.color, 0.6f);
        RefreshProvinces();
    }

    /// <summary>
    /// Return the country's Governement type
    /// </summary>
    /// <returns>The country's governement type</returns>
    public int Ideologie()
    {
        if (Government_Form <= 2)
        {
            return 0;
        }
        else if (Government_Form <= 5)
        {
            return 1;
        }
        else if (Government_Form <= 8)
        {
            return 2;
        }
        return 3;
    }

}