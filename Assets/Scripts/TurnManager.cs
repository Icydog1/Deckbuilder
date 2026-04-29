using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //public List<Enemy> EnemiesScripts = new List<Enemy>();
    public List<GameObject> Enemies = new List<GameObject>();
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameObject player;
    private GameObject newRoundMarker;
    public GameObject currentTurn;
    public Enemy currentEnemyTurnScript;
    public List<GameObject> turnOrder = new List<GameObject>();
    public bool endOfRound, playerTurn, enemyTurn;
    private LevelManager levelManager;


    public static event Action<TurnManager> RoundEnded;
    public static event Action<TurnManager> RoundStarted;
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
            NextRound();
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
                playerControler.StartTurn();
                playerTurn = true;
            }

        }
    }

    public void NextRound()
    {
        currentTurn = turnOrder[0];
        deckManager.DrawNewHand();
        if (RoundEnded != null)
        {
            //Debug.Log("Round ended");
            RoundEnded(this);
        }
        levelManager.IncreaseRoundNumber();
        if (RoundStarted != null)
        {
            //Debug.Log("Round Started");
            RoundStarted(this);
        }


        NextTurn();
    }

    public void ResetTurnOrder(LevelManager levelManager = null)
    {
        turnOrder.Clear();
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        currentTurn = turnOrder[1];
        playerControler.ForceEndTurn();
    }


}
