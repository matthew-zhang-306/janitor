using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    [Header("For screen fade transitions")]
    public ScreenFade screenFade;
    public SongPlayer songPlayer;

    [Header("For loading screen transitions")]
    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;


    public void LoadLevel (int sceneIndex)
    {
        //AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        StartCoroutine(LoadAsynchronously(sceneIndex));
        
        if (songPlayer != null) {
            songPlayer.StopSong();
        }

        if (screenFade != null && loadingScreen != null) {
            screenFade.FadeOut();
            Helpers.Invoke(this, () => {
                screenFade.gameObject.SetActive(false);;
                loadingScreen.SetActive(true);
            }, screenFade.fadeTime);
        }
        else if (loadingScreen != null) {
            loadingScreen.SetActive(true);
        }
        else if (screenFade != null) {
            screenFade.FadeOut();
        }
        
    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        // give time for the loading screen to load
        float delayTime = screenFade != null ? screenFade.fadeTime : 0f;
        if (loadingScreen != null) {
            delayTime += 0.5f;
        }
        yield return new WaitForSeconds(screenFade != null ? screenFade.fadeTime : 1f);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone && loadingScreen != null)
        {

            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;
            progressText.text = (int)(progress * 100) + "%";

            yield return null;
        }
    }
}
