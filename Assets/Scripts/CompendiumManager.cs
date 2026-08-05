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
    private GameObject compendiumDisplay, characterTabs;
    private DeckManager deckManager;
    private RewardManager rewardManager;
    private UIManager UIManager;

    private bool isShown;
    private bool isDisplayingCards, isDisplayingRelics;
    private List<GameObject> displayedList = new List<GameObject>();
    private string currentTab;
    //private GameObject openedTab;

    void Awake()
    {
        compendiumScreenBlocker = gameObject.GetComponent<Image>();
        //compendium = transform.Find("Compendium").gameObject;
        //compendiumText = compendium.transform.Find("CompendiumText").GetComponent<TextMeshProUGUI>();
        compendiumText = compendium.transform.Find("CompendiumTextDisplayer").transform.Find("Viewport").transform.Find("Content").transform.Find("CompendiumText").GetComponent<TextMeshProUGUI>();

        compendiumDisplay = compendium.transform.Find("CompendiumListDisplayer").gameObject;
        deckManager = RefrenceStorage.deckManager;
        rewardManager = RefrenceStorage.rewardManager;
        UIManager = RefrenceStorage.UIManager;
        characterTabs = compendium.transform.Find("CompendiumCharacterTabs").gameObject;
    }

    public void ShowCompendium()
    {
        isShown = true;
        UIManager.IsInCompendium = true;
        //compendiumScreenBlocker.enabled = true;
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
        else if (isDisplayingRelics)
        {
            StopDisplayingRelicsInList();
        }
        DisplayText("");
        compendium.SetActive(false);
        UIManager.IsInCompendium = false;
        //compendiumScreenBlocker.enabled = false;

    }

    public void OpenTab(string tabName, GameObject tab)
    {
        if (currentTab != tabName)
        {
            currentTab = tabName;
            if (isDisplayingCards)
            {
                StopDisplayingCardsInList();
                if (tabName == "hunter")
                {
                    StartCoroutine(DisplayCards(rewardManager.AllHunterCards));
                }
                else if (tabName == "tinkerer")
                {
                    StartCoroutine(DisplayCards(rewardManager.AllTinkererCards));
                }
                else if (tabName == "wizard")
                {
                    StartCoroutine(DisplayCards(rewardManager.AllWizardCards));
                }
            }
            else if (isDisplayingRelics)
            {
                StopDisplayingRelicsInList();
                if (tabName == "hunter")
                {
                    StartCoroutine(DisplayRelics(rewardManager.AllHunterRelics));
                }
                else if (tabName == "tinkerer")
                {
                    StartCoroutine(DisplayRelics(rewardManager.AllTinkererRelics));
                }
                else if (tabName == "wizard")
                {
                    StartCoroutine(DisplayRelics(rewardManager.AllWizardRelics));
                }
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
            else if (tabName != "hunter" && tabName != "tinkerer" && tabName != "wizard")
            {
                characterTabs.SetActive(false);
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
        if (RefrenceStorage.gameManager.CurrentCharacter == null)
        {
            StartCoroutine(DisplayCards(rewardManager.AllHunterCards));

        }
        else
        {
            StartCoroutine(DisplayCards(rewardManager.AllCardRewards));

        }

    }
    public void OpenRelicTab()
    {
        if (RefrenceStorage.gameManager.CurrentCharacter == null)
        {
            StartCoroutine(DisplayRelics(rewardManager.AllHunterRelics));

        }
        else
        {
            StartCoroutine(DisplayRelics(rewardManager.AllRelicsRewards));
        }

    }
    public IEnumerator DisplayCards(List<GameObject> displayedObjects, int rowLimit = 5)
    {
        isDisplayingCards = true;
        deckManager.IsDisplayingList = true;

        characterTabs.SetActive(true);
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
        int numberOfCards = displayedList.Count;
        layout.constraintCount = rowLimit;
        layout.cellSize = new Vector2(150, 210);
        layout.spacing = new Vector2(0, 0);

        for (int i = 0; i < displayedList.Count; i++)
        {
            GameObject card = displayedList[i];
            card.SetActive(true);
            card.transform.localScale = Vector3.zero;
            card.transform.SetParent(storeTo);
            yield return StartCoroutine(card.GetComponent<Card>().PrepareCardDiscription(true));
            if (isDisplayingCards)
            {
                deckManager.SetRelativeCardSize(card, 1);
            }
        }
        RefrenceStorage.playerControler.UnmodifiedAction = false;
    }
    public IEnumerator DisplayRelics(List<GameObject> displayedObjects, int rowLimit = 7)
    {
        isDisplayingRelics = true;
        deckManager.IsDisplayingList = true;

        characterTabs.SetActive(true);
        compendiumDisplay.SetActive(true);
        compendiumDisplay.GetComponent<RectTransform>().sizeDelta = new Vector2(compendiumDisplay.GetComponent<RectTransform>().sizeDelta.x, compendiumDisplay.transform.parent.parent.GetComponent<RectTransform>().sizeDelta.y - 100);
        Transform storeTo = compendiumDisplay.transform.Find("Viewport").transform.Find("Content");
        GridLayoutGroup layout = storeTo.GetComponent<GridLayoutGroup>();

        //uIManager.IsDisplayingList = true;
        displayedList.Clear();
        //displayedListName = cards;
        foreach (GameObject relic in displayedObjects)
        {
            GameObject newRelic;
            displayedList.Add(newRelic = Instantiate(relic));
            //newCard.GetComponent<Card>().OriginalCard = card;
        }
        //float horizontalSpaceBetweenCards = relativeSpaceBetweenCards * RefrenceStorage.cameraScript.widthHeightRatio;
        //float VerticalSpaceBetweenCards = (relativeSpaceBetweenCards + 0.1f) * RefrenceStorage.cameraScript.widthHeightRatio;
        int numberOfRelics = displayedList.Count;
        //Debug.Log(spaceBetweenCards + " spaceBetweenCards");
        //Debug.Log(cameraScript.widthHeightRatio + " widthHeightRatio");
        //int rowsCount = Mathf.CeilToInt(displayedList.Count / rowLimit);
        layout.constraintCount = rowLimit;
        layout.cellSize = new Vector2(100, 100);
        layout.spacing = new Vector2(5, 5);
        foreach (GameObject relic in displayedList)
        {
            relic.SetActive(true);
            relic.transform.SetParent(storeTo);
            relic.transform.localScale = Vector3.one * 2;
        }
        yield break;
    }

    public void StopDisplayingCardsInList()
    {
        isDisplayingCards = false;
        deckManager.IsDisplayingList = false;
        foreach (GameObject card in displayedList)
        {
            card.GetComponent<Card>().AttemptToDestroy();
        }
        displayedList.Clear();
        compendiumDisplay.SetActive(false);
    }
    public void StopDisplayingRelicsInList()
    {
        isDisplayingRelics = false;
        deckManager.IsDisplayingList = false;
        foreach (GameObject relic in displayedList)
        {
            Destroy(relic);
        }
        displayedList.Clear();
        compendiumDisplay.SetActive(false);
    }
}
