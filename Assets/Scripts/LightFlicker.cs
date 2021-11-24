using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class LightFlicker : MonoBehaviour
{
    float randomDelay;
    public bool flicker;
    private Transform PointLight;
    private Transform SpriteLight;
    float StartingIntensity;
    private UnityEngine.Experimental.Rendering.Universal.Light2D Light;
    private UnityEngine.Experimental.Rendering.Universal.Light2D Light2;
    // Start is called before the first frame update
    void Start()
    {
        PointLight = transform.Find("Point Light 2D");
        SpriteLight = transform.Find("Sprite Light 2D");
        Light = SpriteLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        Light2 = PointLight.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        StartingIntensity = Light.intensity;
        flicker = true;
    }
    private void Update()
    {
        if (flicker)
        {
            flicker = false;
            StartCoroutine("RandomFlickering");
        }
    }
    IEnumerator RandomFlickering()
    {
        randomDelay = Random.Range(0,1.5f);
        Light.intensity = .1f;
        Light2.intensity = .1f;
        yield return new WaitForSeconds(randomDelay);
        Light.intensity = StartingIntensity;
        Light2.intensity = StartingIntensity;
        flicker = true;
    }
}
