using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject hand, deck, discard;
    public List<GameObject> deckContents, handContents, discardContents = new List<GameObject>();
    public List<GameObject> masterDeck, startingDeck = new List<GameObject>();
    private List<List<GameObject>> posibleCardLocations = new List<List<GameObject>>();
    private float relativeSpaceBetweenCards = 0.2f;
    private float handSize;
    private CameraScript cameraScript;





    //public DiscardScript discardScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();

        if (startingDeck.Count == 0)
        {
            for (int i = 0; i < deck.transform.childCount; i++)
            {
                startingDeck.Add(deck.transform.GetChild(i).gameObject);
            }
        }
        deckContents = new List<GameObject>(startingDeck);
        masterDeck = new List<GameObject>(startingDeck);

        posibleCardLocations.Add(deckContents);
        posibleCardLocations.Add(handContents);
        posibleCardLocations.Add(discardContents);



    }

    // Update is called once per frame
    void Update()
    {


    }

    public void DrawCard()
    {
        if (deckContents.Count == 0)
        {
            ReSuffle();
        }
        GameObject currentCard = deckContents[0];
        MoveTo(currentCard, hand);
    }
    public void ReSuffle()
    {
        int discardSize = discardContents.Count;
        for (int i = 0; i < discardSize; i++)
        {
            GameObject currentCard = discardContents[Random.Range(0, discardContents.Count)];
            MoveTo(currentCard, deck);
        }
    }

    public void MoveTo(GameObject card, GameObject location)
    {

        foreach (List<GameObject> posibleLocation in posibleCardLocations)
        {
            if (posibleLocation.Contains(card))
            {
                posibleLocation.Remove(card);
            }
        }
        GetListByName(location.name.ToLower() + "Contents").Add(card);
        card.transform.SetParent(location.transform);
        card.transform.position = location.transform.position;
        if (location == hand)
        {
            card.gameObject.SetActive(true);
        }
        else
        {
            card.gameObject.SetActive(false);
        }
        UpdateHand();
    }

    public List<GameObject> GetListByName(string listName)
    {
        //Debug.Log(listName);
        return (List<GameObject>)GetType().GetField(listName).GetValue(this);
    }
    public void DiscardFirstCard()
    {
        GameObject firstCard = handContents[0];
        DiscardCard(firstCard);
    }
    public void DiscardCard(GameObject currentCard)
    {
        MoveTo(currentCard, discard);
    }

    public void UpdateHand()
    {
        float spaceBetweenCards = relativeSpaceBetweenCards * cameraScript.widthRatio;
        handSize = handContents.Count;
        foreach (GameObject card in handContents)
        {
            card.transform.position = hand.transform.position + Vector3.left * ((handSize - 1) / 2 - handContents.IndexOf(card)) * spaceBetweenCards;
        }

    }
}
