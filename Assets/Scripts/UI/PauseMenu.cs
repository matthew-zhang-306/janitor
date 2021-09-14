using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;
    public GameObject PauseMenuUI;
    public GameObject RestartUI;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused == true)
            {
                ResumeGame();
            }
            if (GamePaused == false)
            {
                PauseGame();
            }
        }
        if (PlayerController.PlayerDead == true)
        {
            RestartUI.SetActive(true);
            Time.timeScale = .1f;
        }
    }

    public void ResumeGame()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }
    public void PauseGame()
    {
        
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
        
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
