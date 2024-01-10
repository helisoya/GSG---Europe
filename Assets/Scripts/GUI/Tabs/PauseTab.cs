using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// The pause menu tab
/// </summary>
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

    /// <summary>
    /// Quit to desktop
    /// </summary>
    public void QuitToDesktop()
    {
        Application.Quit();
    }

    /// <summary>
    /// Quit to Title Screen
    /// </summary>
    public void QuitToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
    }

    /// <summary>
    /// Open settings
    /// </summary>
    public void OpenSettings()
    {
        normalMenuRoot.SetActive(false);
        settingsMenuRoot.SetActive(true);
    }

    /// <summary>
    /// Close settings
    /// </summary>
    public void CloseSettings()
    {
        normalMenuRoot.SetActive(true);
        settingsMenuRoot.SetActive(false);
    }


    /// <summary>
    /// Open the pause menu
    /// </summary>
    public override void OpenTab()
    {
        base.OpenTab();
        Tooltip.instance.HideInfo();
        Timer.instance.StopTime();

        resolutions = Screen.resolutions;

        int currentRes = 0;
        List<string> names = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            names.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (currentRes == 0 && Screen.currentResolution.width == resolutions[i].width &&
            Screen.currentResolution.height == resolutions[i].height &&
            Screen.currentResolution.refreshRate == resolutions[i].refreshRate)
            {
                currentRes = i;
            }
        }

        res_d.ClearOptions();
        res_d.AddOptions(names);
        res_d.SetValueWithoutNotify(currentRes);

        if (PlayerPrefs.HasKey("clouds"))
        {
            cloud_t.SetIsOnWithoutNotify(PlayerPrefs.GetString("clouds") == "true");
        }
        if (PlayerPrefs.HasKey("sun"))
        {
            sun_t.SetIsOnWithoutNotify(PlayerPrefs.GetString("sun") == "true");
        }

        fullscreen_t.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public override void CloseTab()
    {
        base.CloseTab();
        GameGUI.instance.ShowDefault();
        Timer.instance.ResumeTime();
    }




    public void UpdateClouds()
    {
        bool val = cloud_t.isOn;
        clouds.SetActive(val);
        PlayerPrefs.SetString("clouds", val ? "true" : "false");
    }

    public void UpdateSun()
    {
        if (!sun_t.isOn)
        {
            sun.StopCycle();
            sun.enabled = false;
            PlayerPrefs.SetString("sun", "false");
        }
        else
        {
            sun.enabled = true;
            PlayerPrefs.SetString("sun", "true");
        }

    }

    public void UpdateFullscreen()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, fullscreen_t.isOn);
    }

    public void UpdateResolution()
    {
        Screen.SetResolution(resolutions[res_d.value].width, resolutions[res_d.value].height, fullscreen_t.isOn);
    }
}
