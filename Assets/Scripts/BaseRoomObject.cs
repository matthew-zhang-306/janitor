using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoomObject : MonoBehaviour
{

    protected bool IsRoomActive;

    public virtual void SetRoomActive(bool isActive)
    {
        IsRoomActive = isActive;
    }
}
