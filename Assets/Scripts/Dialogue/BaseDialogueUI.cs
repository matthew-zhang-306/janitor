using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

[System.Serializable]
public class DialogueScript {
    public DialogueLines[] lines;
}

[System.Serializable]
public class DialogueLines {
    public string speaker;
    [TextArea(3, 10)] public string lines;
}

[System.Serializable]
public class SpeakerData {
    public string speakerName;
    public GameObject actor;
    public Color color;
}

public class BaseDialogueUI : MonoBehaviour
{
    public DialogueLines[] script;
    public SpeakerData[] speakers;

    public TextMeshProUGUI dialogueText;
    public GameObject continueObject;
    
    public float scrollSpeed;
    public float periodPauseAmount = 5f;
    public float commaPauseAmount = 2f;

    protected Coroutine dialogueCoroutine;
    protected bool advanceInput;
    protected bool oldAdvanceInput;
    protected bool skipInput;

    public UnityEvent OnDialogueEnd;


    private void Start() {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    protected virtual void Update() {
        GetInput();
    }

    protected virtual void GetInput() {
        oldAdvanceInput = advanceInput;
        advanceInput = CustomInput.GetButton("Fire1");
        skipInput = CustomInput.GetButton("Fire2");
    }


    public void StartDialogue() {
        dialogueCoroutine = StartCoroutine(RunDialogue());
    }

    public void StopDialogue() {
        StopCoroutine(dialogueCoroutine);
        StartCoroutine(OnEndLine());
        StartCoroutine(OnEnd());
        OnDialogueEnd?.Invoke();
    }


    public virtual IEnumerator RunDialogue() {
        dialogueText.text = "";
        yield return StartCoroutine(OnStart());

        foreach (DialogueLines lines in script) {
            yield return StartCoroutine(OnSpeakerSwitch(lines.speaker));

            foreach (string line in lines.lines.Split('\n')) {
                // reset input to not carry over an input from the previous dialogue bubble into this one
                GetInput();

                yield return StartCoroutine(OnStartLine());
                yield return StartCoroutine(DisplayLine(line));
                yield return StartCoroutine(OnEndLine());
            }
        }

        yield return StartCoroutine(OnEnd());
        OnDialogueEnd?.Invoke();
    }

    protected IEnumerator DisplayLine(string line) {
        dialogueText.text = line;
        dialogueText.maxVisibleCharacters = 0;
        continueObject.SetActive(false);

        float scrollTimer = 1;
        while (dialogueText.maxVisibleCharacters < line.Length) {
            if ((advanceInput && !oldAdvanceInput) || skipInput) {
                dialogueText.maxVisibleCharacters = line.Length;
                oldAdvanceInput = advanceInput;
            } else {
                scrollTimer -= scrollSpeed * Time.deltaTime;
                while (scrollTimer <= 0) {
                    if (dialogueText.text.Length > dialogueText.maxVisibleCharacters && dialogueText.text[dialogueText.maxVisibleCharacters] == '.') {
                        scrollTimer += periodPauseAmount;
                    }
                    else if (dialogueText.text.Length > dialogueText.maxVisibleCharacters && dialogueText.text[dialogueText.maxVisibleCharacters] == ',') {
                        scrollTimer += commaPauseAmount;
                    }
                    else {
                        scrollTimer += 1f;
                    }
                    dialogueText.maxVisibleCharacters++;
                }

                yield return 0;
            }
        }

        continueObject.SetActive(true);

        while (!(advanceInput && !oldAdvanceInput) && !skipInput) {
            yield return 0;
        }

        if (skipInput) {
            yield return StartCoroutine(OnSkip());
        }
        else {
            yield return StartCoroutine(OnContinue());
        }

        oldAdvanceInput = advanceInput;
    }

    protected virtual IEnumerator OnStart() {
        yield break;
    }

    protected virtual IEnumerator OnSpeakerSwitch(string speaker) {
        yield break;
    }

    protected virtual IEnumerator OnStartLine() {
        yield break;
    }

    protected virtual IEnumerator OnContinue() {
        yield break;
    }

    protected virtual IEnumerator OnSkip() {
        yield break;
    }

    protected virtual IEnumerator OnEndLine() {
        yield break;
    }

    protected virtual IEnumerator OnEnd() {
        yield break;
    }


    public SpeakerData GetSpeaker(string speakerName) {
        for (int i = 0; i < speakers.Length; i++) {
            if (speakers[i].speakerName == speakerName) {
                return speakers[i];
            }
        }

        Debug.LogError("Dialogue UI does not have a speaker named " + speakerName);
        return null;
    }

}
