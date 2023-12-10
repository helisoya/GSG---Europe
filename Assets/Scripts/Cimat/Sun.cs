using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the day / night cycles
/// </summary>
public class Sun : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private int speed;
    [SerializeField] private Color32 color_day;
    [SerializeField] private Color32 color_night;

    private bool day = true;

    private float remainingTime = 30;

    private Coroutine coroutine;


    void Update()
    {
        // Defile time and switch between day / night every few seconds

        if (Time.timeScale != 0 && coroutine == null)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime * speed;
            }
            else
            {

                // Switch between day / night
                if (day)
                {
                    coroutine = StartCoroutine(SwapToColor(color_night));
                }
                else
                {
                    coroutine = StartCoroutine(SwapToColor(color_day));
                }
            }


        }

    }

    /// <summary>
    /// Swap the sun color
    /// </summary>
    /// <param name="color">Target color</param>
    /// <returns>IEnumerator</returns>
    IEnumerator SwapToColor(Color32 color)
    {
        while (GetComponent<Light>().color != color)
        {
            GetComponent<Light>().color = Color32.Lerp(GetComponent<Light>().color, color, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        day = !day;
        remainingTime = 30;
        StopCoroutine(coroutine);
    }

    /// <summary>
    /// Stops the day/night cycle and force it back to daylight
    /// </summary>
    public void StopCycle()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        remainingTime = 30;
        day = true;
        GetComponent<Light>().color = color_day;
    }
}
