using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFloorClean (int amount);
public class FloorMarker : MonoBehaviour
{
    [Tooltip("The \"damage\" this object does to the floor. Use a positive number for cleaning and a negative number for dirtying.")]
    public int markAmount;
    public OnFloorClean callback;
    
}
