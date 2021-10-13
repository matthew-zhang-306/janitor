using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public static bool IgnoreEsc = false;
    public GameObject PauseMenuUI;

    public static EmptyDelegate OnPause;
    public static EmptyDelegate OnResume;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IgnoreEsc)
        {
            if (GamePaused == true)
            {
                ResumeGame();
            }
            else if (GamePaused == false)
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

        OnResume?.Invoke();
    }
    public void PauseGame()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;

        Debug.Log("on pause");
        OnPause?.Invoke();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
