using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader
{
    public string nom;
    public string prenom;
    public int age;

    private int death;

    public Leader()
    {
        age = Random.Range(40, 68);
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

    public void ResetDeath()
    {
        death = Random.Range(80, 97);
    }

}
