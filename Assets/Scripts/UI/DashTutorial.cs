using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    public GameObject DashUI;
    public GameObject MobileDash;
    bool playerInTrigger;
    bool canflicker;


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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player entered my no on square");
            StartCoroutine(PlayerDelay());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Debug.Log("Player still in my no no square.");
            playerInTrigger = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        StartCoroutine("UIStaysOn");
    }

    
   
    IEnumerator UIStaysOn()
    {
        yield return new WaitForSeconds(2);
        playerInTrigger = false;
        canflicker = false;
        DashUI.SetActive(false);
        MobileDash.SetActive(false);
        StopCoroutine("UIStaysOn");
    }

    IEnumerator PlayerDelay()
    {
        yield return new WaitForSeconds(.4f);
        if (playerInTrigger == true && pcBuild == true)
        {
            DashUI.SetActive(true);
        }
        if (playerInTrigger == true && pcBuild == false)
        {
            MobileDash.SetActive(true);
        }
    }
}
