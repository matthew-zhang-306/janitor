using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndZone : MonoBehaviour
{
    public static EmptyDelegate OnLevelEnd;

    public void SkipLevel()
    {
        
        OnLevelEnd?.Invoke();
        Helpers.Invoke(this, LoadNextLevel, 2f);
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            OnLevelEnd?.Invoke();
            Helpers.Invoke(this, LoadNextLevel, 2f);
        }
    }

   void LoadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
