using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Image loading_fill;
    public Image loading_back;

    public Button start_button;

    public Button quit;


    IEnumerator Load(){

        AsyncOperation loading = SceneManager.LoadSceneAsync("Map");
        
        while(loading.progress<1){
            loading_fill.fillAmount = loading.progress;
            yield return new WaitForEndOfFrame();
        }
    }

    public void StartGame(){
        quit.gameObject.SetActive(false);
        start_button.gameObject.SetActive(false);
        loading_back.gameObject.SetActive(true);
        loading_fill.gameObject.SetActive(true);
        StartCoroutine(Load());
    }

    public void Quit(){
        Application.Quit();
    }
}
