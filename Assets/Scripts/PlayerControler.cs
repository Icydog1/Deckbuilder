using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool actionDone, manualEnd;
    public bool isMoving, isAttacking;
    public bool isPlayerTurn;
    private GameObject player;
    private MouseManager mouseManager;
    private MapManager mapManager;
    private TurnManager turnManager;
    private RoomSpawner roomSpawner;
    public GameObject clickedTile, clickedEnemy;
    public GameObject playedCard;
    private PlayerStats playerStats;
    public Card playedCardScript;
    public Vector2 playerOneToOneCords;
    private int moveLeft, targetsLeft, attackDamageValue;
    private bool canJump, canFly;
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
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        //Debug.Log(playerStats);
        playerOneToOneCords = Vector2.zero;


        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile)
        {
            clickedTile = tile;
            //playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
            Vector2 clickedTileCords = clickedTile.transform.position;
            if (isMoving)
            {
                AttemptToMove(tile, clickedTileCords);
            }

        }
    }
    public void AttemptToMove(GameObject tile, Vector2 tileCords)
    {
        int distance = mapManager.GetDistanceBetweenPos(tileCords, player.transform.position);
        if (distance <= moveLeft)
        {
            if (tile.GetComponent<Door>())
            {
                MoveTo(tileCords, distance);
                roomSpawner.SpawnRoomsNextToDoor(mapManager.PosToOneToOne(tileCords), tile.GetComponent<Door>().RoomNextToCords);
            }
            else if (!tile.GetComponent<Wall>() && (!tile.GetComponent<Obstacle>() || canFly))
            {
                MoveTo(tileCords, distance);
            }
        }
        if (moveLeft == 0)
        {
            ActionDone();
            Debug.Log("Done Moving");
        }
    }

    public void MoveTo(Vector2 tileCords, int distance)
    {
        moveLeft -= distance;
        player.transform.position = tileCords;
    }
    public void EnemyClicked(GameObject enemy)
    {
        clickedEnemy = enemy;
        if (isTargetAEnemy)
        {
            if (isAttacking)
            {
                Vector2 clickedEnemyCords = clickedEnemy.transform.position;
                //dont calculate range
                if (mapManager.GetDistanceBetweenPos(clickedEnemyCords, player.transform.position) <= range)
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
