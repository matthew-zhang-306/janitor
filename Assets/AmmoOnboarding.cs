using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoOnboarding : MonoBehaviour
{
    
    public GameObject LowAmmoUI;
    public static bool AmmoLow;
   
    // Start is called before the first frame update
    void Start()
    {
        LowAmmoUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (AmmoLow)
        {
            LowAmmoUI.SetActive(true);
        }
        else
        {
            LowAmmoUI.SetActive(false);
        }
        
    }
}
