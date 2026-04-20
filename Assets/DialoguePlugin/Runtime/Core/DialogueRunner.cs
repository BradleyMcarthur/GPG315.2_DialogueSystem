using UnityEngine;

public class DialogueRunner : MonoBehaviour
{
    [Header("Dialogue Graph")]
    public DialogueGraph dialogueGraph;
    
    [Header("UI")]
    public DialogueUI dialogueUI;
    
    private DialogueNodeData currentNode;

    void Start()
    {
        //StartDialogue(dialogueGraph); //for testing
    }
    
    public void StartDialogue(DialogueGraph dialogueGraph)
    {
        if (dialogueGraph == null)
        {
            Debug.LogError("There is no DialogueGraph");
            return;
        }
        currentNode = dialogueGraph.GetStartNode();

        if (currentNode == null)
        {
            Debug.LogError("Couldnt find start node");
            return;
        }
        DisplayNode();
    }

    public void ChooseOption(int index)
    {
        if (currentNode == null)
            return;

        if (index >= currentNode.nextNodeIDs.Count)
        {
            Debug.LogWarning("Invalid choice index");
            return;
        }

        string nextID = currentNode.nextNodeIDs[index];

        DialogueNodeData nextNode = dialogueGraph.GetNode(nextID);

        if (nextNode == null)
        {
            Debug.LogWarning("Node not found: " + nextID);
            return;
        }

        currentNode = nextNode;
        DisplayNode();
    }

    void DisplayNode()
    {
        if (dialogueUI != null)
        {
            dialogueUI.DisplayNode(
                currentNode,
                this
            );
        }
    }
}
