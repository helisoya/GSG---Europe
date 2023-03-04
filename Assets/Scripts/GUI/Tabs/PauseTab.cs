using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PauseTab : GUITab
{
    [SerializeField] protected GameObject normalMenuRoot;
    [SerializeField] protected GameObject settingsMenuRoot;


    [Header("Settings")]
    [SerializeField] private GameObject clouds;
    [SerializeField] private Sun sun;
    [SerializeField] private Toggle cloud_t;
    [SerializeField] private Toggle sun_t;
    [SerializeField] private TMP_Dropdown res_d;
    [SerializeField] private Toggle fullscreen_t;
    private Resolution[] resolutions;

    public void QuitToDesktop()
    {
        Application.Quit();
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    public void OpenSettings()
    {
        normalMenuRoot.SetActive(false);
        settingsMenuRoot.SetActive(true);
    }

    public void CloseSettings()
    {
        normalMenuRoot.SetActive(true);
        settingsMenuRoot.SetActive(false);
    }

    public override void OpenTab()
    {
        base.OpenTab();
        Timer.instance.StopTime();

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
    }

    public override void CloseTab()
    {
        base.CloseTab();
        CanvasWorker.instance.ShowDefault();
        Timer.instance.ResumeTime();
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
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen_t.isOn);
    }

    public void UpdateResolution()
    {
        PlayerPrefs.SetInt("resolution", res_d.value);
        Screen.SetResolution(resolutions[res_d.value].width, resolutions[res_d.value].height, fullscreen_t.isOn);
    }
}
