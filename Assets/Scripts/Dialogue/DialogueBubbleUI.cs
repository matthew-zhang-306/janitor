using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueBubbleUI : BaseDialogueUI
{
    public TextMeshPro speakerText;
    public SpriteRenderer speakerBox;

    public Vector2 bubbleOffset;
    public float openTime;
    Tween openTween;
    Vector3 onPosition;
    Vector3 offPosition;


    protected override IEnumerator OnStart() {
        yield break;
    }

    protected override IEnumerator OnSpeakerSwitch(string speaker) {
        /*
        SpeakerData speaker = dialogue.GetSpeaker(dialogueLines.speaker);
        speakerText.text = speaker.name;
        SetColor(speaker.color);
        SetPositions(speaker, cameraPos);

        openTween?.Kill();
        openTween = DOTween.Sequence()
            .Insert(0, transform.DOScale(Vector3.one, openTime).SetEase(Ease.OutCubic))
            .Insert(0, transform.DOMove(onPosition, openTime).SetEase(Ease.OutCubic));
        
        yield return new WaitForSeconds(openTime);
        */
        yield break;
    }

    protected override IEnumerator OnStartLine() {
        yield break;
    }
    
    protected override IEnumerator OnContinue() {
        yield break;
    }

    protected override IEnumerator OnSkip() {
        yield break;
    }

    protected override IEnumerator OnEndLine() {
        yield break;
    }

    protected override IEnumerator OnEnd() {
        Destroy(gameObject);
        yield return 0;
    }


/*
    private void SetColor(Color color) {
        speakerBox.color = color;
        downArrow.GetComponent<SpriteRenderer>().color = color;
    }

    private void SetPositions(SpeakerData speaker, Vector3 cameraPos) {
        Vector2 fullBubbleOffset = bubbleOffset + speaker.size / 2;
        Vector3 diff = cameraPos - speaker.actor.transform.position;
        Vector2 actualOffset = new Vector2(Mathf.Min(Mathf.Abs(diff.x), fullBubbleOffset.x) * Mathf.Sign(diff.x), fullBubbleOffset.y * Mathf.Sign(diff.y));
        
        offPosition = speaker.actor.transform.position;
        onPosition = offPosition + actualOffset.ToVector3();
        transform.position = offPosition;
        transform.localScale = Vector3.zero;
    }

    private void MoveToBack() {
        dialogueText.sortingOrder -= 5;
        downArrow.GetComponent<SpriteRenderer>().sortingOrder -= 1;
        dialogueBox.sortingOrder -= 5;
    }
*/
}
