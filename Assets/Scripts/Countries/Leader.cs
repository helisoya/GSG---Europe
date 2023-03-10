using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Leader()
    {
        age = Random.Range(40, 68);
        RandomizeLeaderGFX();
        ResetDeath();
    }

    public bool Age(int ADD)
    { // Fait veillir le dirigeant, si il est plus vieux que death -> renvoir True/KAPUT
        age += ADD;
        if (age >= death)
        {
            return true;
        }
        return false;
    }

    public void RandomizeLeaderGFX()
    {
        bodyGFX = LeadersData.GetRandomBodyPart();
        hairGFX = LeadersData.GetRandomHairPart();
        headGFX = LeadersData.GetRandomHeadPart();

        bodyColor = LeadersData.GetRandomBodyColor();
        headColor = LeadersData.GetRandomHeadColor();
        hairColor = LeadersData.GetRandomHairColor();
    }

    public void ResetDeath()
    {
        death = Random.Range(80, 97);
    }

}
