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
        yield return dialogueText.DOFade(0f, 0.5f).SetEase(Ease.Linear).WaitForCompletion();
        dialogueText.color = dialogueText.color.WithAlpha(1);
        dialogueText.maxVisibleCharacters = 0;
    
        yield return new WaitForSeconds(0.1f);
    }
}
