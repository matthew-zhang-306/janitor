using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ClickSound() {
        SoundManager.PlaySound(SoundManager.Sound.MouseClick, 1.0f);
    }
}
