using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTutorial : MonoBehaviour
{
    public GameObject DashUI;
    bool playerInTrigger;
    bool canflicker;

    private void FixedUpdate()
    {
        
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
        playerInTrigger = false;
        canflicker = false;
        DashUI.SetActive(false);
    }

    IEnumerator Flicker()
    {
        
        while (canflicker == true)
        {
            DashUI.SetActive(true);
            yield return new WaitForSeconds(2f);
            DashUI.SetActive(false);
            yield return new WaitForSeconds(.5f);
            DashUI.SetActive(true);
        }
        DashUI.SetActive(false);


    }

    IEnumerator PlayerDelay()
    {
        yield return new WaitForSeconds(2f);
        if (playerInTrigger == true)
        {
            canflicker = true;
            StartCoroutine(Flicker());
        }
    }
}
