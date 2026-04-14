using UnityEngine;

public class DialogueDebugger : MonoBehaviour
{
    public DialogueGraph graph;

    void Start()
    {
        var startNode =
            graph.GetStartNode();

        Debug.Log(
            "Start Node: "
            + startNode.dialogueText
        );

        var nextNode =
            graph.GetNode("Node2");

        Debug.Log(
            "Next Node: "
            + nextNode.dialogueText
        );
    }
}
