using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent (typeof (Canvas))]
public class MobileUIMaster : MonoBehaviour
{
    private Camera cam;
    public MobileBase[] elements;

    void Start ()
    {
        #if (UNITY_ANDROID || UNITY_IPHONE)
        cam = this.GetComponent<Canvas>().worldCamera;    
        EnhancedTouchSupport.Enable();
    
        #else
        this.gameObject.SetActive(false);
        #endif
        
    }
    void Update ()
    {
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        foreach (var ui in elements) {
            bool used = false;

            for (int i = 0; i < touches.Count; i++) 
            {
                var touch = touches[i];

                var worldPos = cam.ScreenToWorldPoint(touch.screenPosition);
                if (ui.Within(touch.screenPosition, cam)) {
                
                    ui.Apply(touch.screenPosition, cam);
                    used = true;
                    //one touch per elem
                    
                }
            }   

            if (!used)
            {
                ui.Reset();
            }
        }
    }
}