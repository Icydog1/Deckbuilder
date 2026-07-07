using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    //private Image mainMenuScreenBlocker;
    //[SerializeField]
    //private GameObject mainMenu;
    //private TextMeshProUGUI mainMenuText;
    //private GameObject mainMenuDisplay;
    //private DeckManager deckManager;
    //private UIManager UIManager;

    //private bool isShown;
    //private bool isDisplayingCards, isDisplayingRelics;
    //private List<GameObject> displayedList = new List<GameObject>();
    //private string currentTab;
    //private GameObject openedTab;

    void Awake()
    {
        //mainMenuScreenBlocker = gameObject.GetComponent<Image>();
        ////mainMenu = transform.Find("MainMenu").gameObject;
        //mainMenuText = mainMenu.transform.Find("MainMenuText").GetComponent<TextMeshProUGUI>();
        //mainMenuDisplay = mainMenu.transform.Find("MainMenuListDisplayer").gameObject;
        //deckManager = RefrenceStorage.deckManager;
        //UIManager = RefrenceStorage.UIManager;
    }

    public void ShowMainMenu()
    {
        //isShown = true;
        //UIManager.IsOnMainMenu = true;
        ////mainMenuScreenBlocker.enabled = true;
        //mainMenu.SetActive(true);
        //currentTab = "card";
    }
    public void HideMainMenu()
    {
        //isShown = false;
        //if (isDisplayingCards)
        //{
        //    StopDisplayingCardsInList();
        //}
        //else if (isDisplayingRelics)
        //{
        //    StopDisplayingRelicsInList();
        //}
        //DisplayText("");
        //mainMenu.SetActive(false);
        //UIManager.IsOnMainMenu = false;
        ////mainMenuScreenBlocker.enabled = false;

    }

    public void OpenTab(string tabName, GameObject tab)
    {
        //if (currentTab != tabName)
        //{
        //    currentTab = tabName;
        //    if (isDisplayingCards)
        //    {
        //        StopDisplayingCardsInList();
        //    }
        //    else if (isDisplayingRelics)
        //    {
        //        StopDisplayingRelicsInList();
        //    }
        //    DisplayText("");
        //    if (tabName == "card")
        //    {
        //        OpenCardTab();
        //    }
        //    else if (tabName == "relic")
        //    {
        //        OpenRelicTab();
        //    }
        //    else
        //    {
        //        DisplayText(tab.GetComponent<Text>().text);
        //    }
        //}
    }
}
