using UnityEngine;

[CreateAssetMenu(fileName = "HunterCharacter", menuName = "ScriptableObjects/Hunter")]

public class Hunter : Character
{
    public void Awake()
    {
        characterName = "Hunter";
    }
}


