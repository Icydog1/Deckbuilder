
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

    private bool nextAction;
    public static event Action<GameManager> GameStarted;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();


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
        roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }

    public void StepDone()
    {
        nextAction = true;
    }

}
