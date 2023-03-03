using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public GameObject clouds;

    public Sun sun;

    public Toggle cloud_t;

    public Toggle sun_t;

    public TMP_Dropdown res_d;

    public Toggle fullscreen_t;

    private Resolution[] resolutions;

    bool start = true;

    void Awake()
    {

        resolutions = Screen.resolutions;

        List<string> names = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            names.Add(resolutions[i].width + " x " + resolutions[i].height);
        }

        res_d.ClearOptions();
        res_d.AddOptions(names);

        if (PlayerPrefs.HasKey("resolution"))
        {
            if (PlayerPrefs.GetInt("resolution") < resolutions.Length)
            {
                res_d.value = PlayerPrefs.GetInt("resolution");
            }
        }

        if (PlayerPrefs.HasKey("clouds"))
        {
            cloud_t.isOn = (PlayerPrefs.GetString("clouds") == "true");
        }
        if (PlayerPrefs.HasKey("sun"))
        {
            sun_t.isOn = (PlayerPrefs.GetString("sun") == "true");
        }
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            fullscreen_t.isOn = (PlayerPrefs.GetString("fullscreen") == "true");
        }
        start = false;
    }

    public void UpdateClouds()
    {
        bool val = cloud_t.isOn;
        clouds.SetActive(val);
        PlayerPrefs.SetString("clouds", val.ToString());
    }

    public void UpdateSun()
    {
        if (!sun_t.isOn)
        {
            sun.StopCycle();
            sun.enabled = false;
            PlayerPrefs.SetString("sun", false.ToString());
        }
        else
        {
            sun.enabled = true;
            PlayerPrefs.SetString("sun", true.ToString());
        }

    }

    public void UpdateFullscreen()
    {
        PlayerPrefs.SetString("fullscreen", fullscreen_t.isOn.ToString());
        if (!start)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen_t.isOn);
        }

    }

    public void UpdateResolution()
    {
        PlayerPrefs.SetInt("resolution", res_d.value);
        if (!start)
        {
            Screen.SetResolution(resolutions[res_d.value].width, resolutions[res_d.value].height, fullscreen_t.isOn);
        }

    }

    public void Quit()
    {
        Application.Quit();
    }
}
