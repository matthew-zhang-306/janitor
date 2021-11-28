using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightFlicker : BaseRoomObject
{
    float randomDelay;
    public bool IgnoreRoomStatus;
    private Transform PointLight;
    private Transform SpriteLight;
    float StartingIntensity;
    private UnityEngine.Experimental.Rendering.Universal.Light2D Light;
    private UnityEngine.Experimental.Rendering.Universal.Light2D Light2;
    bool turnOn;
    // Start is called before the first frame update
    void Start()
    {
        PointLight = transform.Find("Point Light 2D");
        SpriteLight = transform.Find("Sprite Light 2D");
        Light = SpriteLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        Light2 = PointLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        Light.enabled = false;
        Light2.enabled = false;
    }
    private void Update()
    {
        if (IsRoomActive || IgnoreRoomStatus)
        {
            StartCoroutine("randomTurnOn");
            
        }
    }
    IEnumerator randomTurnOn()
    {
        randomDelay = Random.Range(0,.5f);

        yield return new WaitForSeconds(randomDelay);
        Light.enabled = true;
        Light2.enabled = true;
    }
}
