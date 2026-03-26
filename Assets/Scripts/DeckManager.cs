using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.Rendering.GPUSort;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DeckManager : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject hand, deck, discard, play;
    public List<GameObject> entireDeck, startingDeck = new List<GameObject>();
    public List<GameObject> deckContents, handContents, discardContents, playContents = new List<GameObject>();
    public List<GameObject> DeckContents {  get { return deckContents; } }
    public List<GameObject> DiscardContents { get { return discardContents; } }
    public List<GameObject> EntireDeck { get { return entireDeck; } }

    private List<GameObject> displayedList = new List<GameObject>();
    private List<GameObject> displayedListName;
    private List<List<GameObject>> posibleCardLocations = new List<List<GameObject>>();
    private VariableDisplayer cardsInDeckDisplay, cardsInDiscardDisplay, cardsInEntireDeckDisplay;
    private float relativeSpaceBetweenCardsInHand = 0.35f;
    //if change set hand positon
    private float baseCardSize = 0.9f;
    public float BaseCardSize { get { return baseCardSize; } }

    private float selectedCardHeightIncrease = 0.25f;

    private int handSize;
    private int maxHandSize = 10;

    private int startHandSize = 5;
    private CameraScript cameraScript;
    private MouseManager mouseManager;
    private GameObject listDisplayer;
    private UIManager uIManager;

    private bool isDisplayingCards;
    public bool IsDisplayingCards { get { return isDisplayingCards; } }
    




    //public DiscardScript discardScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        cardsInDeckDisplay = GameObject.Find("CardsInDeckDisplay").GetComponent<VariableDisplayer>();
        cardsInDiscardDisplay = GameObject.Find("CardsInDiscardDisplay").GetComponent<VariableDisplayer>();
        cardsInEntireDeckDisplay = GameObject.Find("CardsInEntireDeckDisplay").GetComponent<VariableDisplayer>();
        listDisplayer = GameObject.Find("ListDisplayer");
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        //Debug.Log(deck.transform.childCount);
        if (startingDeck.Count == 0)
        {
            for (int i = 0; i < deck.transform.childCount; i++)
            {
                startingDeck.Add(deck.transform.GetChild(i).gameObject);
                //Debug.Log(deck.transform.GetChild(i).gameObject);
            }
        }
        //deckContents = new List<GameObject>(startingDeck);
        //entireDeck = new List<GameObject>(startingDeck);

        posibleCardLocations.Add(deckContents);
        posibleCardLocations.Add(handContents);
        posibleCardLocations.Add(discardContents);
        posibleCardLocations.Add(playContents);

        GameManager.GameStarted += SpawnStartingDeck;
        GameManager.GameStarted += DrawStartingHand;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void PlayCard(GameObject card)
    {
        MoveTo(card, play);
    }
    public void SpawnStartingDeck(GameManager gameManager)
    {
        foreach (GameObject card in startingDeck)
        {
            GameObject newCard = Instantiate(card);
            entireDeck.Add(newCard);
            deckContents.Add(newCard);
            newCard.transform.SetParent(deck.transform);
            newCard.gameObject.SetActive(false);
        }
        Suffle(ref deckContents);

    }

    public void DrawStartingHand(GameManager gameManager)
    {
        DrawNewHand();
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
        entireDeck.Add(card);
        cardsInEntireDeckDisplay.DisplayText(entireDeck.Count);
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
        /*
        int discardSize = discardContents.Count;
        for (int i = 0; i < discardSize; i++)
        {
            GameObject currentCard = discardContents[Random.Range(0, discardContents.Count)];
            MoveTo(currentCard, deck);
        }
        */
        while (discardContents.Count > 0)
        {
            MoveTo(discardContents[0], deck);
        }
        Suffle(ref deckContents);
    }

    public void Suffle(ref List<GameObject> list)
    {
        List<GameObject> tempList = new List<GameObject>(list);
        list.Clear();
        int listsize = tempList.Count;
        for (int i = 0; i < listsize; i++)
        {
            GameObject currentCard = tempList[Random.Range(0, tempList.Count)];
            list.Add(currentCard);
            tempList.Remove(currentCard);
        }
    }


    public void MoveTo(GameObject card, GameObject location, int newIndex = -1)
    {

        foreach (List<GameObject> posibleLocation in posibleCardLocations)
        {
            if (posibleLocation.Contains(card))
            {
                posibleLocation.Remove(card);
            }
        }
        if (newIndex != -1)
        {
            GetListByName(location.name.ToLower() + "Contents").Insert(newIndex, card);
        }
        else
        {
            GetListByName(location.name.ToLower() + "Contents").Add(card);
        }
        DeSelectCard(card);
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
        cardsInDeckDisplay.DisplayText(deckContents.Count);
        cardsInDiscardDisplay.DisplayText(discardContents.Count);

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
        
        if (GetRelativeCardSize(card) < 1.5f)
        {
            SetRelativeCardSize(card, 2);
            card.transform.position = card.transform.position + new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);
        }
    }
    public void DeSelectCard(GameObject card)
    {
        if (GetRelativeCardSize(card) > 1.5f)
        {
            SetRelativeCardSize(card, 1);
            card.transform.position = card.transform.position - new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);
        }
    }
    public void SetRelativeCardSize(GameObject card, float size)
    {
        card.transform.localScale = new Vector3(size * BaseCardSize, size * BaseCardSize, 1);
    }
    public float GetRelativeCardSize(GameObject card)
    {
        return (card.transform.localScale.x/BaseCardSize);
    }
    public void UpdateHand()
    {
        handSize = handContents.Count;
        while (handContents.Count > maxHandSize)
        {
            MoveTo(handContents[handContents.Count - 1], discard);
        }
        SeperateCards(handContents, hand.transform.position, relativeSpaceBetweenCardsInHand * baseCardSize);
    }

    public void SeperateCards(List<GameObject> cards, Vector2 pos, float relativeSpaceBetweenCards)
    {
        float spaceBetweenCards = relativeSpaceBetweenCards * cameraScript.widthHeightRatio;
        //Debug.Log(spaceBetweenCards + " spaceBetweenCards");
        //Debug.Log(cameraScript.widthHeightRatio + " widthHeightRatio");
        int numberOfCards = cards.Count;
        foreach (GameObject card in cards)
        {
            card.SetActive(true);
            card.transform.position = new Vector3((((float)numberOfCards - 1) / 2 - cards.IndexOf(card)) * spaceBetweenCards + pos.x, pos.y, card.transform.position.z);
            if (GetRelativeCardSize(card) > 1.5f)
            {
                card.transform.position = card.transform.position + new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);
            }

        }
    }
    public void DisplayCardsInListByName(string listName, Vector2 pos, int rowLimit, bool randomOrder)
    {
        List<GameObject> list = GetListByName(listName);
        if (list == displayedListName)
        {
            StopDisplayingCardsInList();
        }
        else
        {
            if (displayedListName != null)
            {
                StopDisplayingCardsInList();

            }
            DisplayCardsInList(list, pos, relativeSpaceBetweenCardsInHand, rowLimit, randomOrder);
        }

    }
    public void DisplayCardsInList(List<GameObject> cards, Vector2 pos, float relativeSpaceBetweenCards, int rowLimit, bool randomOrder)
    {
        isDisplayingCards = true;
        listDisplayer.SetActive(true);
        uIManager.IsDisplayingList = true;
        displayedList.Clear();
        displayedListName = cards;
        foreach (GameObject card in cards)
        {
            displayedList.Add(Instantiate(card));
        }
        float horizontalSpaceBetweenCards = relativeSpaceBetweenCards * cameraScript.widthHeightRatio;
        float VerticalSpaceBetweenCards = (relativeSpaceBetweenCards + 0.25f) * cameraScript.widthHeightRatio;
        int numberOfCards = displayedList.Count;
        //Debug.Log(spaceBetweenCards + " spaceBetweenCards");
        //Debug.Log(cameraScript.widthHeightRatio + " widthHeightRatio");
        int rowsCount = Mathf.CeilToInt(displayedList.Count / rowLimit);
        if (randomOrder)
        {
            List<GameObject> cardsInList = new List<GameObject>(displayedList);
            displayedList = new List<GameObject>();
            while (cardsInList.Count > 0)
            {
                GameObject currentCard = cardsInList[Random.Range(0, cardsInList.Count)];
                displayedList.Add(currentCard);
                cardsInList.Remove(currentCard);
            }
        }
        foreach (GameObject card in displayedList)
        {
            card.transform.SetParent(listDisplayer.transform);
            card.SetActive(true);
            int row = Mathf.FloorToInt(displayedList.IndexOf(card) / rowLimit);
            int column = displayedList.IndexOf(card) % rowLimit;
            card.transform.position = new Vector3(pos.x + (column - (rowLimit/2)) * horizontalSpaceBetweenCards, pos.y - row * VerticalSpaceBetweenCards, card.transform.position.z);
        }
    }

    public void ReorderDisplayedCardsInList(List<GameObject> cards, Vector2 pos, float relativeSpaceBetweenCards, int rowLimit, bool randomOrder)
    {

    }

    public void StopDisplayingCardsInList()
    {
        isDisplayingCards = false;
        foreach (GameObject card in displayedList)
        {
            Destroy(card);
        }
        listDisplayer.SetActive(false);
        uIManager.IsDisplayingList = false;
        displayedListName = null;
    }
}
