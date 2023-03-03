using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float time = 10;

    private float time_default;

    public int maxspeed = 5;

    private float lastspeed;

    public static Timer instance;

    private bool isStoped;

    void Awake()
    {
        instance = this;
        time_default = time;
        isStoped = false;
    }


    void Update()
    {
        if (!Manager.instance.picked || isStoped)
        {
            return;
        }
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = time_default;
            Manager.instance.NextTurn();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale + 1, 0, maxspeed);
            UpdateText();
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = Mathf.Clamp(Time.timeScale - 1, 0, maxspeed);
            UpdateText();
        }
    }

    public void StopTime()
    {
        lastspeed = Time.timeScale;
        Time.timeScale = 0;
        isStoped = true;
        UpdateText();
    }

    public void ResumeTime()
    {
        Time.timeScale = lastspeed;
        isStoped = false;
        UpdateText();
    }

    void UpdateText()
    {
        CanvasWorker.instance.UpdateSpeedText((int)Time.timeScale);
    }
}
