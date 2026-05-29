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
    private LevelManager levelManager;
    private GameObject pauseScreenBlocker;
    private OverallStatistics overallStatistics;



    private bool nextAction;
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
        levelManager = RefrenceStorage.levelManager;
        roomSpawner = RefrenceStorage.roomSpawner;
        pauseScreenBlocker = RefrenceStorage.pauseScreenBlocker;
        overallStatistics = RefrenceStorage.overallStatistics;

        //mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        //mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        //turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        //roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        //levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        //pauseScreenBlocker = GameObject.Find("PauseScreenBlocker");
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
        levelManager.StartLevel();
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

    public void StepDone()
    {
        nextAction = true;
    }
    //    public IEnumerator ReStartGame()
    public void EndGame()
    {
        pauseScreenBlocker.GetComponent<Image>().enabled = true;
        pauseScreenBlocker.GetComponent<RectTransform>().sizeDelta = pauseScreenBlocker.transform.parent.GetComponent<RectTransform>().sizeDelta;

        pauseScreenBlocker.transform.Find("RestartGameButton").gameObject.SetActive(true);

    }
    public IEnumerator ReStartGame()
    {
        pauseScreenBlocker.GetComponent<Image>().enabled = false;
        mouseManager.MouseOffObject(pauseScreenBlocker.transform.Find("RestartGameButton").gameObject);
        pauseScreenBlocker.transform.Find("RestartGameButton").gameObject.SetActive(false);
        //Debug.Log("ReStarted Game 1");
        yield return StartCoroutine(levelManager.ResetGame());

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
