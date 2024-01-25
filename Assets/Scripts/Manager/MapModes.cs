using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles the different map modes
/// </summary>
public class MapModes : MonoBehaviour
{
    [SerializeField] private Color32[] _colors_ideologie;
    [SerializeField] private Color32[] _colors_relations;
    [SerializeField] private Color32[] _colors_formable;
    [SerializeField] private Color32 _colors_grayed;


    public enum MAPMODE
    {
        POLITICAL,
        IDEOLOGICAL,
        DIPLOMATIC,
        FEDERATION,
        FORMABLE,
        UNITPLACEMENT
    }
    private MAPMODE _currentMapMode = MAPMODE.POLITICAL;


    private static MapModes instance;

    void Awake()
    {
        instance = this;
    }

    public static Color32[] colors_ideologie
    {
        get { return instance._colors_ideologie; }
    }

    public static Color32[] colors_relations
    {
        get { return instance._colors_relations; }
    }

    public static Color32[] colors_formable
    {
        get { return instance._colors_formable; }
    }

    public static Color32 colors_grayed
    {
        get { return instance._colors_grayed; }
    }

    public static MAPMODE currentMapMode
    {
        get { return instance._currentMapMode; }
        set { instance._currentMapMode = value; }
    }



    /// <summary>
    /// Updates the mapmodes if needed
    /// </summary>
    void Update()
    {
        if (!Manager.instance.picked || !GameGUI.instance.canSwitchMapMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentMapMode = MAPMODE.POLITICAL;
            Manager.instance.RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentMapMode = MAPMODE.IDEOLOGICAL;
            Manager.instance.RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _currentMapMode = MAPMODE.DIPLOMATIC;
            Manager.instance.RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _currentMapMode = MAPMODE.FEDERATION;
            Manager.instance.RefreshMap();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && Manager.instance.currentFormable != null)
        {
            _currentMapMode = MAPMODE.FORMABLE;
            Manager.instance.RefreshMap();
        }
    }

    /// <summary>
    /// Changes the current map mode
    /// </summary>
    /// <param name="newMode">The new map mode</param>
    public static void ChangeMapMode(MAPMODE newMode)
    {
        instance._currentMapMode = newMode;
        Manager.instance.RefreshMap();
    }
}
