using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public int speed;

    public Color32 color_day;

    public Color32 color_night;

    bool day = true;

    float remainingTime = 30;

    Coroutine coroutine;

    
    void Update()
    {
        if(Time.timeScale!=0 && coroutine==null){
            if(remainingTime>0){
                remainingTime-=Time.deltaTime*speed;
            }else{
                if(day){
                    coroutine = StartCoroutine(SwapToColor(color_night));
                }else{
                    coroutine = StartCoroutine(SwapToColor(color_day));
                }
            }
            

        }

    }

    IEnumerator SwapToColor(Color32 color){
        while(GetComponent<Light>().color!=color){
            GetComponent<Light>().color = Color32.Lerp(GetComponent<Light>().color,color,Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        day = !day;
        remainingTime = 30;
        StopCoroutine(coroutine);
    }


    public void StopCycle(){
        if(coroutine!=null){
            StopCoroutine(coroutine);
        }
        remainingTime = 30;
        day = true;
        GetComponent<Light>().color = color_day;
    }
}
