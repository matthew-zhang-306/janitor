using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public delegate void Restart ();
public class PauseMenu : MonoBehaviour
{
    public Restart eventRestart;
    public static bool GamePaused = false;
    public GameObject PauseMenuUI;
    public GameObject RestartUI;
    public PlayerController player; // no

    void Start ()
    {
        player.onDeath += () => {
            GamePaused = true;
            RestartUI.SetActive(true);
            Time.timeScale = 0f;
            return;
        };
        eventRestart += player.ResetFromPrevious;
    }
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
        eventRestart();
        RestartUI.SetActive (false);
        ResumeGame();

        //Load room

        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
