using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    bool canPause = true;

    public static bool GamePaused = false;
    public static bool IgnoreEsc = false;
    public GameObject PauseMenuUI;

    public static EmptyDelegate OnPause;
    public static EmptyDelegate OnResume;

    private void OnEnable() {
        LevelEndZone.OnLevelEnd += DisablePause;
    }
    private void OnDisable() {
        LevelEndZone.OnLevelEnd -= DisablePause;
    }
    void Start ()
    {
        CustomInput.close.started += ctx => {
            if (canPause && !IgnoreEsc) {
                if (GamePaused == true)
                {
                    ResumeGame();
                }
                else if (GamePaused == false)
                {
                    PauseGame();
                }
            }
        };
    }
    void Update()
    {
        // if (!canPause)
        //     return;

        // if (CustomInput.GetButton("Close") && !IgnoreEsc)
        // {
        //     if (GamePaused == true)
        //     {
        //         ResumeGame();
        //     }
        //     else if (GamePaused == false)
        //     {
        //         PauseGame();
        //     }
        // }
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


    private void DisablePause() {
        if (GamePaused)
            ResumeGame();
        canPause = false;
    }
}
