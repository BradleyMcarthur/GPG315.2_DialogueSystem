using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI currentLanguageText;

    public Image portraitImage;
    public Button[] choiceButtons;

    [Header("Character Database")]
    public CharacterDatabase characterDatabase;

    //Runner displays each node to the Ui for player to read and interact with
    public void DisplayNode(DialogueNodeData node, DialogueRunner runner)
    {
        if (node == null)
            return;
        
        //grabs character data
        CharacterData character = characterDatabase.GetCharacter(node.characterID);

        if (character != null)
        {
            characterNameText.text = character.characterName;

            portraitImage.sprite = character.portrait;
        }
        else
        {
            characterNameText.text = node.characterID;
        }
        
        dialogueText.text = node.GetText(runner.currentLanguage);
        currentLanguageText.text = runner.currentLanguage;

        //setup buttons and only display needed ones (eg. 2 response choices = 2 buttons)
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < node.choices.Count)
            {
                choiceButtons[i].gameObject.SetActive(true);

                int index = i;

                choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.choices[i];

                choiceButtons[i].onClick.RemoveAllListeners();

                choiceButtons[i].onClick.AddListener(() => runner.ChooseOption(index));
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
