using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IntroDialogueUI : BaseDialogueUI
{
    private void Start() {
        StartDialogue();
    }


    private float maxVisibleCharactersF;

    protected override IEnumerator OnContinue() {
        maxVisibleCharactersF = dialogueText.maxVisibleCharacters;
        continueObject.SetActive(false);

        dialogueText.DOKill();
        yield return DOTween.To(
            () => maxVisibleCharactersF,
            f => {
                maxVisibleCharactersF = f;
                dialogueText.maxVisibleCharacters = (int)maxVisibleCharactersF;
            },
            0,
            Mathf.Min(maxVisibleCharactersF / (scrollSpeed * 3), 0.5f)
        ).SetEase(Ease.Linear)
        .SetLink(gameObject)
        .SetTarget(dialogueText)
        .WaitForCompletion();
    
        yield return new WaitForSeconds(0.1f);
    }
}
