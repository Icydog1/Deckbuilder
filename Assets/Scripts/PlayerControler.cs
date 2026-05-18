using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class PlayerControler : Figure
{
    private bool actionDone, manualEnd;
    private bool isMoving, isAttacking, isAppliyingConditions;
    private bool isPlayerTurn;
    private GameObject player;
    private RoomSpawner roomSpawner;
    private GameObject clickedTile, clickedEnemy;
    private GameObject playedCard;
    public GameObject PlayedCard { get { return playedCard; } set { playedCard = value; } }
    private VariableDisplayer topEnergyDisplay, bottomEnergyDisplay;
    private RewardManager rewardManager;
    private GameManager gameManager;
    private AbilityManager abilityManager;

    private string moveCostDisplaySetting;
    public string MoveCostDisplaySetting { set { moveCostDisplaySetting = value; ShowMoveCostDisplay(); } }

    //private PlayerStats playerStats;
    private Card playedCardScript;
    public Card PlayedCardScript { get { return playedCardScript; } set { playedCardScript = value; } }

    //private Vector2 playerOneToOneCords;
    //public Vector2 PlayerOneToOneCords { get { return playerOneToOneCords; } }


    //private List<Func<IEnumerator>> currentActionQueue = new List<Func<IEnumerator>>();
    //public List<Func<IEnumerator>> CurrentActionQueue { get { return currentActionQueue; } set { currentActionQueue = value; } }


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
    private GameObject interactButton;
    private GameObject currentTile;
    public GameObject CurrentTile { get { return currentTile; } set { currentTile = value; } }

    //public GameObject CurrentTile { get { return currentTile; } set { currentTile = value; } }

    private List<Figure> posibleTargets;
    private List<string> actionsRemaining = new List<string>();
    public List<string> ActionsRemaining { get { return actionsRemaining; } set { actionsRemaining = value; statsDisplayer.Plan(actionsRemaining); } }

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayText(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayText(bottomEnergy); } }

    public bool NextAction { get { return nextAction; } set { nextAction = value; } }
    public static event Action<PlayerControler> PlayerTurnStartedFuntions;
    public static event Func<PlayerControler, IEnumerator> PlayerTurnStarted;
    
    private int kineticBatteryCount, kineticBatterySteps;
    public int KineticBatteryCount { get { return kineticBatteryCount; } set { kineticBatteryCount = value;} }

    private int level, potentialLevel, XP, XPThreshold;
    public int Level { get { return level; } set { level = value; } }
    public int PotentialLevel { get { return potentialLevel; } set { potentialLevel = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        player = GameObject.Find("Player");
        statsDisplayer = GameObject.Find("PlayerStats").GetComponent<FigureStats>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        topEnergyDisplay = GameObject.Find("TopEnergyDisplay").GetComponent<VariableDisplayer>();
        bottomEnergyDisplay = GameObject.Find("BottomEnergyDisplay").GetComponent<VariableDisplayer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        interactButton = GameObject.Find("InteractButton");
        //interactButton.SetActive(false);

        base.Awake(); 
    }

    public override void Start()
    {

        isPlayer = true;
        team = 0;
        //Debug.Log(playerStats);
        GameManager.LateGameStarted += PreparePlayer;
        GameManager.ResetGame += ResetPlayer;

        //dev mode
        //GainNewAbility(1, new List<Func<IEnumerator>>() { () => Move(1000, false, true) }); GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1000, true) }); GainNewAbility(1, new List<Func<IEnumerator>>() { () => Block(1000, true) }); GainNewAbility(1, new List<Func<IEnumerator>>() { () => Attack(1000, 100, 1, 1, null, true) });






        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

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
            StartCoroutine(CheatMode());

            //GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1000, true) });
            //GainNewAbility(1, new List<Func<IEnumerator>>() { () => Block(1000, true) });
            //GainNewAbility(1, new List<Func<IEnumerator>>() { () => Attack(1000, 100, 1, 1, null, true) });
            //ApplyCondition(new GainAbility(new Ability(2, new List<Func<IEnumerator>>() { () => Move(1, true, true) })));

        }
    }
    public IEnumerator CheatMode()
    {
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Move(1000, true, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Block(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Attack(1000, 100, 1, 1, null, true) })));
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
        kineticBatterySteps = 0;
        kineticBatteryCount = 0;
        conditions.Clear();
    }
    public IEnumerator PreparePlayer(GameManager gameManager)
    {
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(2, new List<Func<IEnumerator>>() {() => Move(1, false, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() {() => Lockpick(1, true) })));
        level = 1;
        potentialLevel = 1;
        XP = 0;
        XPThreshold = 10;
        maxHealth = 100;
        statsDisplayer.SetLeveAndXP(level, potentialLevel, XP, XPThreshold);
        health = maxHealth;
        transform.position = new Vector3 (0,0,transform.position.z);
        oneToOnePos = Vector2.zero;
        currentTile = mapManager.GetTileAtHex(oneToOnePos);
        statsDisplayer.SetHealthAndBlock(health, 0);
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
        statsDisplayer.Plan(actionsRemaining);
        //statsDisplayer.SetConditions(new string[0]);
    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile && canPreformActions)
        {
            clickedTile = tile;

            Vector2 clickedTileCords = clickedTile.transform.position;
            if (isMoving)
            {
                //AttemptToMove(tile, clickedTileCords);
            }
        }
    }
    public void PlanMove(GameObject tile)
    {
        //Debug.Log("moveing twords " + tile + " at " + mapManager.PosToOneToOne(tile.transform.position));
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
        yield return StartCoroutine(pathfinder.MoveAlongPath(gameObject, oneToOnePos));
        //yield return new WaitUntil(() => pathfinder.DoneMoving == true);
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
        if (actionsRemaining.Count > 0)
        {
            actionsRemaining[0] = Regex.Replace(actionsRemaining[0], "(Move)( )([0-9]+)", "$1 " + moveLeft);
        }
        statsDisplayer.Plan(actionsRemaining);
        if (currentTile.GetComponent<Interactable>())
        {
            interactButton.SetActive(true);
        }
        else
        {
            interactButton.SetActive(false);
        }
        if (currentTile.GetComponent<Door>())
        {
            roomSpawner.SpawnRoomsNextToDoor(currentTile, currentTile.GetComponent<Door>().RoomNextToCords);
            //way to make it so enemies do spawning stuff without having to go through a long chain of coroutines
            //loads enemies that spawned
            yield return StartCoroutine(actionManager.PreformPreparedActions());
            //updates current tile as it is no longer a door
            currentTile = mapManager.GetTileAtHex(oneToOnePos);
        }
        if (currentTile.GetComponent<Stair>())
        {
            //yield return StartCoroutine(actionManager.PreformPreparedActions());
            yield return StartCoroutine(levelManager.GoUpLevel());
        }
        else if (moveLeft == 0)
        {
            EndAction();
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
                EndAction();
            }
        }

    }


    public IEnumerator FigureClicked(GameObject figure)
    {
        //Debug.Log("clicked" + figure);
        clickedEnemy = figure;
        if (canPreformActions && isAttacking)
        {
            if (posibleTargets.Contains(figure.GetComponent<Figure>()) && targetsLeft > 0)
            {
                targetsLeft--;
                posibleTargets.Remove(figure.GetComponent<Figure>());
                yield return StartCoroutine(clickedEnemy.GetComponent<Figure>().AttackedFor(attackDamageValue, repeats, appliedConditions));
            }
            if (targetsLeft == 0)
            {
                if (isAttacking)
                {
                    for (int i = 0; i < conditions.Count; i++)
                    {
                        if (conditions[i].ConditionName == "Vigor")
                        {
                            yield return StartCoroutine(conditions[i].OnLoss(this));
                            conditions.RemoveAt(i);
                            yield return StartCoroutine(deckManager.UpdateCardsDisplay());
                            yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
                        }
                    }
                }
                EndAction();
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
                EndAction();
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

    public IEnumerator StartTurn()
    {
        //Debug.Log("started turn");
        if (PlayerTurnStartedFuntions != null)
        {
            PlayerTurnStartedFuntions(this);
        }
        if (PlayerTurnStarted != null)
        {
            yield return StartCoroutine(PlayerTurnStarted(this));
        }
        yield return StartCoroutine(deckManager.DrawNewHand());

        isPlayerTurn = true;
        TopEnergy = 2;
        BottomEnergy = 2;
        yield return StartCoroutine(baseStartTurn());
    }

    public IEnumerator ForceEndTurn()
    {
        if (cardPlayed)
        {
            playedCardScript.StopPlaying = true;
        }
        if (preformingAction)
        {
            ForceEndAction();
        }
        //Debug.Log("ended turn");
        yield return StartCoroutine(EndTurn());
    }
    public IEnumerator AtemptToEndTurn()
    {
        UpdatePlayer();
        if (canEndTurn)
        {
            yield return StartCoroutine(EndTurn());
        }
    }
    public IEnumerator EndTurn()
    {
        UpdatePlayer();
        yield return StartCoroutine(deckManager.DiscardHand());
        isPlayerTurn = false;
        yield return StartCoroutine(pathfinder.BuildPlayerElevationMap());
        yield return StartCoroutine(base.baseEndTurn());
    }
    public void ManualEnd()
    {
        if ((cardPlayed || preformingAbility) && isPlayerTurn)
        {
            EndAction();
            //ActionDone();
        }
    }

    public override void ActionDone()
    {
        //Somthing breaks game but want to convert most actions to EndAction as plans dont go away
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
        if (preformingAction && actionsRemaining.Count > 0)
        {
            actionsRemaining.Remove(actionsRemaining[0]);
            statsDisplayer.Plan(actionsRemaining);
        }
        nextAction = true;
    }

    public void ForceEndAction()
    {
        EndAction();

        actionsRemaining.Clear();
        statsDisplayer.Plan(actionsRemaining);
        nextAction = true;
    }
    public override void EndAction()
    {

        if (actionManager.ActionStackNames.Peek() == "Move")
        {
            //Debug.Log("Ended Move");
            isMoving = false;
            CanJump = false;

            moveLeft = 0;
            if (isMoving && moveCostDisplaySetting == "On Move")
            {
                mapManager.showMoveCost(false);
            }
        }
        else if (actionManager.ActionStackNames.Peek() == "Ability")
        {
            //Debug.Log("Ended Ability");

        }
        else if(actionManager.ActionStackNames.Peek() == "Block")
        {
            //Debug.Log("Ended block");
        }
        else if (actionManager.ActionStackNames.Peek() == "Attack")
        {
            //Debug.Log("Ended Attack");
            isAttacking = false;
            targetsLeft = 0;
        }
        else if (actionManager.ActionStackNames.Peek() == "Condition")
        {
            //Debug.Log("Ended Condition");
        }
        else
        {
            Debug.Log("Unspecified action ended");
        }
        actionManager.ActionStackNames.Pop();
        if (preformingAction && actionsRemaining.Count > 0)
        {
            actionsRemaining.Remove(actionsRemaining[0]);
            statsDisplayer.Plan(actionsRemaining);
        }

        //isTargetATile = false; //?
        //isTargetAEnemy = false; //?
        //nextAction = true; //?
        //actionDone = true; //?
    }



    public IEnumerator ControledMove(int moveValue, bool isJump = false)
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
        yield return new WaitUntil(() => isMoving == false);

    }

    public IEnumerator ControledAttack(int attackValue, int attackRange, int targets, int times, Condition[] attackConditions)
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
        yield return new WaitUntil(() => isAttacking == false);

    }

    public IEnumerator ControledApplyConditions(Condition[] newConditions, string targetType, int conditionsRange, int targets)
    {
        actionDone = false;
        isAppliyingConditions = true;
        targetsLeft = targets;
        range = conditionsRange;
        appliedConditions = newConditions;
        posibleTargets = FindPosibleTargets(targetType, conditionsRange);
        yield return null;
        //yield return StartCoroutine(); // select targets
    }
    public IEnumerator Ability(int abilityValue)
    {
        int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            //string currentDescriptionString = "Ability " + finalAbility;
            string currentDescriptionString = finalAbility + "<sprite name=Ability>";
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            actionManager.ActionStackNames.Push("Ability");
            abilityManager.AbilityPower += finalAbility;
            //abilityManager.SelectedPower += finalAbility;
            yield return StartCoroutine(abilityManager.SetSelectedPower(abilityManager.SelectedPower + finalAbility));
            EndAction();
        }
    }

    public IEnumerator Lockpick(int lockpickValue, bool isVariable = false)
    {
        if (isVariable)
        {

            lockpickValue *= variableCardModifier;
        }
        int finalLockpick = conditionEffects.ModifyAbility(this, lockpickValue);
        //Debug.Log(finalLockpick);
        if (isPlanning)
        {
            string currentDescriptionString = finalLockpick + " <sprite name=Lockpick>";
            actionManager.PlanToList.Add(currentDescriptionString);
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
                //ActionDone();
            }
        }
        yield return null;
    }
    public IEnumerator Draw(int cardCount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Draw " + cardCount + " card";
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            yield return StartCoroutine(deckManager.DrawCards(cardCount));
            //ActionDone();
        }
    }
    public IEnumerator GainEnergy(int amount,bool isTop)
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
            actionManager.PlanToList.Add(currentDescriptionString);
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
            //ActionDone();
        }
        yield return null;
    }
    public IEnumerator GainTopEnergy(int amount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);

        if (isPlanning)
        {
            string currentDescriptionString = "Gain " + amount + " top energy";
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            TopEnergy += amount;
            //ActionDone();
        }
        yield return null;
    }
    public IEnumerator GainBottomEnergy(int amount)
    {
        //int finalAbility = conditionEffects.ModifyAbility(this, abilityValue);
        if (isPlanning)
        {
            string currentDescriptionString = "Gain " + amount + " bottom energy";
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            BottomEnergy += amount;
            //ActionDone();
        }
        yield return null;

    }
    public IEnumerator GainNewAbility(int cost, List<Func<IEnumerator>> abilities, int duration = -1)
    {
        //Debug.Log("start of gaining ablility");
        //Debug.Log(abilities[0]());
        yield return StartCoroutine(GainAbility(new Ability(cost, abilities), duration));
        //Debug.Log("end of gaining ablility");

    }
    public IEnumerator GainAbility(Ability ability, int duration = -1)
    {
        if (isPlanning)
        {
            VariableCardModifier = 1;
            string currentDescriptionString = "Gain ability";
            if (duration == 1)
            {
                currentDescriptionString += " for this turn";
            }
            else if (duration > 1)
            {
                currentDescriptionString += " for " + duration + " turns";
            }
            unmodifiedAction = true;
            string planString = string.Empty;
            yield return StartCoroutine(GetPlanString(ability.Abilities ,(result) => { planString = result; }));
            currentDescriptionString += ": " + ability.Cost + "<sprite name=Ability> for " + planString;
            //Debug.Log(currentDescriptionString);
            //Debug.Log(planString);

            //currentDescriptionString += ": " + ability.Cost + "<sprite name=Ability> for " + GetPlanString(ability.Abilities);
            unmodifiedAction = false;
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            yield return StartCoroutine(abilityManager.GainAbility(ability));
            //ActionDone();
        }
        yield return null;
    }
    public IEnumerator LoseAbility(Ability ability)
    {
        if (isPlanning)
        {
            string planString = string.Empty;
            yield return StartCoroutine(GetPlanString(ability.Abilities, (result) => { planString = result; }));
            string currentDescriptionString = "Lose ability: " + ability.Cost + "<sprite name=Ability> for " + planString;

            //string currentDescriptionString = "Lose ability: " + ability.Cost + "<sprite name=Ability> for " + GetPlanString(ability.Abilities);
            actionManager.PlanToList.Add(currentDescriptionString);
        }
        else
        {
            abilityManager.LoseAbility(ability);
            //ActionDone();
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
        gameManager.EndGame();
        Debug.Log("You Died");
    }

    public void UpdateMoveType()
    {
        ShowMoveCostDisplay();
    }
    public override IEnumerator MoveOneSpace()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].ConditionName == "Untouchable")
            {
                yield return StartCoroutine(actionManager.PreformAction(Block(conditions[i].Value)));
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);
            }
        }
        if (kineticBatteryCount > 0)
        {
            kineticBatterySteps++;
            if (kineticBatterySteps == 3)
            {
                yield return StartCoroutine(actionManager.PreformAction(ApplyCondition(new Vigor(kineticBatteryCount), false)));
                kineticBatterySteps = 0;
            }

            //Debug.Log("Queued kineticBattery");

        }
    }
    public void GainXP(int abount)
    {
        XP += abount;
        while (XP >= XPThreshold)
        {
            PotentialLevelUp();
        }
        statsDisplayer.SetLeveAndXP(level, potentialLevel, XP, XPThreshold);
    }
    public void PotentialLevelUp()
    {
        potentialLevel++;
        XP -= XPThreshold;
        XPThreshold += 2;
        //temp
        LevelUp();
    }

    public IEnumerator LevelUp()
    {
        level++;
        statsDisplayer.SetLeveAndXP(level, potentialLevel, XP, XPThreshold);
        yield return StartCoroutine(rewardManager.LevelUpReward());
    }

}
