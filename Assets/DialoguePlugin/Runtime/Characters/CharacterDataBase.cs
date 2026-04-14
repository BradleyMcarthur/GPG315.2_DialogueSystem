using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterDatabase",
    menuName = "Dialogue/Character Database"
)]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters = new List<CharacterData>();

    public CharacterData GetCharacter(string id)
    {
        return characters.Find(character => character.characterID == id);
    }
}
