using UnityEngine;

[CreateAssetMenu(fileName = "WizardCharacter", menuName = "ScriptableObjects/Wizard")]

public class Wizard : Character
{
    public void Awake()
    {
        characterName = "Wizard";
    }
}
