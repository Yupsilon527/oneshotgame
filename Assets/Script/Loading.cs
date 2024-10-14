using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Button startGameButton;

    public GameObject pressPlayIndicator;
    public GameObject loadingIndicator;
    // Start is called before the first frame update
    private void Awake()
    {

        if (pressPlayIndicator != null)
            pressPlayIndicator.gameObject.SetActive(true);
        if (loadingIndicator != null)
            loadingIndicator.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        startGameButton.interactable = false;
        StartCoroutine(LoadHubworldCoroutine());
    }
    IEnumerator LoadHubworldCoroutine()
    {
        if (pressPlayIndicator != null)
            pressPlayIndicator.gameObject.SetActive(false);
        if (loadingIndicator != null)
            loadingIndicator.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        yield return SceneManager.LoadSceneAsync("GameLevel", LoadSceneMode.Additive);
        yield return new WaitForEndOfFrame();

        if (pressPlayIndicator != null)
            pressPlayIndicator.gameObject.SetActive(true);
        if (loadingIndicator != null)
            loadingIndicator.gameObject.SetActive(false);

        startGameButton.interactable = true;
        SceneManager.UnloadSceneAsync("Loading");
    }
}
