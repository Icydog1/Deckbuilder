using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private GameObject mainMenu;
    private DeckManager deckManager;
    private UIManager UIManager;
    private GameObject characterSelectScreenBlocker;
    private GameObject characterSelect;
    private GameManager gameManager;

    [SerializeField]
    private SelectCharacter baseCharacterButton;
    private SelectCharacter selectedCharacterButton;

    private bool isShown;
    public bool IsShown {  get { return isShown; } }


    void Awake()
    {
        //mainMenuScreenBlocker = gameObject.GetComponent<Image>();
        mainMenu = RefrenceStorage.mainMenu;
        //mainMenuText = mainMenu.transform.Find("MainMenuText").GetComponent<TextMeshProUGUI>();
        //mainMenuDisplay = mainMenu.transform.Find("MainMenuListDisplayer").gameObject;
        deckManager = RefrenceStorage.deckManager;
        UIManager = RefrenceStorage.UIManager;
        gameManager = RefrenceStorage.gameManager;
        characterSelectScreenBlocker = RefrenceStorage.characterSelectScreenBlocker;
        characterSelect = characterSelectScreenBlocker.transform.Find("CharacterSelect").gameObject;
    }
    private void Start()
    {
        GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        isShown = true;
        mainMenu.SetActive(true);
        UIManager.IsOnMainMenu = true;
    }
    public void CharacterSelect()
    {
        if (!gameManager.IsInGame)
        {
            characterSelect.SetActive(true);
            UIManager.IsSelectingCharacter = true;
            SelectCharacter(baseCharacterButton);
            selectedCharacterButton.transform.Find("Image").GetComponent<Image>().color = Color.red;
        }
    }
    public void SelectCharacter(SelectCharacter button)
    {
        if (!gameManager.IsInGame)
        {
            if (selectedCharacterButton != null)
            {
                selectedCharacterButton.transform.Find("Image").GetComponent<Image>().color = selectedCharacterButton.BackupBaseColor;
            }
            selectedCharacterButton = button;
            selectedCharacterButton.BaseColor = Color.red;
            gameManager.CurrentCharacter = button.SelectedCharacter;
        }

    }
    public void StartGame()
    {
        if (!gameManager.IsInGame)
        {
            LeaveMainMenu();
            StartCoroutine(gameManager.StartGame());
        }


    }
    public void LeaveMainMenu()
    {
        isShown = false;
        mainMenu.SetActive(false);
        characterSelect.SetActive(false);
        UIManager.IsSelectingCharacter = false;
        UIManager.IsOnMainMenu = false;
        //if (isDisplayingCards)
        //{
        //    StopDisplayingCardsInList();
        //}
        //else if (isDisplayingRelics)
        //{
        //    StopDisplayingRelicsInList();
        //}
        //DisplayVariable("");
        //mainMenu.SetActive(false);
        //mainMenuScreenBlocker.enabled = false;

    }
}
