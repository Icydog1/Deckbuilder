
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

    
    private bool nextAction;
    public static event Action<GameManager> GameStarted;
    public static event Action<GameManager> ResetGame;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();


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
        if (GameStarted != null)
        {
            GameStarted(this);
        }
        levelManager.StartLevel();
        //roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }

    public void StepDone()
    {
        nextAction = true;
    }
//    public IEnumerator ReStartGame()

    public void ReStartGame()
    {
        levelManager.ClearLevel();
        //yield return new WaitForEndOfFrame();
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        if (ResetGame != null)
        {
            ResetGame(this);
        }
        if (GameStarted != null)
        {
            GameStarted(this);
        }
        levelManager.StartLevel();

        //roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }
}
