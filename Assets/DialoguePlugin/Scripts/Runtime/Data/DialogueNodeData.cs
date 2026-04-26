using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialogueNodeData
{
    [Header("Node Info")]
    public string nodeID;
    public string characterID;
    public bool isStartNode = false;

    [TextArea(3, 5)]
    public string dialogue_EN;
    public string dialogue_FR;
    public string dialogue_JP;

    [Header("Branching")]
    public List<string> choices = new List<string>();

    public List<string> nextNodeIDs = new List<string>();
    
    public Vector2 position;
    
    [System.NonSerialized]
    public Vector2 scrollPosition;
    
    //Returns text to UI based on language thats set
    public string GetText(string language)
    {
        switch (language)
        {
            case "EN":
                return dialogue_EN;

            case "FR":
                return dialogue_FR;

            case "JP":
                return dialogue_JP;

            default:
                return dialogue_EN;
        }
    }
}