using UnityEngine;

[CreateAssetMenu(fileName = "TinkererCharacter", menuName = "ScriptableObjects/Tinkerer")]

public class Tinkerer : Character
{
    public void Awake()
    {
        characterName = "Tinkerer";
    }
}


