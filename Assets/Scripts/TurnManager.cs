using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    //public List<Enemy> EnemiesScripts = new List<Enemy>();
    public List<GameObject> Enemies = new List<GameObject>();
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameObject player;
    public GameObject currentTurn;
    public Enemy currentEnemyTurnScript;
    public List<GameObject> turnOrder = new List<GameObject>();
    public bool endOfRound, playerTurn, enemyTurn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        playerControler = player.GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();

        turnOrder.Add(player);
        currentTurn = player;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NextTurn()
    {
        if (turnOrder.IndexOf(currentTurn) + 1 == turnOrder.Count)
        {
            NextRound();
            endOfRound = true;
        }
        else
        {
            currentTurn = turnOrder[turnOrder.IndexOf(currentTurn) + 1];
            if (currentTurn.GetComponent<Enemy>())
            {
                currentEnemyTurnScript = currentTurn.GetComponent<Enemy>();
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
        deckManager.DrawNewHand();
        currentTurn = turnOrder[0];
    }

}
