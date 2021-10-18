using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScene : MonoBehaviour
{
    public void StartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Cutscene()
    {
        SceneManager.LoadScene("Cutscene");
    }

    public void School()
    {
        SceneManager.LoadScene("School");
    }

    public void Sewer()
    {
        SceneManager.LoadScene("Sewer");
    }

    public void End()
    {
        SceneManager.LoadScene("End");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
