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
    }

    IEnumerator AnimationDelay()
    {
        
        yield return new WaitForSeconds(TimeDisplayed);
        WASDUI.SetActive(false);
        Joystick.SetActive(false);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (pcBuild)
            {
                
                WASDUI.SetActive(true);

            }
            else
            {
                
                Joystick.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {  
        if (collision.tag == "Player")
        {
            StartCoroutine(AnimationDelay());
        }
            

    }
}
