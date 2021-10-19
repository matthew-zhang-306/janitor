using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDTutorial : MonoBehaviour
{
    public GameObject WASDUI;
    bool playerInTrigger;
    public GameObject Joystick;

    public float TimeDisplayed;

    bool pcBuild = true;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            pcBuild = true;
        }
        if (Application.isMobilePlatform)
        {
            pcBuild = false;
        }


        if (pcBuild)
        {
            StartCoroutine(AnimationDelay());
            WASDUI.SetActive(true);
            
        }
        else
        {
            StartCoroutine(AnimationDelay());
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
