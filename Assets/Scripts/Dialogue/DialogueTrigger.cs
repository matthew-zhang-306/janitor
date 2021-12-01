using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueLines[] script;
    public SpeakerData[] speakers;

    public GameObject dialogueBubblePrefab;
    public bool isOneShot;
    
    BaseDialogueUI dialogue = null;
    PlayerController playerIn;
    bool inDialogue => dialogue != null;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !inDialogue) {
            playerIn = other.GetComponent<PlayerController>();
            
            dialogue =
                GameObject.Instantiate(dialogueBubblePrefab, transform.position, Quaternion.identity)
                .GetComponent<BaseDialogueUI>();
            dialogue.script = script;
            dialogue.speakers = speakers;
            dialogue.StartDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") && inDialogue) {
            dialogue.StopDialogue();
            dialogue = null;
        }

        if (isOneShot) {
            gameObject.SetActive(false);
        }
    }


}
