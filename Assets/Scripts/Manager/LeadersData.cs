using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the differents body parts and colors
/// </summary>
public class LeadersData : MonoBehaviour
{
    private static LeadersData instance;

    [SerializeField] private string[] bodyImg;
    [SerializeField] private string[] headImg;
    [SerializeField] private string[] hairImg;
    [SerializeField] private Color[] hairColor;
    [SerializeField] private Color[] headColor;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Returns a random body part;
    /// </summary>
    /// <returns>a random body part</returns>
    public static string GetRandomBodyPart()
    {
        return instance.bodyImg[Random.Range(0, instance.bodyImg.Length)];
    }

    /// <summary>
    /// Returns a random hair part;
    /// </summary>
    /// <returns>a random hair part</returns>
    public static string GetRandomHairPart()
    {
        return instance.hairImg[Random.Range(0, instance.hairImg.Length)];
    }

    /// <summary>
    /// Returns a random head part;
    /// </summary>
    /// <returns>a random head part</returns>
    public static string GetRandomHeadPart()
    {
        return instance.headImg[Random.Range(0, instance.headImg.Length)];
    }

    /// <summary>
    /// Returns a random body color;
    /// </summary>
    /// <returns>a random body color</returns>
    public static Color GetRandomBodyColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    /// <summary>
    /// Returns a random hair color;
    /// </summary>
    /// <returns>a random hair color</returns>
    public static Color GetRandomHairColor()
    {
        return instance.hairColor[Random.Range(0, instance.hairColor.Length)];
    }

    /// <summary>
    /// Returns a random head color;
    /// </summary>
    /// <returns>a random head color</returns>
    public static Color GetRandomHeadColor()
    {
        return instance.headColor[Random.Range(0, instance.headColor.Length)];
    }
}
