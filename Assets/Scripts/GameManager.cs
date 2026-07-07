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

    
    private GameObject settings, restartGameButton, settingsRestartGameButton;

    //private bool nextAction;
    public static event Action<GameManager> GameStartedFunctions;
    public static event Action<GameManager> ResetGame;
    public static event Func<GameManager, IEnumerator> GameStarted;
    public static event Func<GameManager, IEnumerator> LateGameStarted;


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
        //mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        //mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        //turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        //roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        //floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        //pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");
        restartGameButton = deathScreenBlocker.transform.Find("RestartGameButton").gameObject;
        settings = pauseScreenBlocker.transform.Find("Settings").gameObject;
        settingsRestartGameButton = settings.transform.Find("SettingsRestartGameButton").gameObject;


    }

    void Start()
    {


        StartCoroutine(StartGame());

        //GameObject.Find("ListDisplayerScreenBlocker").GetComponent<Image>().enabled = true;
        //GameObject.Find("ListDisplayer").SetActive(false);
    }

    private IEnumerator StartGame()
    {
        //move out
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
    public void EndGame()
    {
        UIManager.IsDead = true;
        //deathScreenBlocker.GetComponent<Image>().enabled = true;
        //deathScreenBlocker.GetComponent<RectTransform>().sizeDelta = deathScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;

        restartGameButton.SetActive(true);

    }
    public IEnumerator ReStartGame()
    {
        OverallStatistics.ResetStatistics();
        //pauseScreenBlocker.GetComponent<Image>().enabled = false;
        UIManager.IsPaused = false;
        UIManager.IsDead = false;
        //deathScreenBlocker.GetComponent<Image>().enabled = false;
        mouseManager.MouseOffObject(restartGameButton);
        restartGameButton.gameObject.SetActive(false);
        mouseManager.MouseOffObject(settingsRestartGameButton);
        settings.SetActive(false);
        //Debug.Log("ReStarted Game 1");
        yield return StartCoroutine(floorManager.ResetGame());

        //Debug.Log("ReStarted Game 2");

        //yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        if (ResetGame != null)
        {
            ResetGame(this);
        }
        StartCoroutine(StartGame());
    }

}
