using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //public List<Enemy> EnemiesScripts = new List<Enemy>();
    private List<GameObject> enemies = new List<GameObject>();
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameObject player;
    private GameObject newRoundMarker;
    private GameObject currentTurn;
    private Enemy currentEnemyTurnScript;
    private List<GameObject> turnOrder = new List<GameObject>();
    public List<GameObject> TurnOrder { get { return turnOrder; } }
    private bool endOfRound, playerTurn, enemyTurn;
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
        LevelManager.LevelGenerated += ResetTurnOrder;

    }
    private void Start()
    {
    }
    public void StartTakingTurns()
    {
        currentTurn = turnOrder[0];
        if (RoundStarted != null)
        {
            RoundStarted(this);
        }
        NextTurn();
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
        //Debug.Log("next turn");
        playerTurn = false;
        if (turnOrder.IndexOf(currentTurn) + 1 == turnOrder.Count)
        {
            endOfRound = true;
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
                enemyTurn = true;
            }
            else
            {
                currentEnemyTurnScript = null;
                enemyTurn = false;
            }
            if (currentTurn == player)
            {
                StartCoroutine(playerControler.StartTurn());
                playerTurn = true;
            }

        }
    }

    public IEnumerator NextRound()
    {
        currentTurn = turnOrder[0];
        if (RoundEndedFunctions != null)
        {
            //Debug.Log("Round ended");
            RoundEndedFunctions(this);
        }
        if (RoundEnded != null)
        {
            //Debug.Log("Round ended");
            yield return StartCoroutine(RoundEnded(this));
        }
        levelManager.IncreaseRoundNumber();
        if (RoundStartedFunctions != null)
        {
            //Debug.Log("Round Started");
            RoundStartedFunctions(this);
        }
        if (RoundStarted != null)
        {
            //Debug.Log("Round ended");
            yield return StartCoroutine(RoundStarted(this));
        }



        NextTurn();
    }

    public IEnumerator ResetTurnOrder(LevelManager levelManager = null)
    {
        turnOrder.Clear();
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        currentTurn = turnOrder[1];
        yield return StartCoroutine(playerControler.ForceEndTurn());
    }


}
