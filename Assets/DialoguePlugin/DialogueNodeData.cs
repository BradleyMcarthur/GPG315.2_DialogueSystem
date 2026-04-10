using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueNodeData
{
    public string nodeID;

    public string characterID;

    [TextArea(3, 5)]
    public string dialogueText;

    public List<string> choices = new List<string>();

    public List<string> nextNodeIDs = new List<string>();
}