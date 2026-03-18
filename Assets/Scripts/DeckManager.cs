using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject hand, deck, discard, play;
    public List<GameObject> masterDeck, startingDeck = new List<GameObject>();
    public List<GameObject> deckContents, handContents, discardContents, playContents = new List<GameObject>();
    private List<List<GameObject>> posibleCardLocations = new List<List<GameObject>>();
    private float relativeSpaceBetweenCards = 0.4f;
    private float selectedCardHeightIncrease = 0.25f;

    private int handSize;
    private int startHandSize = 5;
    private CameraScript cameraScript;
    private MouseManager mouseManager;





    //public DiscardScript discardScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

        //Debug.Log(deck.transform.childCount);
        if (startingDeck.Count == 0)
        {
            for (int i = 0; i < deck.transform.childCount; i++)
            {
                startingDeck.Add(deck.transform.GetChild(i).gameObject);
                Debug.Log(deck.transform.GetChild(i).gameObject);

            }
        }
        deckContents = new List<GameObject>(startingDeck);
        masterDeck = new List<GameObject>(startingDeck);

        posibleCardLocations.Add(deckContents);
        posibleCardLocations.Add(handContents);
        posibleCardLocations.Add(discardContents);
        posibleCardLocations.Add(playContents);
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void PlayCard(GameObject card)
    {
        MoveTo(card, play);
    }

    public void DrawNewHand()
    {
        int cardsInHand = handSize;
        for (int i = 0; i < cardsInHand; i++)
        {
            DiscardFirstCard();
            //Debug.Log("card Discarded");
        }
        for (int i = 0; i < startHandSize; i++)
        {
            DrawCard();
        }
    }

    public void GainCard(GameObject card)
    {
        masterDeck.Add(card);
        MoveTo(card, deck, Random.Range(0, deckContents.Count + 1));
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

    public void MoveTo(GameObject card, GameObject location, int newIndex = 0)
    {

        foreach (List<GameObject> posibleLocation in posibleCardLocations)
        {
            if (posibleLocation.Contains(card))
            {
                posibleLocation.Remove(card);
            }
        }
        GetListByName(location.name.ToLower() + "Contents").Insert(newIndex, card);
        card.transform.SetParent(location.transform);
        card.transform.position = location.transform.position;
        if (location == hand || location == play)
        {
            card.gameObject.SetActive(true);
        }
        else
        {
            card.gameObject.SetActive(false);
        }
        mouseManager.MouseOffObject(card);
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
    public void SelectCard(GameObject card)
    {
        if (card.transform.localScale == Vector3.one)
        {
            card.transform.localScale = new Vector3(2, 2, 1);
            card.transform.position = card.transform.position + new Vector3(0, selectedCardHeightIncrease * cameraScript.zoom, 0);
        }
    }
    public void DeSelectCard(GameObject card)
    {
        if (card.transform.localScale == new Vector3(2, 2, 1))
        {
            card.transform.localScale = Vector3.one;
            card.transform.position = card.transform.position - new Vector3(0, selectedCardHeightIncrease * cameraScript.zoom, 0);
        }
    }

    public void UpdateHand()
    {
        handSize = handContents.Count;
        SeperateCards(handContents, hand.transform.position);
    }

    public void SeperateCards(List<GameObject> cards, Vector2 pos)
    {
        float spaceBetweenCards = relativeSpaceBetweenCards * cameraScript.widthHeightRatio;
        int numberOfCards = cards.Count;
        //Debug.Log(spaceBetweenCards + " spaceBetweenCards");
        //Debug.Log(cameraScript.widthHeightRatio + " widthHeightRatio");

        foreach (GameObject card in cards)
        {
            //Debug.Log(cards.IndexOf(card) + " index");
            card.transform.position = new Vector3((((float)numberOfCards - 1) / 2 - cards.IndexOf(card)) * spaceBetweenCards + pos.x, pos.y, card.transform.position.z);
            //Debug.Log((((float)numberOfCards - 1) / 2 - cards.IndexOf(card)) * spaceBetweenCards + " new x pos");
            if (card.transform.localScale != Vector3.one)
            {
                card.transform.localScale = Vector3.one;
                SelectCard(card);
            }
        }
    }
}
