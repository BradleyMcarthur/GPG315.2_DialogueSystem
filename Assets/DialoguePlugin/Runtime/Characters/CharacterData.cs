using UnityEngine;

[CreateAssetMenu(
    fileName = "Character",
    menuName = "Dialogue/Character"
)]
public class CharacterData : ScriptableObject
{
    [Header("Character Info")]

    public string characterID;

    public string characterName;

    public Sprite portrait;
}
