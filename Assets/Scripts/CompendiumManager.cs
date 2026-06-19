using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompendiumManager : MonoBehaviour
{
    private Image compendiumScreenBlocker;
    [SerializeField]
    private GameObject compendium;
    private TextMeshProUGUI compendiumText;
    private GameObject compendiumDisplay;
    private DeckManager deckManager;

    private bool isShown;
    private bool isDisplayingCards;
    private List<GameObject> displayedList = new List<GameObject>();
    private string currentTab;
    //private GameObject openedTab;

    void Awake()
    {
        compendiumScreenBlocker = gameObject.GetComponent<Image>();
        //compendium = transform.Find("Compendium").gameObject;
        compendiumText = compendium.transform.Find("CompendiumText").GetComponent<TextMeshProUGUI>();
        compendiumDisplay = compendium.transform.Find("CompendiumListDisplayer").gameObject;
        deckManager = RefrenceStorage.deckManager;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowCompendium()
    {
        isShown = true;
        compendiumScreenBlocker.enabled = true;
        compendium.SetActive(true);
        currentTab = "card";
        OpenCardTab();
    }
    public void HideCompendium()
    {
        isShown = false;
        if (isDisplayingCards)
        {
            StopDisplayingCardsInList();
        }
        DisplayText("");
        compendium.SetActive(false);
        compendiumScreenBlocker.enabled = false;
    }

    public void OpenTab(string tabName, GameObject tab)
    {
        if (currentTab != tabName)
        {
            currentTab = tabName;
            if (isDisplayingCards)
            {
                StopDisplayingCardsInList();
            }
            DisplayText("");
            if (tabName == "card")
            {
                OpenCardTab();
            }
            else if (tabName == "relic")
            {
                OpenRelicTab();
            }
            else
            {
                DisplayText(tab.GetComponent<Text>().text);
            }
        }
    }
    public void DisplayText(string text)
    {
        compendiumText.SetText(text);
    }

    public void OpenCardTab()
    {
        StartCoroutine(DisplayInList(RefrenceStorage.rewardManager.AllCards, "card", 5));
    }
    public void OpenRelicTab()
    {
        StartCoroutine(DisplayInList(RefrenceStorage.rewardManager.AllRelics, "relic", 7));
    }
    public IEnumerator DisplayInList(List<GameObject> displayedObjects, string type, int rowLimit = 5)
    {
        isDisplayingCards = true;
        deckManager.IsDisplayingCards = true;
        compendiumDisplay.SetActive(true);
        compendiumDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(compendiumDisplay.GetComponent<RectTransform>().sizeDelta.x, compendiumDisplay.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y - 100);
        Transform storeTo = compendiumDisplay.transform.Find("Viewport").transform.Find("Content");
        GridLayoutGroup layout = storeTo.GetComponent<GridLayoutGroup>();

        //uIManager.IsDisplayingList = true;
        displayedList.Clear();
        //displayedListName = cards;
        foreach (GameObject card in displayedObjects)
        {
            GameObject newCard;
            displayedList.Add(newCard = Instantiate(card));
            //newCard.GetComponent<Card>().OriginalCard = card;
        }
        //float horizontalSpaceBetweenCards = relativeSpaceBetweenCards * RefrenceStorage.cameraScript.widthHeightRatio;
        //float VerticalSpaceBetweenCards = (relativeSpaceBetweenCards + 0.1f) * RefrenceStorage.cameraScript.widthHeightRatio;
        int numberOfCards = displayedList.Count;
        //Debug.Log(spaceBetweenCards + " spaceBetweenCards");
        //Debug.Log(cameraScript.widthHeightRatio + " widthHeightRatio");
        //int rowsCount = Mathf.CeilToInt(displayedList.Count / rowLimit);
        layout.constraintCount = rowLimit;
        if (type == "card")
        {
            layout.cellSize = new Vector2(150, 210);
            layout.spacing = new Vector2(0, 0);
            foreach (GameObject card in displayedList)
            {
                card.SetActive(true);
                card.transform.localScale = Vector3.zero;
                card.transform.SetParent(storeTo);
                yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription(true));
                RefrenceStorage.deckManager.SetRelativeCardSize(card, 1);
            }
            RefrenceStorage.playerControler.UnmodifiedAction = false;
        }
        else if (type == "relic")
        {
            layout.cellSize = new Vector2(100, 100);
            layout.spacing = new Vector2(5, 5);
            foreach (GameObject card in displayedList)
            {
                card.SetActive(true);
                card.transform.SetParent(storeTo);
                card.transform.localScale = Vector3.one * 2;
            }
        }

    }
    public void StopDisplayingCardsInList()
    {
        isDisplayingCards = false;
        deckManager.IsDisplayingCards = false;
        foreach (GameObject card in displayedList)
        {
            Destroy(card);
        }
        displayedList.Clear();
        compendiumDisplay.SetActive(false);
        //uIManager.IsDisplayingList = false;
        //displayedListName = null;
    }
}
