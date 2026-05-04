using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

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
    //private Vector2 playerOneToOneCords;
    //public Vector2 PlayerOneToOneCords { get { return playerOneToOneCords; } }


    public List<System.Action> currentActionQueue = new List<System.Action>();

    private bool canPlayCards, canEndTurn, canPreformActions, cardPlayed, gettingReward, preformingAbility, preformingAction, canPreformAbilities;
    private bool waitUntilVariable;
    public bool CanPlayCards { get { UpdatePlayer(); return canPlayCards; } }
    public bool CanPreformAbilities { get { UpdatePlayer(); return canPreformAbilities; } }

    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; UpdatePlayer(); } }
    public bool PreformingAbility { get { return preformingAbility; } set { preformingAbility = value; UpdatePlayer(); } }

    public bool GettingReward { get { return gettingReward; } set { gettingReward = value; UpdatePlayer(); } }
    private int moveLeft, targetsLeft, attackDamageValue, repeats;
    private Condition[] appliedConditions;
    private bool canJump, canMove;
    private bool CanJump { set { canJump = value; UpdateMoveType(); } }
    public bool CanMove { get { UpdatePlayer(); return canMove; } set { canMove = value; } }

    private int range;
    private bool isTargetATile, isTargetAEnemy;
    private GameObject selectedTile;
    private List<Figure> posibleTargets;
    public List<string> actionsRemaining = new List<string>();
    public List<string> ActionsRemaining { set { actionsRemaining = value; statsDisplayer.Plan(actionsRemaining); } }

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayText(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayText(bottomEnergy); } }

    public bool NextAction { get { return nextAction; } set { nextAction = value; } }
    public static event Action<PlayerControler> PlayerTurnStarted;

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
        GameManager.ResetGame += ResetPlayer;

        //dev mode
        //GainNewAbility(1, new List<System.Action>() { () => Move(1000, false, true) }); GainNewAbility(1, new List<System.Action>() { () => Lockpick(1000, true) }); GainNewAbility(1, new List<System.Action>() { () => Block(1000, true) }); GainNewAbility(1, new List<System.Action>() { () => Attack(1000, 100, 1, 1, null, true) });


        GainNewAbility(1, new List<System.Action>() {() => Move(1, false ,true)});
        GainNewAbility(1, new List<System.Action>() {() => Lockpick(1, true) });



        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
        //oneToOnePos = mapManager.PosToOneToOne(player.transform.position);
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            //dev mode
            GainNewAbility(1, new List<System.Action>() { () => Move(1000, false, true) }); GainNewAbility(1, new List<System.Action>() { () => Lockpick(1000, true) }); GainNewAbility(1, new List<System.Action>() { () => Block(1000, true) }); GainNewAbility(1, new List<System.Action>() { () => Attack(1000, 100, 1, 1, null, true) });

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

    public void ResetPlayer(GameManager gameManager)
    {
        conditions.Clear();
    }
    public void PreparePlayer(GameManager gameManager)
    {
        maxHealth = 100;
        health = maxHealth;
        oneToOnePos = Vector2.zero;
        statsDisplayer.SetHealthAndBlock(health, 0);
        statsDisplayer.DisplayConditions(conditions);
        statsDisplayer.Plan(actionsRemaining);
        //statsDisplayer.SetConditions(new string[0]);
    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile && canPreformActions)
        {
            clickedTile = tile;
            //playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            //playerOneToOneCords = mapManager.PosToOneToOne(player.transform.position);
            Vector2 clickedTileCords = clickedTile.transform.position;
            if (isMoving)
            {
                //AttemptToMove(tile, clickedTileCords);
            }
        }
    }
    public void PlanMove(GameObject tile)
    {
        if (!isPreformingAnimation)
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


    }
    public IEnumerator MoveAlongPath()
    {
        pathfinder.MoveLeft = moveLeft;
        StartCoroutine(pathfinder.MoveAlongPath(gameObject, oneToOnePos));
        yield return new WaitUntil(() => pathfinder.DoneMoving == true);
        pathfinder.DoneMoving = false;
        foreach (Vector2 tileCords in pathfinder.ActualPath)
        {
            GameObject newTile = mapManager.GetTileAtHex(tileCords);
            if (newTile != null)
            {
                GameObject border = newTile.transform.Find("Border").gameObject;
                border.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
        moveLeft = pathfinder.MoveLeft;
        //actionsRemaining[0] = actionsRemaining[0];
        //actionsRemaining[0] = Regex.Replace(actionsRemaining[0], "(.)([A-Z,0-9])", "$1 $2");
        actionsRemaining[0] = Regex.Replace(actionsRemaining[0], "(Move)( )([0-9]+)", "$1 " + moveLeft);
        statsDisplayer.Plan(actionsRemaining);

        //oneToOnePos = mapManager.PosToOneToOne(player.transform.position);
        if (mapManager.GetTileAtHex(oneToOnePos).GetComponent<Door>())
        {
            GameObject door = mapManager.GetTileAtHex(oneToOnePos);
            roomSpawner.SpawnRoomsNextToDoor(door, door.GetComponent<Door>().RoomNextToCords);
        }
        if (mapManager.GetTileAtHex(oneToOnePos).GetComponent<Stair>())
        {

            levelManager.GoUpLevel();

        }
        else if (moveLeft == 0)
        {
            ActionDone();
        }
        else
        {
            Vector2 checkpos = Vector2.zero;
            bool couldMoveMore = false;
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: checkpos = oneToOnePos + Vector2.up; break;
                    case 1: checkpos = oneToOnePos + Vector2.down; break;
                    case 2: checkpos = oneToOnePos + Vector2.right; break;
                    case 3: checkpos = oneToOnePos + Vector2.left; break;
                    case 4: checkpos = oneToOnePos + Vector2.up + Vector2.right; break;
                    case 5: checkpos = oneToOnePos + Vector2.down + Vector2.left; break;
                }
                //later add enemy obstical and wall detection
                if (mapManager.GetTileAtHex(checkpos).GetComponent<Tile>().MoveCost <= moveLeft)
                {
                    couldMoveMore = true;
                }
            }
            if (!couldMoveMore)
            {
                ActionDone();
            }
        }
    }


    public void FigureClicked(GameObject figure)
    {
        //Debug.Log("clicked" + figure);
        clickedEnemy = figure;
        if (canPreformActions && isAttacking)
        {
            if (posibleTargets.Contains(figure.GetComponent<Figure>()) && targetsLeft > 0)
            {
                clickedEnemy.GetComponent<Figure>().AttackedFor(attackDamageValue, repeats, appliedConditions);
                targetsLeft--;
                posibleTargets.Remove(figure.GetComponent<Figure>());
            }
            if (targetsLeft == 0)
            {
                ActionDone();
            }
        }
        else if (canPreformActions && isAppliyingConditions)
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
        else if (figure.GetComponent<Enemy>())
        {
            StartCoroutine(figure.GetComponent<Enemy>().DisplayMovePosibilities());
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
            if (isMoving)
            {
                canMove = true;
            }
            else
            {
                canMove = false;
            }
        }
        else
        {
            canPreformActions = false;
            canMove = false;
        }
        if (cardPlayed || preformingAbility)
        {
            preformingAction = true;
        }
        else
        {
            preformingAction = false;
        }
    }

    public void StartTurn()
    {
        if (PlayerTurnStarted != null)
        {
            PlayerTurnStarted(this);
        }
        
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
        }
        if (preformingAction)
        {
            ForceEndAction();
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
        CanJump = false;
        isAttacking = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        if (preformingAction)
        {
            actionsRemaining.Remove(actionsRemaining[0]);
            statsDisplayer.Plan(actionsRemaining);
        }
        nextAction = true;
    }
    public void ForceEndAction()
    {
        if (isMoving && moveCostDisplaySetting == "On Move")
        {
            mapManager.showMoveCost(false);
        }
        isMoving = false;
        CanJump = false;
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
        CanJump = isJump;
        if (moveCostDisplaySetting == "On Move")
        {
            ShowMoveCostDisplay();
        }
    }

    public void ControledAttack(int attackValue, int attackRange, int targets, int times, Condition[] attackConditions)
    {
        actionDone = false;
        isAttacking = true;
        targetsLeft = targets;
        attackDamageValue = attackValue;
        range = attackRange;
        repeats = times;
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
        int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            //string currentDescriptionString = "Ability " + finalAbility;
            string currentDescriptionString = "<sprite name=Ability> " + finalAbility;

            planDescription.Add(currentDescriptionString);
        }
        else
        {
            abilityManager.AbilityPower += finalAbility;
            abilityManager.SelectedPower += finalAbility;
            ActionDone();
        }
    }

    public void Lockpick(int lockpickValue, bool isVariable = false)
    {
        if (isVariable)
        {

            lockpickValue *= variableCardModifier;
        }
        int finalLockpick = conditionEffects.ModifyAbility(this, lockpickValue);
        //Debug.Log(finalLockpick);
        if (isPlanning)
        {
            string currentDescriptionString = "Lockpick " + finalLockpick;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            currentTile = mapManager.GetTileAtHex(oneToOnePos);
            if (currentTile.GetComponent<Lootable>())
            {
                currentTile.GetComponent<Lootable>().Lockpick(finalLockpick);

                StartCoroutine(WaitUntilRewardSelected());
            }
            else
            {
                ActionDone();
            }
        }
    }
    public void Draw(int cardCount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Draw " + cardCount + " card";
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            deckManager.DrawCards(cardCount);
            ActionDone();
        }
    }
    public void GainEnergy(int amount,bool isTop)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Gain " + amount;
            if (isTop)
            {
                currentDescriptionString += " top";
            }
            else
            {
                currentDescriptionString += " bottom";
            }
            currentDescriptionString += " energy";
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            if (isTop)
            {
                TopEnergy += amount;
            }
            else
            {
                BottomEnergy += amount;
            }
            ActionDone();
        }
    }
    public void GainTopEnergy(int amount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Gain " + amount + " top energy";
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            TopEnergy += amount;
            ActionDone();
        }
    }
    public void GainBottomEnergy(int amount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);
        if (isPlanning)
        {
            string currentDescriptionString = "Gain " + amount + " bottom energy";
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            BottomEnergy += amount;
            ActionDone();
        }
    }
    public void GainNewAbility(int cost, List<System.Action> abilities)
    {

        if (isPlanning)
        {
            string currentDescriptionString = "Gain ability: " + cost + " AP for " + GetPlanString(abilities);
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            abilityManager.GainAbility(cost, abilities);
            ActionDone();
        }
    }
    public IEnumerator WaitUntilRewardSelected()
    {
        yield return new WaitUntil(() => gettingReward == false);
        //Debug.Log("test");
        ActionDone();
    }
    public IEnumerator WaitUntil()
    {
        yield return new WaitUntil(() => waitUntilVariable == false);
        //Debug.Log("test");
        ActionDone();
    }

    public override void Die()
    {
        gameManager.ReStartGame();
        Debug.Log("You Died");
    }

    public void UpdateMoveType()
    {
        ShowMoveCostDisplay();
    }



}
