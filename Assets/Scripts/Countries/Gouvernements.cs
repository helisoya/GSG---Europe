using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gouvernements : MonoBehaviour
{
    // Gouvernements 
    // 0-2 = République -> Elections tout les 4-5-7 ans selon le type choisi. Leader entre 42-62 ans
    // -> Passage Monarchie = Réelir 2 fois le meme type et choisir le coup d'état
    //                        OU prendre droite/extreme droite et réptablir la monarchie
    // -> Passage Dem. Pop. /Rep. Soc. = Elir un parti d'extrème et choisir le coup d'état

    // 3-5 = Monarchie -> Monarche à vie. Selon le type choisi, successeur élu ou pas. Leader entre 15-73 ans
    // -> Passage TOUT = Se faire renverser par les jacobins et choisir le régime voulu

    // 6-8 = Démocratie Populaire -> Leader a vie ou non selon le type choisi. Successeur élu a la mort ou après 10-20 ans, entre 40-75 ans.
    // -> Passage TOUT = Se faire renverser par un coup d'état des modérés/opposition

    // 9-11 = République Sociale -> Leader a vie. Succésseur élu a la mort, entre 30-60 ans.
    // -> Passage TOUT = Se faire renverser par un coup d'état des modérés/opposition


    // 00 = Démocratie Directe
    // 01 = Démocratie Mixte
    // 02 = Démocratie Parlementaire

    // 03 = Monarchie Absolu
    // 04 = Monarchie Elective
    // 05 = Monarchie Parlementaire

    // 06 = Republique Soviétique
    // 07 = Démocratie Populaire 
    // 08 = Union Populaire

    // 09 = Nouveau Reich
    // 10 = République Sociale
    // 11 = Junte Militaire


    public string[] noms;

    public string[] descs;

    
}
