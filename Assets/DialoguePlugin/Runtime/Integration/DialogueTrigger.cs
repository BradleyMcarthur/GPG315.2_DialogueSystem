using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Dialogue")]

    public DialogueGraph dialogueGraph;
    public DialogueRunner dialogueRunner;

    [Header("Trigger Settings")]

    public bool triggerOnEnter = true;
    public bool requireKeyPress = false;
    public KeyCode interactionKey = KeyCode.E;
    
    private bool playerInRange = false;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter)
            return;

        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!requireKeyPress)
            {
                StartDialogue();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void Update()
    {
        if (requireKeyPress && playerInRange)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                StartDialogue();
            }
        }
    }

    private void StartDialogue()
    {
        if (hasTriggered)
            return;

        if (dialogueRunner != null &&
            dialogueGraph != null)
        {
            hasTriggered = true;

            dialogueRunner.StartDialogue(dialogueGraph);
        }
    }
}
