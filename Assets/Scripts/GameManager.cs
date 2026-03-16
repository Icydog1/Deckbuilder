
using System.Collections;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    private MapManager mapManager;
    private MouseManager mouseManager;
    private TurnManager turnManager;
    private RoomSpawner roomSpawner;

    private bool nextAction;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();

        StartCoroutine(StartGame());
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
        roomSpawner.SpawnStartingRoom();
        turnManager.StartTakingTurns();
    }

    public void StepDone()
    {
        nextAction = true;
    }

}
