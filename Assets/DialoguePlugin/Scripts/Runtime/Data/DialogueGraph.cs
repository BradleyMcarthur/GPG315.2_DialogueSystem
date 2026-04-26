using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueGraph", menuName = "Dialogue/Dialogue Graph")]
public class DialogueGraph : ScriptableObject
{
    [Header("Dialogue Nodes")]
    public List<DialogueNodeData> nodes = new List<DialogueNodeData>();

    public DialogueNodeData GetNode(string id)
    {
        return nodes.Find(node => node.nodeID == id);
    }

    //dont remember what i was gonna use this for but didnt use it so idk
    public DialogueNodeData GetStartNode()
    {
        if (nodes.Count > 0)
            return nodes[0];

        return null;
    }
}
