using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool actionDone, manualEnd;
    private bool isMoving, isAttacking;
    public bool isPlayerTurn;
    private GameObject player;
    private MouseManager mouseManager;
    private MapManager mapManager;
    private TurnManager turnManager;
    public GameObject clickedTile, clickedEnemy;
    public GameObject playedCard;
    private PlayerStats playerStats;
    public Card playedCardScript;
    private Vector3 playerHexCords;
    public Vector2 playerOneToOneCords;
    private int moveLeft, targetsLeft, attackDamageValue;
    private int range;
    private bool isTargetATile, isTargetAEnemy;

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; } }

    public int maxHealth = 100, health;

    public List<System.Action> currentActionQueue = new List<System.Action>();

    public bool canPlayCards;
    private bool cardPlayed;

    public bool CanPlayCards { get { return canPlayCards; } }
    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        player = GameObject.Find("Player");
        playerStats = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        //Debug.Log(playerStats);
        playerOneToOneCords = Vector2.zero;


        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
        playerOneToOneCords = mapManager.HexToOneToOne(playerHexCords);
    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile)
        {
            clickedTile = tile;
            playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            playerOneToOneCords = mapManager.HexToOneToOne(playerHexCords);
            Vector2 clickedTileCords = clickedTile.transform.position;

            if (mapManager.GetDistanceTo(clickedTileCords, player.transform.position) <= moveLeft)
            {
                moveLeft -= mapManager.GetDistanceTo(clickedTileCords, player.transform.position);
                player.transform.position = clickedTileCords;
                Debug.Log(moveLeft);
            }
            if (moveLeft == 0)
            {
                ActionDone();
                Debug.Log("Done Moving");
            }
        }
    }
    public void EnemyClicked(GameObject enemy)
    {
        clickedEnemy = enemy;
        if (isTargetAEnemy)
        {
            if (isAttacking)
            {
                Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedEnemy.transform.position);
                Vector2 clickedEnemyCords = clickedEnemy.transform.position;
                //sometimes doent work
                if (mapManager.GetDistanceTo(clickedEnemyCords, player.transform.position) <= range)
                {
                    clickedEnemy.GetComponent<Enemy>().AttackedFor(attackDamageValue);
                    targetsLeft--;
                }
                else
                {
                    Debug.Log("Out Of range");
                }
                if (targetsLeft == 0)
                {
                    ActionDone();
                    //Debug.Log("Done Attacking");
                }
            }
        }

    }
    public void UpdatePlayer()
    {
        if (!cardPlayed && isPlayerTurn)
        {
            canPlayCards = true;
        }
        else
        {
            canPlayCards = false;
        }
    }

    public void StartTurn()
    {
        isPlayerTurn = true;
        UpdatePlayer();
    }
    public void EndTurn()
    {
        if (!cardPlayed && isPlayerTurn)
        {
            isPlayerTurn = false;
            UpdatePlayer();
            turnManager.NextTurn();
        }
    }
    public void ManualEnd()
    {
        if (cardPlayed && isPlayerTurn)
        {
            ActionDone();
        }
    }

    public void ActionDone()
    {
        isMoving = false;
        isAttacking = false;
        manualEnd = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        playedCardScript.currentStep++;
        playedCardScript.nextAction = true;
    }

    public void Move(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        isTargetATile = true;
        moveLeft = moveValue;
    }

    public void Attack(int attackValue, int attackRange = 1, int targets = 1)
    {
        actionDone = false;
        isAttacking = true;
        targetsLeft = targets;
        attackDamageValue = attackValue;
        range = attackRange;
        isTargetAEnemy = true;

    }

    public void AttackedFor(int attackValue, int range = 1)
    {
        health -= attackValue;
        playerStats.SetHealth(health);
    }

}
