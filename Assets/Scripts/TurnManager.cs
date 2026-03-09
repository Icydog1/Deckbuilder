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

    public static event Action<TurnManager> RoundEnded;
    public static event Action<TurnManager> RoundStarted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player");
        playerControler = player.GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        newRoundMarker = GameObject.Find("NewRoundMarker");
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
    }
    private void Start()
    {
        currentTurn = player;

        if (RoundStarted != null)
        {
            RoundStarted(this);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextTurn()
    {
        Debug.Log("next turn");
        playerTurn = false;
        enemyTurn = false;
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
                StartCoroutine(currentEnemyTurnScript.TakeTurn());
                //currentEnemyTurnScript.isMyTurn = true;
                enemyTurn = true;
            }
            if (currentTurn == player)
            {
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
            RoundEnded(this);
        }
        if (RoundStarted != null)
        {
            RoundStarted(this);
        }
        

        NextTurn();
    }




}
