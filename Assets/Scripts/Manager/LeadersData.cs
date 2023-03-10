using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static string GetRandomBodyPart()
    {
        return instance.bodyImg[Random.Range(0, instance.bodyImg.Length)];
    }

    public static string GetRandomHairPart()
    {
        return instance.hairImg[Random.Range(0, instance.hairImg.Length)];
    }

    public static string GetRandomHeadPart()
    {
        return instance.headImg[Random.Range(0, instance.headImg.Length)];
    }

    public static Color GetRandomBodyColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }

    public static Color GetRandomHairColor()
    {
        return instance.hairColor[Random.Range(0, instance.hairColor.Length)];
    }

    public static Color GetRandomHeadColor()
    {
        return instance.headColor[Random.Range(0, instance.headColor.Length)];
    }
}
