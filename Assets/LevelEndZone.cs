using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndZone : MonoBehaviour
{
    public static EmptyDelegate OnLevelEnd;

    public void SkipLevel()
    {
        LoadNextLevel();
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LoadNextLevel();
        }
    }

   void LoadNextLevel() {
        OnLevelEnd?.Invoke();

        this.Invoke(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1), 2f);
        
    }
}
