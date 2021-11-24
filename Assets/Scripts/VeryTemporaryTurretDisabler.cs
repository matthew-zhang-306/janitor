using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VeryTemporaryTurretDisabler : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        GetComponentInParent<RoomManager>().RoomObjects.ToList().ForEach(roomObject => roomObject.SetRoomActive(false));
    }
}
