using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public DeckManager gameManager;
    public List<GameObject> deckContents = new List<GameObject>();
    public List<GameObject> startingDeck = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*for(int i = 0; i < transform.childCount; i++)
        {
            startingDeck.Add(transform.GetChild(i).gameObject);
            gameManager.deckContents.Add(transform.GetChild(i).gameObject);
        }
        //deckContents = startingDeck;
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
