using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCleanBar : MonoBehaviour
{
    public FloorController Floor;
    public Slider cleanBar;


    // Update is called once per frame
    void Update()
    {
        cleanBar.value = Mathf.Clamp01(Floor.GetCleanPercent() / 0.8f);
    }
}
