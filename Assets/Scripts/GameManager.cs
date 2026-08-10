using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //private RefrenceStorage refrenceStorage;

    private MapManager mapManager;
    private MouseManager mouseManager;
    private TurnManager turnManager;
    private RoomSpawner roomSpawner;
    private FloorManager floorManager;
    private GameObject pauseScreenBlocker;
    private GameObject deathScreenBlocker;
    private UIManager UIManager;
    private MainMenuManager mainMenuManager;

    private GameObject settings, restartGameButton, endlessModeButton, settingsRestartGameButton;
    private VariableDisplayer deathText;
    private Character currentCharacter;
    public Character CurrentCharacter { get { return currentCharacter; } set { currentCharacter = value; } }

    //private bool nextAction;
    public static event Action<GameManager> GameStartedFunctions;
    public static event Action<GameManager> ResetGame;
    public static event Func<GameManager, IEnumerator> GameStarted;
    public static event Func<GameManager, IEnumerator> LateGameStarted;

    private bool isInGame;
    public bool IsInGame { get { return isInGame; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //refrenceStorage = GameObject.Find("RefrenceStorage").GetComponent<RefrenceStorage>();
        mapManager = RefrenceStorage.mapManager;
        turnManager = RefrenceStorage.turnManager;
        mouseManager = RefrenceStorage.mouseManager;
        floorManager = RefrenceStorage.floorManager;
        roomSpawner = RefrenceStorage.roomSpawner;
        pauseScreenBlocker = RefrenceStorage.pauseScreenBlocker;
        deathScreenBlocker = RefrenceStorage.deathScreenBlocker;
        UIManager = RefrenceStorage.UIManager;
        mainMenuManager = RefrenceStorage.mainMenuManager;
        //mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        //mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        //turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        //roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        //floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        //pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");
        restartGameButton = deathScreenBlocker.transform.Find("EndGameButton").gameObject;
        endlessModeButton = deathScreenBlocker.transform.Find("EndlessModeButton").gameObject;
        settings = pauseScreenBlocker.transform.Find("Settings").gameObject;
        settingsRestartGameButton = settings.transform.Find("SettingsRestartGameButton").gameObject;
        deathText = deathScreenBlocker.transform.Find("DeathText").GetComponent<VariableDisplayer>();

    }
    //Start the game for the first time
    public IEnumerator StartGame()
    {
        //move out
        isInGame = true;
        yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        if (GameStartedFunctions != null)
        {
            GameStartedFunctions(this);
        }

        if (GameStarted != null)
        {
            Delegate[] listeners = GameStarted.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<GameManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
            //yield return StartCoroutine(GameStarted(this));
        }
        floorManager.StartFloor();
        if (LateGameStarted != null)
        {
            Delegate[] listeners = LateGameStarted.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<GameManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
            //yield return StartCoroutine(LateGameStarted(this));
        }
        //roomSpawner.SpawnStartingRoom();
        yield return StartCoroutine(turnManager.StartTakingTurns());
    }

    //stop the game
    public IEnumerator StopGame()
    {
        OverallStatistics.ResetStatistics();
        UIManager.IsPaused = false;
        UIManager.IsDead = false;
        mainMenuManager.GoToMainMenu();
        mouseManager.MouseOffObject(restartGameButton);
        restartGameButton.SetActive(false);
        endlessModeButton.SetActive(false);
        deathText.Disable();
        mouseManager.MouseOffObject(settingsRestartGameButton);
        settings.SetActive(false);
        yield return StartCoroutine(floorManager.ResetGame());

        //does nothing right now
        if (ResetGame != null)
        {
            ResetGame(this);
        }
        isInGame = false;
    }
    //display restart game button, doesnt stop the game until restart game button is presed
    public void Death()
    {
        UIManager.IsDead = true;
        deathText.DisplayString("Death");
        restartGameButton.SetActive(true);
    }
    //display restart game button and endless mode button
    public void Victory()
    {
        UIManager.IsDead = true;
        deathText.DisplayString("Victory");
        restartGameButton.SetActive(true);
        endlessModeButton.SetActive(true);
    }
    //resumes game after endless mode button is presed
    public IEnumerator ContinueGame()
    {
        UIManager.IsPaused = false;
        UIManager.IsDead = false;
        mouseManager.MouseOffObject(endlessModeButton);
        restartGameButton.SetActive(false);
        endlessModeButton.SetActive(false);
        deathText.Disable();
        yield break;
    }
}
