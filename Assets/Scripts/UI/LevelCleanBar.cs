using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCleanBar : MonoBehaviour
{
    public FloorController Floor;
    public Slider cleanBar;

    // this value is currently set in the inspector
    // but it should be equal to whatever threshold the room manager has
    public float cleanThreshold;


    // Update is called once per frame
    void Update()
    {
        cleanBar.value = Mathf.Clamp01(Floor.GetCleanPercent());
    }
}
