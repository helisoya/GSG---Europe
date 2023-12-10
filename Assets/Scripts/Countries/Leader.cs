using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A leader of a country, purely cosmetic
/// </summary>
public class Leader
{
    public string nom;
    public string prenom;
    public int age;

    private int death;
    public string bodyGFX;
    public string hairGFX;
    public string headGFX;
    public Color bodyColor;
    public Color hairColor;
    public Color headColor;



    /// <summary>
    /// Generates a new Random Leader
    /// </summary>
    public Leader()
    {
        age = Random.Range(40, 68);
        RandomizeLeaderGFX();
        ResetDeath();
    }


    /// <summary>
    /// Age the leader
    /// </summary>
    /// <param name="years">The number of years to add</param>
    /// <returns>Is the leader dead ?</returns>
    public bool Age(int years)
    { // Fait veillir le dirigeant, si il est plus vieux que death -> renvoir True/KAPUT
        age += years;
        if (age >= death)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// Get random body parts and colors
    /// </summary>
    public void RandomizeLeaderGFX()
    {
        bodyGFX = LeadersData.GetRandomBodyPart();
        hairGFX = LeadersData.GetRandomHairPart();
        headGFX = LeadersData.GetRandomHeadPart();

        bodyColor = LeadersData.GetRandomBodyColor();
        headColor = LeadersData.GetRandomHeadColor();
        hairColor = LeadersData.GetRandomHairColor();
    }


    /// <summary>
    /// Resets the leader's death
    /// </summary>
    public void ResetDeath()
    {
        death = Random.Range(80, 97);
    }

}
