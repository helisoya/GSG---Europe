using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image loading_fill;
    [SerializeField] private Image loading_back;
    [SerializeField] private GameObject buttonsParent;

    /// <summary>
    /// Handles loading the map
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator Load()
    {

        AsyncOperation loading = SceneManager.LoadSceneAsync("Map");

        while (loading.progress < 1)
        {
            loading_fill.fillAmount = loading.progress;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Starts loading the main map
    /// </summary>
    public void StartGame()
    {
        buttonsParent.SetActive(false);
        loading_back.gameObject.SetActive(true);
        loading_fill.gameObject.SetActive(true);
        StartCoroutine(Load());
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
