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
    private PlayerStats playerStats;
    private GameObject player;
    private GameObject newRoundMarker;
    private GameObject currentTurn;
    private AIFigure currentAIFigureTurnScript;
    private List<GameObject> turnOrder = new List<GameObject>();
    public List<GameObject> TurnOrder { get { return turnOrder; } }
    //private bool endOfRound, playerTurn, enemyTurn;
    private bool takingTurns;

    private FloorManager floorManager;


    public static event Action<TurnManager> RoundEndedFunctions;
    public static event Action<TurnManager> RoundStartedFunctions;

    public static event Func<TurnManager,IEnumerator> RoundEnded;
    public static event Func<TurnManager, IEnumerator> RoundStarted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = RefrenceStorage.player;
        playerControler = RefrenceStorage.playerControler;
        deckManager = RefrenceStorage.deckManager;
        playerStats = RefrenceStorage.playerStats;
        floorManager = RefrenceStorage.floorManager;
        newRoundMarker = GameObject.Find("NewRoundMarker");
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        FloorManager.FloorCleared += ResetTurnOrder;
    }
    //enable the turn order
    public IEnumerator StartTakingTurns()
    {
        takingTurns = true;
        //Debug.Log(turnOrder.Count);
        currentTurn = turnOrder[0];
        yield return StartCoroutine(StartOfRound());
    }
    //remove a object from the turn order
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
    //take the next turn
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
                if (currentTurn.GetComponent<AIFigure>())
                {
                    currentAIFigureTurnScript = currentTurn.GetComponent<AIFigure>();
                    currentAIFigureTurnScript.StartStopTurn(true);
                    //currentEnemyTurnScript.isMyTurn = true;
                    //enemyTurn = true;
                }
                else
                {
                    currentAIFigureTurnScript = null;
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
    //preform next round
    public IEnumerator NextRound()
    {
        yield return StartCoroutine(EndOfRound());
        yield return StartCoroutine(StartOfRound());

    }
    //run end of round stuff
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
            Delegate[] listeners = RoundEnded.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<TurnManager, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
            //Debug.Log("Round ended");
            //yield return StartCoroutine(RoundEnded(this));
        }
    }
    //run start of round stuff
    public IEnumerator StartOfRound()
    {
        OverallStatistics.round++;
        playerStats.SetTurnCount(OverallStatistics.round);
        OverallStatistics.difficultyRound++;

        //the * floor is a temerary thing to make the enemies harder

        OverallStatistics.difficulty = Mathf.Pow(1.001f, OverallStatistics.difficultyRound) * (1 + (float)(OverallStatistics.floor-1) * 0.5f);
        //OverallStatistics.difficulty += 0.001f * OverallStatistics.difficulty;
        OverallStatistics.enemyScaling = Mathf.RoundToInt(OverallStatistics.difficulty*1000) - 1000;
        float difficulty = 1 + (float)OverallStatistics.enemyScaling / 1000;
        //Debug.Log((float)OverallStatistics.enemyScaling);
        //Debug.Log(difficulty);
        playerStats.SetDifficulty(difficulty);

        floorManager.IncreaseRoundNumber();
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
    //Reset turn order to just the player and stop taking turns
    public IEnumerator ResetTurnOrder(FloorManager floorManager = null)
    {
        takingTurns = false;
        if (currentTurn == player)
        {
            yield return StartCoroutine(playerControler.ForceEndTurn());
        }
        yield return StartCoroutine(EndOfRound());

        turnOrder.Clear();
        turnOrder.Add(newRoundMarker);
        turnOrder.Add(player);
        currentTurn = turnOrder[0];
    }
}
