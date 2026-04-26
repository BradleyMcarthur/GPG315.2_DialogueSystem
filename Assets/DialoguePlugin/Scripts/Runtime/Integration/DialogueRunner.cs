using UnityEngine;

//Runtime script for swapping/displaying nodes and running through conversations. UI and trigger use it. Also has language set function
public class DialogueRunner : MonoBehaviour
{
    [Header("Dialogue Graph")]
    public DialogueGraph currentGraph;
    public string currentLanguage = "EN";
    
    [Header("UI")]
    public DialogueUI dialogueUI;
    
    private DialogueNodeData currentNode;
    
    public void StartDialogue(DialogueGraph graph)
    {
        currentGraph = graph;
        currentNode = graph.nodes.Find(node => node.isStartNode);

        if (currentNode == null)
        {
            currentNode = graph.nodes[0];
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

        DialogueNodeData nextNode = currentGraph.GetNode(nextID);

        if (nextNode == null)
        {
            Debug.LogWarning("Node not found: " + nextID);
            return;
        }

        currentNode = nextNode;
        DisplayNode();
    }
    
    public void SetLanguage(string lang)
    {
        currentLanguage = lang;

        // refresh current dialogue if active
        if (currentNode != null)
        {
            DisplayNode();
        }
    }
    
    void DisplayNode()
    {
        if (dialogueUI != null)
        {
            dialogueUI.DisplayNode(currentNode, this);
        }
    }
}
