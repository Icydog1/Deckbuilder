using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //public List<Enemy> EnemiesScripts = new List<Enemy>();
    //private List<GameObject> enemies = new List<GameObject>();
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameObject player;
    private GameObject newRoundMarker;
    private GameObject currentTurn;
    private Enemy currentEnemyTurnScript;
    private List<GameObject> turnOrder = new List<GameObject>();
    public List<GameObject> TurnOrder { get { return turnOrder; } }
    //private bool endOfRound, playerTurn, enemyTurn;
    private bool takingTurns;

    private LevelManager levelManager;


    public static event Action<TurnManager> RoundEndedFunctions;
    public static event Action<TurnManager> RoundStartedFunctions;

    public static event Func<TurnManager,IEnumerator> RoundEnded;
    public static event Func<TurnManager, IEnumerator> RoundStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player");
        playerControler = player.GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        newRoundMarker = GameObject.Find("NewRoundMarker");
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        LevelManager.LevelCleared += ResetTurnOrder;
    }
    private void Start()
    {
    }
    public IEnumerator StartTakingTurns()
    {
        takingTurns = true;
        //Debug.Log(turnOrder.Count);
        currentTurn = turnOrder[0];
        yield return StartCoroutine(StartOfRound());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RemoveFromTurnOrder(GameObject removedObject)
    {
        if (currentTurn == removedObject)
        {
            currentTurn = turnOrder[turnOrder.IndexOf(removedObject) - 1];
            turnOrder.Remove(removedObject);
            NextTurn();
        }
        else
        {
            turnOrder.Remove(removedObject);
        }
    }
    public void NextTurn()
    {
        if (takingTurns)
        {
            //Debug.Log("next turn");
            //playerTurn = false;
            if (turnOrder.IndexOf(currentTurn) + 1 == turnOrder.Count)
            {
                //endOfRound = true;
                StartCoroutine(NextRound());
            }
            else
            {
                currentTurn = turnOrder[turnOrder.IndexOf(currentTurn) + 1];
                if (currentTurn.GetComponent<Enemy>())
                {
                    currentEnemyTurnScript = currentTurn.GetComponent<Enemy>();
                    currentEnemyTurnScript.StartStopTurn(true);
                    //currentEnemyTurnScript.isMyTurn = true;
                    //enemyTurn = true;
                }
                else
                {
                    currentEnemyTurnScript = null;
                    //enemyTurn = false;
                }
                if (currentTurn == player)
                {
                    StartCoroutine(playerControler.StartTurn());
                    //playerTurn = true;
                }
            }
        }
    }

    public IEnumerator NextRound()
    {
        yield return StartCoroutine(EndOfRound());
        yield return StartCoroutine(StartOfRound());

    }
    public IEnumerator EndOfRound()
    {
        //Debug.Log("Round ended");
        currentTurn = turnOrder[0];
        if (RoundEndedFunctions != null)
        {
            RoundEndedFunctions(this);
        }
        if (RoundEnded != null)
        {
            //Debug.Log("Round ended");
            yield return StartCoroutine(RoundEnded(this));
        }
    }
    public IEnumerator StartOfRound()
    {

        levelManager.IncreaseRoundNumber();
        if (RoundStartedFunctions != null)
        {
            //Debug.Log("Round Started");
            RoundStartedFunctions(this);
        }
        if (RoundStarted != null)
        {
            //Debug.Log(RoundStarted);
            Delegate[] listeners = RoundStarted.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<TurnManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
        }
        //Debug.Log("Round started");
        NextTurn();
    }

    public IEnumerator ResetTurnOrder(LevelManager levelManager = null)
    {
        takingTurns = false;
        if (currentTurn == player)
        {
            yield return StartCoroutine(playerControler.ForceEndTurn());
        }
        yield return StartCoroutine(EndRound());

        turnOrder.Clear();
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        currentTurn = turnOrder[0];
    }
    public IEnumerator EndRound()
    {
        takingTurns = false;
        if (currentTurn == player)
        {
            yield return StartCoroutine(playerControler.ForceEndTurn());
        }
        yield return StartCoroutine(EndOfRound());
        takingTurns = true;
    }
}
