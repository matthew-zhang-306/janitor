using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KeyCardCollectable : Interactable
{
    public Transform spriteContainer;

    private void Start() {
        spriteContainer.DOScaleX(-1, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public override void DoAction (PlayerController pc, Inventory i)
    {
        i.numKeys += 1;
        spriteContainer.DOKill();
        Destroy (gameObject);
        SoundManager.PlaySound(SoundManager.Sound.KeyCollecting, 1f);
    }
}
