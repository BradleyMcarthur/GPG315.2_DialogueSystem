using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueNodeData
{
    [Header("Node Info")]
    public string nodeID;

    public string characterID;

    [TextArea(3, 5)]
    public string dialogueText;

    [Header("Branching")]
    public List<string> choices = new List<string>();

    public List<string> nextNodeIDs = new List<string>();
}