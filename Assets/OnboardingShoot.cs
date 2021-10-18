using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnboardingShoot : MonoBehaviour
{

    public GameObject MouseClick;
    public GameObject MobileShoot;


    // Start is called before the first frame update
    void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            MouseClick.SetActive(true);
        }
        if (Application.isMobilePlatform)
        {
            MobileShoot.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
