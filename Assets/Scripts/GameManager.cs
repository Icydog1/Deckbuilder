
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{

    private MapManager mapManager;
    private MouseManager mouseManager;
    private TurnManager turnManager;
    private RoomSpawner roomSpawner;
    private LevelManager levelManager;
    private GameObject pauseScreenBlocker;


    
    private bool nextAction;
    public static event Action<GameManager> GameStartedFunctions;
    public static event Action<GameManager> ResetGame;
    public static event Func<GameManager, IEnumerator> GameStarted;
    public static event Func<GameManager, IEnumerator> LateGameStarted;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");

        

        StartCoroutine(StartGame());

        //GameObject.Find("ListDisplayerScreenBlocker").GetComponent<Image>().enabled = true;
        //GameObject.Find("ListDisplayer").SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator StartGame()
    {
        yield return new WaitForEndOfFrame();

        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        if (GameStartedFunctions != null)
        {
            GameStartedFunctions(this);
        }
        if (GameStarted != null)
        {
            yield return StartCoroutine(GameStarted(this));
        }
        levelManager.StartLevel();
        if (LateGameStarted != null)
        {
            yield return StartCoroutine(LateGameStarted(this));
        }
        //roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }

    public void StepDone()
    {
        nextAction = true;
    }
    //    public IEnumerator ReStartGame()
    public void EndGame()
    {
        pauseScreenBlocker.GetComponent<Image>().enabled = true;
        pauseScreenBlocker.transform.Find("RestartGameButton").gameObject.SetActive(true);

    }

    public IEnumerator ReStartGame()
    {
        pauseScreenBlocker.GetComponent<Image>().enabled = false;
        mouseManager.MouseOffObject(pauseScreenBlocker.transform.Find("RestartGameButton").gameObject);
        pauseScreenBlocker.transform.Find("RestartGameButton").gameObject.SetActive(false);
        Debug.Log("ReStarted Game 1");
        yield return StartCoroutine(levelManager.ResetGame());
        Debug.Log("ReStarted Game 2");
        //yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        if (ResetGame != null)
        {
            ResetGame(this);
        }
        if (GameStartedFunctions != null)
        {
            GameStartedFunctions(this);
        }
        Debug.Log("ReStarted Game 3");

        if (GameStarted != null)
        {
            yield return StartCoroutine(GameStarted(this));
        }
        Debug.Log("ReStarted Game 4");
        levelManager.StartLevel();
        if (LateGameStarted != null)
        {
            yield return StartCoroutine(LateGameStarted(this));
        }
        //roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }
}
