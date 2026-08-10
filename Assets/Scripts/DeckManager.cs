using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Lootable;

public class DeckManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField]
    private GameObject hand, deck, discard, play, exhaust;
    public GameObject Hand { get { return hand; } }
    public GameObject Deck { get { return deck; } }
    public GameObject Discard { get { return discard; } }

    [SerializeField]
    private List<GameObject> additionalCardsInStartingDeck = new List<GameObject>();

    private List<GameObject> startingDeck;

    public List<GameObject> entireDeck = new List<GameObject>();
    public List<GameObject> deckContents = new List<GameObject>(), handContents = new List<GameObject>(), discardContents = new List<GameObject>(), playContents = new List<GameObject>(), exhaustContents = new List<GameObject>();
    public List<GameObject> DeckContents {  get { return deckContents; } }
    public List<GameObject> DiscardContents { get { return discardContents; } }
    public List<GameObject> HandContents { get { return handContents; } }
    public List<GameObject> ExhaustContents { get { return exhaustContents; } }

    public List<GameObject> EntireDeck { get { return entireDeck; } }

    private List<GameObject> displayedList = new List<GameObject>();
    private List<GameObject> displayedListName;
    private Dictionary<string,List<GameObject>> posibleCardLocations = new Dictionary<string,List<GameObject>>();
    //public Dictionary<string, List<GameObject>> PosibleCardLocations { get { return posibleCardLocations; } }
    private VariableDisplayer cardsInDeckDisplay, cardsInDiscardDisplay, cardsInEntireDeckDisplay, cardsInExhaustDisplay;
    private float relativeSpaceBetweenCardsInHand = 0.35f;
    //if change set hand positon
    private float baseCardSize = 0.9f;
    public float BaseCardSize { get { return baseCardSize; } }

    private float selectedCardHeightIncrease = 100f;

    private int handSize;
    private int maxHandSize = 10;

    private int startHandSize = 5;
    private CameraScript cameraScript;
    private MouseManager mouseManager;
    [SerializeField]
    private GameObject listDisplayer;
    private UIManager uIManager;
    private PlayerControler playerControler;

    private bool isDisplayingList, isChoosingCard;
    public bool IsDisplayingList { get { return isDisplayingList; } set { isDisplayingList = value; } }
    private GameObject selectedCard;
    public GameObject SelectedCard { get { return selectedCard; } set { selectedCard = value; } }


    public bool IsChoosingCard { get { return isChoosingCard; } set { isChoosingCard = value; } }



    //public DiscardScript discardScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        cardsInDeckDisplay = GameObject.Find("CardsInDeckDisplay").GetComponent<VariableDisplayer>();
        cardsInDiscardDisplay = GameObject.Find("CardsInDiscardDisplay").GetComponent<VariableDisplayer>();
        cardsInEntireDeckDisplay = GameObject.Find("CardsInEntireDeckDisplay").GetComponent<VariableDisplayer>();
        cardsInExhaustDisplay = GameObject.Find("CardsInExhaustDisplay").GetComponent<VariableDisplayer>();
        //listDisplayer = GameObject.Find("ListDisplayer");
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

        //Debug.Log(deck.transform.childCount);
        //if (startingDeck.Count == 0)
        //{
        //    for (int i = 0; i < deck.transform.childCount; i++)
        //    {
        //        startingDeck.Add(deck.transform.GetChild(i).gameObject);
        //        //Debug.Log(deck.transform.GetChild(i).gameObject);
        //    }
        //}
        //deckContents = new List<GameObject>(startingDeck);
        //entireDeck = new List<GameObject>(startingDeck);

        posibleCardLocations.Add("deck", deckContents);
        posibleCardLocations.Add("hand", handContents);
        posibleCardLocations.Add("discard", discardContents);
        posibleCardLocations.Add("play", playContents);
        posibleCardLocations.Add("exhaust", exhaustContents);

        GameManager.ResetGame += ResetDeck;
        GameManager.GameStartedFunctions += SpawnStartingDeck;
        //GameManager.GameStartedFunctions += DrawStartingHand;
    }

    // Update is called once per frame
    void Update()
    {


    }

    public IEnumerator PlayCard(GameObject card)
    {
        yield return StartCoroutine(MoveTo(card, play));
    }
    public void ResetDeck(GameManager gameManager)
    {
        foreach (GameObject card in entireDeck)
        {
            card.GetComponent<Card>().AttemptToDestroy();
        }
        deckContents.Clear();
        handContents.Clear();
        discardContents.Clear();
        playContents.Clear();
        exhaustContents.Clear();
        entireDeck.Clear();
    }
    public void SpawnStartingDeck(GameManager gameManager)
    {
        startingDeck = new List<GameObject>(gameManager.CurrentCharacter.StartingDeck);
        startingDeck.AddRange(additionalCardsInStartingDeck);
        foreach (GameObject card in startingDeck)
        {
            GameObject newCard = Instantiate(card);
            entireDeck.Add(newCard);
            deckContents.Add(newCard);
            newCard.transform.SetParent(deck.transform);
            newCard.GetComponent<Card>().AttemptToDisable();
        }
        cardsInEntireDeckDisplay.DisplayVariable(entireDeck.Count);
        Suffle(ref deckContents);
    }
    public IEnumerator DiscardHand()
    {
        List<GameObject> discardedCards = new List<GameObject>(handContents);
        foreach (GameObject card in discardedCards)
        {
            yield return StartCoroutine(DiscardCard(card));
        }
    }

    public IEnumerator DrawNewHand(int handSize)
    {
        //int cardsInHand = handSize;
        //for (int i = 0; i < cardsInHand; i++)
        //{
        //    DiscardFirstCard();
        //    //Debug.Log("card Discarded");
        //}
        yield return StartCoroutine(DrawCards(handSize));
    }

    public IEnumerator GainCard(GameObject card)
    {
        entireDeck.Add(card);
        cardsInEntireDeckDisplay.DisplayVariable(entireDeck.Count);
        yield return StartCoroutine(MoveTo(card, deck, UnityEngine.Random.Range(0, deckContents.Count + 1)));
    }
    public IEnumerator DestroyCard(GameObject card)
    {
        Card cardScript = card.GetComponent<Card>();
        if (cardScript.OriginalCard != null)
        {
            displayedList.Remove(card);
            yield return StartCoroutine(DestroyCard(cardScript.OriginalCard));
        }
        //yield return StartCoroutine(MoveTo(card, deck, UnityEngine.Random.Range(0, deckContents.Count + 1)));
        //removes the card from list it is in and does stuff to make sure that the thing updates properly
        if (deckContents.Contains(card))
        {
            deckContents.Remove(card);
            cardsInDeckDisplay.DisplayVariable(deckContents.Count);
        }
        if (handContents.Contains(card))
        {
            handContents.Remove(card);
            yield return StartCoroutine(UpdateHand());
        }
        if (discardContents.Contains(card))
        {
            discardContents.Remove(card);
            cardsInDiscardDisplay.DisplayVariable(discardContents.Count);
        }
        if (playContents.Contains(card))
        {
            playContents.Remove(card);
            playerControler.PlayedCardScript.StopPlaying = true;
            playerControler.ForceEndAction();
        }
        if (exhaustContents.Contains(card))
        {
            exhaustContents.Remove(card);
            cardsInExhaustDisplay.DisplayVariable(exhaustContents.Count);
            //add more exaust stuff
        }
        entireDeck.Remove(card);
        cardsInEntireDeckDisplay.DisplayVariable(entireDeck.Count);
        cardScript.AttemptToDestroy();
    }
    public IEnumerator DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return StartCoroutine(DrawCard());
        }
        yield return null;
    }

    public IEnumerator DrawCard()
    {
        if (deckContents.Count == 0)
        {
            if (discardContents.Count == 0)
            {
                yield break;
            }
            yield return StartCoroutine(ReSuffle());
        }
        GameObject currentCard = deckContents[0];
        yield return StartCoroutine(MoveTo(currentCard, hand));
    }
    public IEnumerator ReSuffle()
    {
        /*
        int discardSize = discardContents.Count;
        for (int i = 0; i < discardSize; i++)
        {
            GameObject currentCard = discardContents[Random.Range(0, discardContents.Count)];
            MoveTo(currentCard, deck);
        }
        */
        OverallStatistics.shuffles++;
        while (discardContents.Count > 0)
        {
            yield return StartCoroutine(MoveTo(discardContents[0], deck));
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
            GameObject currentCard = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            list.Add(currentCard);
            tempList.Remove(currentCard);
        }
    }


    public IEnumerator MoveTo(GameObject card, GameObject location, int newIndex = Var.nullValue)
    {
        GameObject cardLocation = card.transform.parent.gameObject;
        string locationName = cardLocation.name.ToLower();
        if (posibleCardLocations.ContainsKey(locationName))
        {
            posibleCardLocations[locationName].Remove(card);
        }
        //foreach (KeyValuePair<string, List<GameObject>> posibleLocation in posibleCardLocations)
        //{
        //    if (posibleLocation.Contains(card))
        //    {
        //        posibleLocation.Remove(card);
        //    }
        //}
        DeSelectCard(card);
        if (newIndex != Var.nullValue)
        {
            //GetListByName(location.name.ToLower() + "Contents").Insert(newIndex, card);
            posibleCardLocations[location.name.ToLower()].Insert(newIndex, card);
        }
        else
        {
            posibleCardLocations[location.name.ToLower()].Add(card);

            //GetListByName(location.name.ToLower() + "Contents").Add(card);
        }
        card.transform.position = location.transform.position;
        Card cardScript = card.GetComponent<Card>();
        if (location == hand)
        {
            card.gameObject.SetActive(true);
            yield return StartCoroutine(cardScript.PrepareCardDiscription());
        }
        if (location == play)
        {
            card.gameObject.SetActive(true);
            //Debug.Log("updating card description");
            yield return StartCoroutine(cardScript.PrepareCardDiscription());
            SelectCard(card);
        }
        else
        {
            cardScript.AttemptToDisable();
            //card.gameObject.SetActive(false);
        }
        card.transform.SetParent(location.transform);
        mouseManager.MouseOffObject(card);
        cardsInDeckDisplay.DisplayVariable(deckContents.Count);
        cardsInDiscardDisplay.DisplayVariable(discardContents.Count);
        cardsInExhaustDisplay.DisplayVariable(exhaustContents.Count);

        yield return StartCoroutine(UpdateHand());
        
    }
    public IEnumerator ExhaustUntil(GameObject card, Func<bool> conditiion, GameObject returnToList)
    {
        //Debug.Log("exhausing");
        yield return StartCoroutine(MoveTo(card, exhaust));
        //Debug.Log("done exhausing");
        StartCoroutine(ExhaustWaitUntil(card, conditiion, returnToList));
    }
    public IEnumerator ExhaustWaitUntil(GameObject card, Func<bool> conditiion, GameObject returnToList)
    {
        //Debug.Log("waiting");
        yield return new WaitUntil(conditiion);
        //Debug.Log("done");
        if (card != null)
        {
            int index = Var.nullValue;
            if (returnToList == deck)
            {
                index = UnityEngine.Random.Range(0, deckContents.Count - 1);
            }
            yield return StartCoroutine(MoveTo(card, returnToList, index));
        }
    }
    public IEnumerator returnFromExhaust(GameObject card, string returnToList)
    {
        int index = Var.nullValue;
        if (returnToList == "deck")
        {
            index = UnityEngine.Random.Range(0, deckContents.Count);
        }
        yield return StartCoroutine(MoveTo(card, (GameObject)GetType().GetField(returnToList).GetValue(this), index));

    }
    public List<GameObject> GetListByName(string listName)
    {
        //Debug.Log(listName);
        return (List<GameObject>)GetType().GetField(listName).GetValue(this);
    }
    //public void DiscardFirstCard()
    //{
    //    GameObject firstCard = handContents[0];
    //    DiscardCard(firstCard);
    //}
    public IEnumerator DiscardCard(GameObject currentCard)
    {
        yield return StartCoroutine(MoveTo(currentCard, discard));

    }
    public void SelectCard(GameObject card)
    {
        if (GetRelativeCardSize(card) < 1.5f)
        {
            Canvas canvas = card.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "SelectedCard";
            card.AddComponent<GraphicRaycaster>();
            //card.transform.SetAsLastSibling();
            SetRelativeCardSize(card, 2);
            //card.transform.position = card.transform.position + new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);
            card.GetComponent<RectTransform>().anchoredPosition = card.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, selectedCardHeightIncrease);
            //Debug.Log("selected " + card);
        }
    }
    public void DeSelectCard(GameObject card)
    {

        Canvas canvas = card.GetComponent<Canvas>();
        GraphicRaycaster raycaster = card.GetComponent<GraphicRaycaster>();

        if (canvas != null)
        {
            Destroy(raycaster);            
            Destroy(canvas);
            //Debug.Log("deselected " + card);

        }
        //Debug.Log(GetRelativeCardSize(card));
        if (GetRelativeCardSize(card) > 1.5f && !playContents.Contains(card))
        {
            //Debug.Log("deselected " + card);
            SetRelativeCardSize(card, 1);
            //card.transform.position = card.transform.position - new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);
            if (!isDisplayingList)
            {
                card.GetComponent<RectTransform>().anchoredPosition = card.GetComponent<RectTransform>().anchoredPosition - new Vector2(0, selectedCardHeightIncrease);
            }


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
    public IEnumerator UpdateHand()
    {
        handSize = handContents.Count;
        while (handContents.Count > maxHandSize)
        {
            yield return StartCoroutine(MoveTo(handContents[handContents.Count - 1], discard));
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
                //card.transform.position = card.transform.position + new Vector3(0, selectedCardHeightIncrease * baseCardSize * cameraScript.zoom, 0);

                card.GetComponent<RectTransform>().anchoredPosition = card.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, selectedCardHeightIncrease);

            }

        }
    }
    public void DisplayCardsInListByName(string listName, Vector2 pos, int rowLimit = 5, bool randomOrder = true)
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
            StartCoroutine(DisplayCardsInList(list, listDisplayer, pos, relativeSpaceBetweenCardsInHand, rowLimit, randomOrder));
        }

    }
    public IEnumerator DisplayCardsInList(List<GameObject> cards, GameObject display, Vector2 pos, float relativeSpaceBetweenCards, int rowLimit = 5, bool randomOrder = true)
    {
        isDisplayingList = true;
        display.SetActive(true);
        display.GetComponent<RectTransform>().sizeDelta = new Vector2(display.GetComponent<RectTransform>().sizeDelta.x, display.transform.parent.GetComponent<RectTransform>().sizeDelta.y-100);
        Transform storeTo = display.transform.Find("Viewport").transform.Find("Content");
        uIManager.IsDisplayingList = true;
        displayedList.Clear();
        displayedListName = cards;
        foreach (GameObject card in cards)
        {
            GameObject newCard;
            displayedList.Add(newCard = Instantiate(card));
            newCard.GetComponent<Card>().OriginalCard = card;

        }
        float horizontalSpaceBetweenCards = relativeSpaceBetweenCards * cameraScript.widthHeightRatio;
        float VerticalSpaceBetweenCards = (relativeSpaceBetweenCards + 0.1f) * cameraScript.widthHeightRatio;
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
                GameObject currentCard = cardsInList[UnityEngine.Random.Range(0, cardsInList.Count)];
                displayedList.Add(currentCard);
                cardsInList.Remove(currentCard);
            }
        }
        for (int i = 0; i < displayedList.Count; i++)
        {
            GameObject card = displayedList[i];
            card.SetActive(true);
            card.transform.localScale = Vector3.zero;
            card.transform.SetParent(storeTo);
            yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription());
            SetRelativeCardSize(card, 1);
            //int row = Mathf.FloorToInt(displayedList.IndexOf(card) / rowLimit);
            //int column = displayedList.IndexOf(card) % rowLimit;
            //card.transform.position = new Vector3(hexPos.x + (column - (rowLimit/2)) * horizontalSpaceBetweenCards, hexPos.y - row * VerticalSpaceBetweenCards, card.transform.position.z);
        }
    }

    //public IEnumerator UpdateCardsDisplay()
    //{
    //    foreach (GameObject card in displayedList)
    //    {
    //        yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription());
    //    }
    //    foreach (GameObject card in handContents)
    //    {
    //        yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription());
    //    }
    //    //Debug.Log("Prepeared Hand");
    //    foreach (GameObject card in playContents)
    //    {
    //        yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription());
    //    }
    //}
    public IEnumerator UpdateCardsDisplay(string modifiedAction = "All")
    {
        List<Card> cards = new List<Card>();
        foreach (GameObject card in displayedList)
        {
            cards.Add(card.GetComponent<Card>());
        }
        foreach (GameObject card in handContents)
        {
            cards.Add(card.GetComponent<Card>());
        }
        foreach (GameObject card in playContents)
        {
            cards.Add(card.GetComponent<Card>());
        }
        if (modifiedAction == "All")
        {
            foreach (Card card in cards)
            {
                yield return StartCoroutine(card.PrepareCardDiscription());
            }
        }
        else
        {
            foreach (Card card in cards)
            {
                yield return StartCoroutine(card.UpdateCardDiscription(modifiedAction));
            }
        }

    }
    public void StopDisplayingCardsInList()
    {
        isDisplayingList = false;
        foreach (GameObject card in displayedList)
        {
            card.GetComponent<Card>().AttemptToDestroy();
        }
        displayedList.Clear();
        listDisplayer.SetActive(false);
        uIManager.IsDisplayingList = false;
        displayedListName = null;
    }

    public IEnumerator ChooseCard(List<GameObject> cardOptions, Func<GameObject,IEnumerator> chosenCard)
    {
        if (uIManager.IsDisplayingList == true)
        {
            StopDisplayingCardsInList();
        }
        yield return StartCoroutine(DisplayCardsInList(cardOptions, listDisplayer, listDisplayer.transform.position, relativeSpaceBetweenCardsInHand, 5, false));
        isChoosingCard = true;
        selectedCard = null;
        yield return new WaitUntil(() => selectedCard != null);
        yield return StartCoroutine(chosenCard(selectedCard.GetComponent<Card>().OriginalCard));
        selectedCard = null;
        StopDisplayingCardsInList();
    }
}
