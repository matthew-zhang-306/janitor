using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class PauseMenu : MonoBehaviour
{
    bool canPause = true;

    public static bool GamePaused = false;
    public static bool IgnoreEsc = false;
    public GameObject PauseMenuUI;

    public static EmptyDelegate OnPause;
    public static EmptyDelegate OnResume;

    public InputAction close;

    private void OnEnable() {
        LevelEndZone.OnLevelEnd += DisablePause;
    }
    private void OnDisable() {
        LevelEndZone.OnLevelEnd -= DisablePause;
    }
    void Awake()
    {
        
    }
    void Start ()
    {
        
        CustomInput.close.started += ctx => {
            Debug.Log(canPause);
            Debug.Log(IgnoreEsc);
            if (canPause && !IgnoreEsc) {
                close?.Dispose();
                close = new InputAction("Pause Close", InputActionType.Button, PlayerInputMap.sInputMap.FindAction("Close").bindings[0].effectivePath);

                close.started += ctx2 => {
                    Debug.Log("hi there");
                    if (GamePaused) {
                        ResumeGame();
                        close.Disable();
                    }
                };
                close.Enable();
                PauseGame();
            }
        };
        gameObject.SetActive(false);
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
        SoundManager.PlaySound(SoundManager.Sound.MouseClick, 1.0f);
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;

        OnResume?.Invoke();
        PlayerInputMap.sInputMap.Enable();
    }
    public void PauseGame()
    {
        SoundManager.PlaySound(SoundManager.Sound.MouseClick, 1.0f);
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;

        Debug.Log("on pause");
        OnPause?.Invoke();
        PlayerInputMap.sInputMap.Disable();
    }
    
    public void QuitGame()
    {
        SoundManager.PlaySound(SoundManager.Sound.MouseClick, 1.0f);
        Application.Quit();
    }

    private void DisablePause() {
        if (GamePaused)
            ResumeGame();
        SoundManager.PlaySound(SoundManager.Sound.MouseClick, 1.0f);
        canPause = false;
    }

    public void QuitConfirmOpen () {
        
    }

}

