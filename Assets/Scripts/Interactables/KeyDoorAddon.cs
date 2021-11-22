using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeyDoorAddon : Interactable
{
    [SerializeField] private SpriteRenderer lockedOverlay = default;
    bool isOpen = false;

    private void Start() {
        GetComponent<Door>().CloseDoor();

        // locked overlay bobbing
       /* lockedOverlay.transform.DOLocalMoveY(
            lockedOverlay.transform.localPosition.y - 0.2f, 1f
        ).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);*/
    }

    private void Update() {
        // locked overlay flickering
        // lockedOverlay.enabled = Random.Range(0f, 1f) < 0.95f;
    }

    public override void OnEnter(PlayerController pc, Inventory i)
    {
        if (isOpen)
            return;
        
        if (i.numKeys == 0) {
            _tooltip = "Requires Key Card";
        }
        else {
            _tooltip = "[e] Unlock";   
        }
    }

    public override void DoAction (PlayerController pc, Inventory i)
    {
        if (isOpen)
            return;
        
        if (i.numKeys == 0) {
            // do some animation or something to indicate lockedness
            return;
        }

        isOpen = true;
        i.numKeys -= 1;
        _tooltip = "";

        GetComponent<Door>().OpenDoor();
        // lockedOverlay.DOFade(0, 0.5f);
    }
}
