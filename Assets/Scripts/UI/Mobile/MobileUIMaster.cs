using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Canvas))]
public class MobileUIMaster : MonoBehaviour
{
    private Camera cam;
    public MobileBase[] elements;

    void Start ()
    {
        #if (UNITY_ANDROID || UNITY_IPHONE)
        cam = this.GetComponent<Canvas>().worldCamera;        
        #else
        this.gameObject.SetActive(false);
        #endif
        
    }
    void Update ()
    {
        foreach (var ui in elements) {
            bool used = false;
            for (int i = 0; i < Input.touchCount; i++) 
            {
                var touch = Input.GetTouch (i);

                var worldPos = cam.ScreenToWorldPoint(touch.position);
                if (ui.Within(touch.position, cam))
                {
                    ui.Apply(touch.position, cam);
                    used = true;
                    //one touch per elem
                    break;
                }
            }   
            if (!used)
            {
                ui.Reset();
            }
        }
    }
}