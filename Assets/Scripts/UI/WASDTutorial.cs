using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDTutorial : MonoBehaviour
{
    public GameObject WASDUI;
    bool playerInTrigger;
    public GameObject Joystick;

    public float TimeDisplayed;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            StartCoroutine(AnimationDelay());
            WASDUI.SetActive(true);
        }
        if (Application.isMobilePlatform)
        {
            Joystick.SetActive(true);
        }
    }

    IEnumerator AnimationDelay()
    {
        
        yield return new WaitForSeconds(TimeDisplayed);
        WASDUI.SetActive(false);
        Joystick.SetActive(false);
        
    }
}
