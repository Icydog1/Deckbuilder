using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewCharacter", menuName = "ScriptableObjects/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    [SerializeField]
    private List<GameObject> startingDeck = new List<GameObject>();
    public List<GameObject> StartingDeck { get { return startingDeck; } }

}
