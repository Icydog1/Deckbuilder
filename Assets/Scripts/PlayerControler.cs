using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerControler : Figure
{
    public bool actionDone, manualEnd;
    public bool isMoving, isAttacking, isAppliyingConditions;
    public bool isPlayerTurn;
    private GameObject player;
    private RoomSpawner roomSpawner;
    public GameObject clickedTile, clickedEnemy;
    public GameObject playedCard;
    private GameObject currentTile;
    private VariableDisplayer topEnergyDisplay, bottomEnergyDisplay;
    private RewardManager rewardManager;
    private GameManager gameManager;
    private AbilityManager abilityManager;

    private string moveCostDisplaySetting;
    public string MoveCostDisplaySetting { set { moveCostDisplaySetting = value; ShowMoveCostDisplay(); } }

    //private PlayerStats playerStats;
    public Card playedCardScript;
    public Vector2 playerOneToOneCords;

    public List<System.Action> currentActionQueue = new List<System.Action>();

    private bool canPlayCards, canEndTurn, canPreformActions, cardPlayed, gettingReward, preformingAbility, canPreformAbilities;
    private bool waitUntilVariable;
    public bool CanPlayCards { get { UpdatePlayer(); return canPlayCards; } }
    public bool CanPreformAbilities { get { UpdatePlayer(); return canPreformAbilities; } }

    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; } }
    public bool PreformingAbility { get { return preformingAbility; } set { preformingAbility = value; } }

    public bool GettingReward { get { return gettingReward; } set { gettingReward = value; UpdatePlayer(); } }
    private int moveLeft, targetsLeft, attackDamageValue;
    private Condition[] appliedConditions;
    private bool canJump;
    private int range;
    private bool isTargetATile, isTargetAEnemy;
    private GameObject selectedTile;
    private List<Figure> posibleTargets;
    private List<string> actionsRemaining = new List<string>();
    public List<string> ActionsRemaining { set { actionsRemaining = value; statsDisplayer.Plan(actionsRemaining); } }

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayText(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayText(bottomEnergy); } }

    public bool NextAction { get { return nextAction; } set { nextAction = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        player = GameObject.Find("Player");
        statsDisplayer = GameObject.Find("PlayerStats").GetComponent<PlayerStats>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        topEnergyDisplay = GameObject.Find("TopEnergyDisplay").GetComponent<VariableDisplayer>();
        bottomEnergyDisplay = GameObject.Find("BottomEnergyDisplay").GetComponent<VariableDisplayer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();

        base.Awake(); 
    }

    public override void Start()
    {

        isPlayer = true;
        team = 0;
        //Debug.Log(playerStats);
        GameManager.GameStarted += PreparePlayer;
        GainNewAbility(1, new List<System.Action>() {() => Move(1, false ,true) });
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
        oneToOnePos = mapManager.PosToOneToOne(player.transform.position);
        if (isMoving && !isPreformingAnimation)
        {
            if (mouseManager.SelectedObject)
            {
                if (Input.GetMouseButton(0))
                {
                    if (selectedTile != mouseManager.SelectedObject)
                    {
                        selectedTile = mouseManager.SelectedObject;
                        if (selectedTile.GetComponent<Tile>())
                        {
                            PlanMove(selectedTile);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (mouseManager.SelectedObject.GetComponent<Tile>())
                    {
                        StartCoroutine(MoveAlongPath());
                    }
                }
            }

        }

    }

    public void ShowMoveCostDisplay()
    {
        if (moveCostDisplaySetting == "Always" || (moveCostDisplaySetting == "On Move" && isMoving))
        {
            mapManager.showMoveCost(true, canJump, canFly);
        }
        else
        {
            mapManager.showMoveCost(false);
        }
    }
    public void PreparePlayer(GameManager gameManager)
    {
        maxHealth = 100;
        health = maxHealth;
        playerOneToOneCords = Vector2.zero;
        statsDisplayer.SetHealthAndBlock(health, 0);
        //statsDisplayer.SetConditions(new string[0]);
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
                //AttemptToMove(tile, clickedTileCords);
            }
        }
    }
    public void PlanMove(GameObject tile)
    {
        foreach (Vector2 tileCords in pathfinder.ActualPath)
        {
            GameObject newTile = mapManager.GetTileAtHex(tileCords);
            if (newTile != null)
            {
                GameObject border = newTile.transform.Find("Border").gameObject;
                border.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        pathfinder.PlanPathToTile(oneToOnePos, mapManager.PosToOneToOne(tile.transform.position), gameObject, moveLeft, canJump, canFly);
        foreach (Vector2 tileCords in pathfinder.ActualPath)
        {
            GameObject newTile = mapManager.GetTileAtHex(tileCords);
            GameObject border = newTile.transform.Find("Border").gameObject;
            border.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
    }
    public IEnumerator MoveAlongPath()
    {
        pathfinder.MoveLeft = moveLeft;
        StartCoroutine(pathfinder.MoveAlongPath(gameObject, oneToOnePos));
        yield return new WaitUntil(() => pathfinder.DoneMoving == true);
        pathfinder.DoneMoving = false;
        moveLeft = pathfinder.MoveLeft;
        oneToOnePos = mapManager.PosToOneToOne(player.transform.position);
        if (mapManager.GetTileAtHex(oneToOnePos).GetComponent<Door>())
        {
            GameObject door = mapManager.GetTileAtHex(oneToOnePos);
            roomSpawner.SpawnRoomsNextToDoor(mapManager.PosToOneToOne(door.transform.position), door.GetComponent<Door>().RoomNextToCords);
        }
        if (mapManager.GetTileAtHex(oneToOnePos).GetComponent<Stair>())
        {
            levelManager.GoUpLevel();

        }
        if (moveLeft == 0)
        {
            ActionDone();
        }
    }


    public void FigureClicked(GameObject figure)
    {
        //Debug.Log("clicked" + figure);
        clickedEnemy = figure;
        if (canPreformActions)
        {
            if (isAttacking)
            {
                if (posibleTargets.Contains(figure.GetComponent<Figure>()) && targetsLeft > 0)
                {
                    clickedEnemy.GetComponent<Figure>().AttackedFor(attackDamageValue, appliedConditions);
                    targetsLeft--;
                    posibleTargets.Remove(figure.GetComponent<Figure>());
                }
                if (targetsLeft == 0)
                {
                    ActionDone();
                }
            }
            else if (isAppliyingConditions)
            {
                if (posibleTargets.Contains(figure.GetComponent<Figure>()) && targetsLeft > 0)
                {
                    clickedEnemy.GetComponent<Figure>().GainConditions(appliedConditions);
                    targetsLeft--;
                    posibleTargets.Remove(figure.GetComponent<Figure>());
                }
                if (targetsLeft == 0)
                {
                    ActionDone();
                }
            }
        }

    }
    public void UpdatePlayer()
    {
        if (!cardPlayed && !gettingReward && isPlayerTurn && !deckManager.IsDisplayingCards && !isPreformingAnimation && !preformingAbility)
        {
            canPlayCards = true;
            canEndTurn = true;
            canPreformAbilities = true;
        }
        else
        {
            canPlayCards = false;
            canEndTurn = false;
            canPreformAbilities = false;
        }
        if (!gettingReward && isPlayerTurn && !deckManager.IsDisplayingCards && !isPreformingAnimation)
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

    public void ForceEndTurn()
    {
        if (cardPlayed)
        {
            playedCardScript.StopPlaying = true;
            CardDone();
        }
        isPlayerTurn = false;
        UpdatePlayer();
        base.baseEndTurn();
    }
    public void EndTurn()
    {
        UpdatePlayer();
        if (canEndTurn)
        {
            isPlayerTurn = false;
            UpdatePlayer();
            base.baseEndTurn();
        }
    }
    public void ManualEnd()
    {
        if ((cardPlayed || preformingAbility) && isPlayerTurn)
        {
            ActionDone();
        }
    }

    public override void ActionDone()
    {
        if (isMoving && moveCostDisplaySetting == "On Move")
        {
            mapManager.showMoveCost(false);
        }
        isMoving = false;
        canJump = false;
        isAttacking = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        if (cardPlayed)
        {
            actionsRemaining.Remove(actionsRemaining[0]);
            statsDisplayer.Plan(actionsRemaining);
        }
        nextAction = true;
    }
    public void CardDone()
    {
        if (isMoving && moveCostDisplaySetting == "On Move")
        {
            mapManager.showMoveCost(false);
        }
        isMoving = false;
        canJump = false;
        isAttacking = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        actionsRemaining.Clear();
        statsDisplayer.Plan(actionsRemaining);
        nextAction = true;
    }


    public void ControledMove(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        isTargetATile = true;
        moveLeft = moveValue;
        canJump = isJump;
        if (moveCostDisplaySetting == "On Move")
        {
            mapManager.showMoveCost(true, isJump, canFly);
        }
    }

    public void ControledAttack(int attackValue, int attackRange, int targets, Condition[] attackConditions)
    {
        actionDone = false;
        isAttacking = true;
        targetsLeft = targets;
        attackDamageValue = attackValue;
        range = attackRange;
        isTargetAEnemy = true;
        appliedConditions = attackConditions;
        posibleTargets = FindPosibleTargets("enemy", attackRange);
    }

    public void ControledApplyConditions(Condition[] newConditions, string targetType, int conditionsRange, int targets)
    {
        actionDone = false;
        isAppliyingConditions = true;
        targetsLeft = targets;
        range = conditionsRange;
        appliedConditions = newConditions;
        posibleTargets = FindPosibleTargets(targetType, conditionsRange);
    }
    public void Ability(int abilityValue)
    {
        int finalAbility = conditionManager.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Ability " + finalAbility;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            abilityManager.AbilityPower += finalAbility;
        }
    }

    public void Lockpick(int lockpickValue, bool isVariable = false)
    {
        if (isVariable)
        {

            lockpickValue *= variableCardModifier;
        }
        int finalLockpick = conditionManager.ModifyAbility(this, lockpickValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Lockpick " + finalLockpick;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            currentTile = mapManager.GetTileAtHex(playerOneToOneCords);
            if (currentTile.GetComponent<Lootable>())
            {
                currentTile.GetComponent<Lootable>().Lockpick(finalLockpick);

                StartCoroutine(WaitUntilRewardSelected());
            }
        }
    }

    public void GainNewAbility(int cost, List<System.Action> abilities)
    {

        if (isPlanning)
        {
            string currentDescriptionString = "Gain ability " + "";
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            abilityManager.GainAbility(cost, abilities);
        }
    }
    public IEnumerator WaitUntilRewardSelected()
    {
        yield return new WaitUntil(() => gettingReward == false);
        Debug.Log("test");
        ActionDone();
    }
    public IEnumerator WaitUntil()
    {
        yield return new WaitUntil(() => waitUntilVariable == false);
        Debug.Log("test");
        ActionDone();
    }

    public override void Die()
    {
        Debug.Log("You Died");
    }


}
