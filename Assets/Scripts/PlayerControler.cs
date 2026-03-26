using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerControler : Figure
{
    public bool actionDone, manualEnd;
    public bool isMoving, isAttacking;
    public bool isPlayerTurn;
    private GameObject player;
    private RoomSpawner roomSpawner;
    public GameObject clickedTile, clickedEnemy;
    public GameObject playedCard;
    private GameObject currentTile;
    private VariableDisplayer topEnergyDisplay, bottomEnergyDisplay;
    private RewardManager rewardManager;
    private GameManager gameManager;
    //private PlayerStats playerStats;
    private DeckManager deckManager;
    public Card playedCardScript;
    public Vector2 playerOneToOneCords;

    public List<System.Action> currentActionQueue = new List<System.Action>();

    private bool canPlayCards, canEndTurn, canPreformActions, cardPlayed, gettingReward;
    private bool waitUntilVariable;
    public bool CanPlayCards { get { UpdatePlayer(); return canPlayCards; } }
    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; } }
    public bool GettingReward { get { return gettingReward; } set { gettingReward = value; UpdatePlayer(); } }
    private int moveLeft, targetsLeft, attackDamageValue;
    private bool canJump;
    private int range;
    private bool isTargetATile, isTargetAEnemy;

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayText(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayText(bottomEnergy); } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        player = GameObject.Find("Player");
        statsDisplayer = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        topEnergyDisplay = GameObject.Find("TopEnergyDisplay").GetComponent<VariableDisplayer>();
        bottomEnergyDisplay = GameObject.Find("BottomEnergyDisplay").GetComponent<VariableDisplayer>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        isPlayer = true;

        //Debug.Log(playerStats);
        GameManager.GameStarted += PreparePlayer;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
    }

    public void PreparePlayer(GameManager gameManager)
    {
        maxHealth = 100;
        health = maxHealth;
        playerOneToOneCords = Vector2.zero;
        statsDisplayer.SetHealthAndBlock(health, 0);
        statsDisplayer.SetConditions(new string[0]);

    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile && canPreformActions)
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
                if (tile.GetComponent<Lootable>())
                {
                    //rewardManager.TileReward(tile);
                }
            }
        }
        if (moveLeft == 0)
        {
            ActionDone();
            //Debug.Log("Done Moving");
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
        if (isTargetAEnemy && canPreformActions)
        {
            if (isAttacking)
            {
                //dont calculate range
                if (mapManager.GetDistanceBetweenPos(clickedEnemy.transform.position, player.transform.position) <= range)
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
        if (!cardPlayed && !gettingReward && isPlayerTurn && !deckManager.IsDisplayingCards)
        {
            canPlayCards = true;
            canEndTurn = true;
        }
        else
        {
            canPlayCards = false;
            canEndTurn = false;
        }
        if (!gettingReward && isPlayerTurn && !deckManager.IsDisplayingCards)
        {
            canPreformActions = true;
        }
        else
        {
            canPreformActions = false;
        }
    }

    public void StartTurn()
    {
        isPlayerTurn = true;
        TopEnergy = 2;
        BottomEnergy = 2;
        base.baseStartTurn();
    }
    public void EndTurn()
    {
        UpdatePlayer();
        if (canEndTurn)
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

    public override void ActionDone()
    {
        isMoving = false;
        isAttacking = false;
        //manualEnd = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        //playedCardScript.currentStep++;
        playedCardScript.NextAction = true;
    }

    public void ControledMove(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        isTargetATile = true;
        moveLeft = moveValue;
    }

    public void ControledAttack(int attackValue, int attackRange, int targets)
    {
        actionDone = false;
        isAttacking = true;
        targetsLeft = targets;
        attackDamageValue = attackValue;
        range = attackRange;
        isTargetAEnemy = true;
    }

    public void Lockpick(int lockpickValue)
    {
        float additionalLockpick = lockpickValue;


        int finalLockpick = Mathf.FloorToInt(additionalLockpick);
        if (isPlanning)
        {
            prepareActions.Add(() => Lockpick(finalLockpick));
            string currentDescriptionString = "Lockpick " + finalLockpick;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            currentTile = mapManager.GetTileAtHex(playerOneToOneCords);
            if (currentTile.GetComponent<Lootable>())
            {
                currentTile.GetComponent<Lootable>().Lockpick(finalLockpick);
                waitUntilVariable = !gettingReward;
                StartCoroutine(WaitUntil());
            }
        }
    }

    public IEnumerator WaitUntilRewardSelected()
    {
        yield return new WaitUntil( () => gettingReward == true);
        ActionDone();
    }


    public IEnumerator WaitUntil()
    {
        yield return new WaitUntil(() => waitUntilVariable == true);
        Debug.Log("test");
        ActionDone();
    }
    /*
    public void Block(int blockValue)
    {
        block += blockValue;
        playerStats.SetHealthAndBlock(health, block);
        ActionDone();
    }

    public void AttackedFor(int attackValue)
    {
        if (block > 0 )
        {
            int damageBlocked = Mathf.Min(attackValue, block);
            attackValue -= damageBlocked;
            block -= damageBlocked;
        }
        health -= attackValue;
        playerStats.SetHealthAndBlock(health, block);
    }
    */
    public override void Die()
    {
        Debug.Log("You Died");
    }


}
