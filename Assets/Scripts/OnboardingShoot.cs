using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingShoot : MonoBehaviour
{

    public GameObject MouseClick;
    public GameObject MobileShoot;

    private bool PCBuild = true;
    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            PCBuild = true;
            
        }
        if (Application.isMobilePlatform)
        {
            PCBuild = false;
        }


        if (PCBuild)
        {
            MouseClick.SetActive(true);
        }
        if (PCBuild == false)
        {
            MobileShoot.SetActive(true);
        }
    }

   
}
