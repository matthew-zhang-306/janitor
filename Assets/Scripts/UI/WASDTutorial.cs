using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WASDTutorial : MonoBehaviour
{
    public GameObject WASDUI;
    bool playerInTrigger;
    

    private void Start()
    {
        
        StartCoroutine(PlayerDelay());
        WASDUI.SetActive(true);
    }


    

    IEnumerator PlayerDelay()
    {
        
        yield return new WaitForSeconds(5f);
        WASDUI.SetActive(false);
        
        
    }
}
