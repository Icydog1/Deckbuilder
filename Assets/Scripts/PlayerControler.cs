using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static UnityEngine.GraphicsBuffer;


public class PlayerControler : Figure
{
    private bool actionDone, manualEnd;
    private bool isAttacking, isAppliyingConditions;
    private bool isPlayerTurn, isPlayPhase;
    private GameObject player;
    private RoomSpawner roomSpawner;
    private GameObject clickedTile, clickedFigure;
    private GameObject playedCard;
    public GameObject PlayedCard { get { return playedCard; } set { playedCard = value; } }
    private VariableDisplayer topEnergyDisplay, bottomEnergyDisplay;
    private RewardManager rewardManager;
    //private GameManager gameManager;
    private AbilityManager abilityManager;

    private string moveCostDisplaySetting;
    public string MoveCostDisplaySetting { set { moveCostDisplaySetting = value; UpdateMoveCostDisplay(); } get { return moveCostDisplaySetting; } }

    //private PlayerStats playerStats;
    private Card playedCardScript;
    public Card PlayedCardScript { get { return playedCardScript; } set { playedCardScript = value; } }

    //private Vector2 playerOneToOneCords;
    //public Vector2 PlayerOneToOneCords { get { return playerOneToOneCords; } }


    //private List<Func<IEnumerator>> currentActionQueue = new List<Func<IEnumerator>>();
    //public List<Func<IEnumerator>> CurrentActionQueue { get { return currentActionQueue; } set { currentActionQueue = value; } }


    private bool canPlayCards, canEndTurn, canPreformActions, cardPlayed, gettingReward, preformingAbility, specialPreformingAction, preformingAction, canPreformAbilities;
    private bool waitUntilVariable;
    private bool choosingTargets, choosingTile;
    public bool CanPlayCards { get { UpdatePlayer(); return canPlayCards; } }
    public bool CanPreformAbilities { get { UpdatePlayer(); return canPreformAbilities; } }
    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; UpdatePlayer(); } }

    public bool SpecialPreformingAction { get { return specialPreformingAction; } set { specialPreformingAction = value; UpdatePlayer(); } }
    public bool PreformingAbility { get { return preformingAbility; } set { preformingAbility = value; UpdatePlayer(); } }

    public bool GettingReward { get { return gettingReward; } set { gettingReward = value; UpdatePlayer(); } }
    private int attackDamageValue, repeats;
    private Condition[] appliedConditions;
    private bool canJump, canMove;
    public bool CanJump { get { UpdatePlayer(); return canMove; } set { canJump = value; UpdateMoveType(); } }
    public bool CanMove { get { UpdatePlayer(); return canMove; } set { canMove = value; } }

    private int range;
    private bool isTargetATile, isTargetAEnemy;
    private GameObject selectedTile;
    private GameObject interactButton;
    private GameObject currentTile;
    public GameObject CurrentTile { get { return currentTile; } set { currentTile = value; } }

    //public GameObject CurrentTile { get { return currentTile; } set { currentTile = value; } }

    private List<Figure> allowedTargets;
    private Figure choosenTarget;

    private List<ActionDescription> actionsRemaining = new List<ActionDescription>();
    public List<ActionDescription> ActionsRemaining { get { return actionsRemaining; } set { actionsRemaining = value; statsDisplayer.Plan(actionsRemaining); } }

    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayText(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayText(bottomEnergy); } }

    public bool NextAction { get { return nextAction; } set { nextAction = value; } }
    public static event Action<PlayerControler> PlayerTurnStartedFuntions;
    public static event Func<PlayerControler, IEnumerator> PlayerTurnStarted;

    public event Action<PlayerControler> OpenedDoorFunc;

    public event Action<PlayerControler> MovedSpaceFunc;
    //public event Func<PlayerControler, IEnumerator> MovedSpaceIEnumerator;

    public event Action<PlayerControler> KilledEnemyFunc;
    //public event Func<PlayerControler, IEnumerator> KilledEnemyIEnumerator;
    public event Action<PlayerControler> LostHealth;

    public event Action<PlayerControler> StartedAttackingEnemyFunc, DoneAttackingEnemyFunc;

    //private int kineticBatteryCount, kineticBatterySteps;
    //public int KineticBatteryCount { get { return kineticBatteryCount; } set { kineticBatteryCount = value;} }
    private int waxHandCount;
    public int WaxHandCount { get { return waxHandCount; } set { waxHandCount = value; } }
    private int mortarCount;
    public int MortarCount { get { return mortarCount; } set { mortarCount = value; } }
    //private int adaptiveShieldCount;
    //public int AdaptiveShieldCount { get { return adaptiveShieldCount; } set { adaptiveShieldCount = value; } }
    //private int crackedOpalCount;
    //public int CrackedOpalCount { get { return crackedOpalCount; } set { crackedOpalCount = value; } }
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
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        abilityManager = GameObject.Find("AbilityManager").GetComponent<AbilityManager>();
        //interactButton.SetActive(false);
        interactButton = RefrenceStorage.interactButton;
        base.Awake();
    }

    public override void Start()
    {

        isPlayer = true;
        team = 0;
        //Debug.Log(playerStats);
        GameManager.LateGameStarted += PreparePlayer;
        GameManager.ResetGame += ResetPlayer;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

        //if (isMoving && !isPreformingAnimation)
        //{
        //    if (mouseManager.SelectedObject)
        //    {
        //        if (Input.GetMouseButton(0))
        //        {
        //            if (selectedTile != mouseManager.SelectedObject)
        //            {
        //                selectedTile = mouseManager.SelectedObject;
        //                if (selectedTile.GetComponent<Tile>())
        //                {
        //                    PlanMove(selectedTile);
        //                }
        //            }
        //        }
        //        //if (Input.GetMouseButtonUp(0))
        //        //{
        //        //    if (mouseManager.SelectedObject.GetComponent<Tile>())
        //        //    {
        //        //        StartCoroutine(MoveAlongPath());
        //        //    }
        //        //}
        //    }

        //}
        if (Input.GetKeyDown(KeyCode.K))
        {
            //dev mode
            StartCoroutine(CheatMode());
        }
    }
    public IEnumerator CheatMode()
    {
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Move(1000, true, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Block(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Attack(1000, 100, 1, 1, null, true) })));
    }

    public override void resetBlock()
    {
        block = Mathf.Min(block, Variables.waxHandRetainedBlock * waxHandCount);
    }

    public IEnumerator GoUpLevel()
    {
        player.transform.position = new Vector3(0, 0, player.transform.position.z);
        playerControler.OneToOnePos = Vector2.zero;
        //yield return StartCoroutine(("Vigor"));
        yield break;
    }
    public void ResetPlayer(GameManager gameManager)
    {
        //cardPlayed = false;
        preformingAbility = false;

        waxHandCount = 0;
        interactButton.SetActive(true);
        conditions.Clear();
    }
    public IEnumerator PreparePlayer(GameManager gameManager)
    {
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(2, new List<Func<IEnumerator>>() { () => Move(1, false, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1, true) })));
        level = 0;
        potentialLevel = 1;
        XP = 0;
        XPThreshold = 10;
        maxHealth = 100;
        statsDisplayer.SetLevelAndXP(level, potentialLevel, XP, XPThreshold);
        health = maxHealth;
        transform.position = new Vector3(0, 0, transform.position.z);
        oneToOnePos = Vector2.zero;
        currentTile = mapManager.GetTileAtHex(oneToOnePos);
        statsDisplayer.SetHealthAndBlock(health, maxHealth, 0);
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
        statsDisplayer.Plan(actionsRemaining);
        //statsDisplayer.SetConditions(new string[0]);
    }

    public void TileClicked(GameObject tile)
    {
        if (choosingTile)
        {
            clickedTile = tile;
            choosingTile = false;
        }
        //else if (isTargetATile && canPreformActions)
        //{
        //    clickedTile = tile;

        //    //Vector2 clickedTileCords = clickedTile.transform.position;
        //    //if (isMoving)
        //    //{
        //    //    //AttemptToMove(tile, clickedTileCords);
        //    //}
        //}
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
            //Debug.Log("Come Back updateing move left while moving");
            //foreach (ActionModifier actionModifier in actionsRemaining[0].ActionModifiers)
            //{
            //    if (actionModifier.Type == "Move")
            //    {
            //        actionModifier.ModifiedValue == moveLeft;
            //        statsDisplayer.Plan(actionsRemaining);
            //        break;
            //    }
            //}
            statsDisplayer.ChangePlan("<sprite name=Move>", moveLeft);
            //actionsRemaining[0].GetDescription(); = Regex.Replace(actionsRemaining[0], "(Move)( )([0-9]+)", "$1 " + moveLeft);
        }
        //statsDisplayer.Plan(actionsRemaining);
        currentTile = mapManager.GetTileAtHex(oneToOnePos);
        if (currentTile.GetComponent<Interactable>())
        {
            interactButton.SetActive(true);
        }
        else
        {
            mouseManager.MouseOffObject(interactButton);
            interactButton.SetActive(false);
        }
        if (currentTile.GetComponent<Door>())
        {
            roomSpawner.SpawnRoomsNextToDoor(currentTile, currentTile.GetComponent<Door>().RoomNextToCords);
            //updates current tile as it is no longer a door
            currentTile = mapManager.GetTileAtHex(oneToOnePos);
            if (OpenedDoorFunc != null)
            {
                OpenedDoorFunc(this);
            }
        }
        //way to make it so enemies do spawning stuff without having to go through a long chain of coroutines
        //loads enemies that spawned
        //also preformes all queued actions from moving
        yield return StartCoroutine(actionManager.PreformPreparedActions());
        if (currentTile.GetComponent<Stair>())
        {
            yield return StartCoroutine(levelManager.GoUpLevel());
            currentTile = mapManager.GetTileAtHex(oneToOnePos);
            yield return StartCoroutine(actionManager.PreformPreparedActions());
        }
        else if (moveLeft <= 0)
        {
            isMoving = false;
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
                isMoving = false;
            }
        }
    }


    public IEnumerator FigureClicked(GameObject figure)
    {
        //Debug.Log("clicked" + figure);
        clickedFigure = figure;
        Figure clickedFigureScript = figure.GetComponent<Figure>();
        if (choosingTargets && allowedTargets.Contains(clickedFigureScript))
        {
            choosenTarget = clickedFigureScript;
            choosingTargets = false;
        }
        else if (canPreformActions && isAttacking)
        {
            if (allowedTargets.Contains(clickedFigureScript) && targetsLeft > 0)
            {
                targetsLeft--;
                allowedTargets.Remove(clickedFigureScript);
                effectedFigures.Add(clickedFigureScript);
                if (StartedAttackingEnemyFunc != null)
                {
                    StartedAttackingEnemyFunc(this);
                }
                yield return gameManager.StartCoroutine(clickedFigureScript.AttackedFor(this, attackDamageValue, repeats, appliedConditions));
                if (DoneAttackingEnemyFunc != null)
                {
                    DoneAttackingEnemyFunc(this);
                }
            }
            if (targetsLeft == 0)
            {
                //Debug.Log("ended attack");
                EndAction();
            }
        }
        else if (canPreformActions && isAppliyingConditions)
        {
            if (allowedTargets.Contains(clickedFigureScript) && targetsLeft > 0)
            {
                effectedFigures.Add(clickedFigureScript);

                yield return StartCoroutine(clickedFigureScript.GainConditions(appliedConditions));
                targetsLeft--;
                allowedTargets.Remove(clickedFigureScript);
            }
            if (targetsLeft == 0)
            {
                EndAction();
            }
        }
        else if (figure.GetComponent<AIFigure>())
        {
            StartCoroutine(figure.GetComponent<AIFigure>().DisplayMovePosibilities());
        }

    }

    //public IEnumerator ControledMove(int moveValue, bool isJump = false)
    //{
    //    actionDone = false;
    //    isMoving = true;
    //    isTargetATile = true;
    //    moveLeft = moveValue;
    //    CanJump = isJump;
    //    UpdateMoveCostDisplay();
    //    yield return new WaitUntil(() => isMoving == false);

    //}
    public void UpdateMoveCostDisplay()
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
    //public IEnumerator ControledAttack(int attackValue, int attackRange, int targets, int times, Condition[] attackConditions)
    //{
    //    actionDone = false;
    //    isAttacking = true;
    //    targetsLeft = targets;
    //    attackDamageValue = attackValue;
    //    range = attackRange;
    //    repeats = times;
    //    isTargetAEnemy = true;
    //    appliedConditions = attackConditions;
    //    allowedTargets = FindPosibleTargets("enemy", attackRange);
    //    yield return new WaitUntil(() => isAttacking == false);

    //}

    //public IEnumerator ControledApplyConditions(Condition[] newConditions, string targetType, int conditionsRange, int targets)
    //{
    //    actionDone = false;
    //    isAppliyingConditions = true;
    //    targetsLeft = targets;
    //    range = conditionsRange;
    //    appliedConditions = newConditions;
    //    allowedTargets = FindPosibleTargets(targetType, conditionsRange);
    //    //yield return null;
    //    yield return new WaitUntil(() => isAppliyingConditions == false);

    //    //yield return StartCoroutine(); // select targets
    //}
    public IEnumerator ControledChooseFigures(List<Figure> posibleTargets, System.Action<Figure> callback)
    {
        //actionDone = false;
        //isAppliyingConditions = true;
        allowedTargets = posibleTargets;
        choosingTargets = true;
        yield return new WaitUntil(() => choosingTargets == false);
        callback?.Invoke(choosenTarget);
    }
    public IEnumerator ControledChooseTile(System.Action<GameObject> callback)
    {
        //actionDone = false;
        //isAppliyingConditions = true;
        choosingTile = true;
        yield return new WaitUntil(() => choosingTile == false);
        callback?.Invoke(clickedTile);
    }

    public void UpdatePlayer()
    {
        if (!cardPlayed && !gettingReward && isPlayPhase && !deckManager.IsDisplayingCards && !isPreformingAnimation && !preformingAbility)
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
        if (cardPlayed || preformingAbility || specialPreformingAction)
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
        isPlayerTurn = true;
        if (PlayerTurnStartedFuntions != null)
        {
            PlayerTurnStartedFuntions(this);
        }
        if (PlayerTurnStarted != null)
        {
            Delegate[] listeners = PlayerTurnStarted.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<PlayerControler, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
            //yield return StartCoroutine(GameStarted(this));
        }
        yield return StartCoroutine(deckManager.DrawNewHand());
        TopEnergy = 2;
        BottomEnergy = 2;
        yield return StartCoroutine(baseStartTurn());
        isPlayPhase = true;
    }

    public IEnumerator ForceEndTurn()
    {
        UpdatePlayer();
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
        isPlayPhase = false;
        UpdatePlayer();
        yield return StartCoroutine(deckManager.DiscardHand());
        yield return StartCoroutine(pathfinder.BuildPlayerElevationMap());
        yield return StartCoroutine(base.baseEndTurn());
        isPlayerTurn = false;
    }
    public void ManualEnd()
    {
        UpdatePlayer();
        if (preformingAction && isPlayerTurn)
        {
            //targetsLeft = 0;
            EndAction();
            //ActionDone();
        }
    }

    public override void ActionDone()
    {
        //Somthing breaks game but want to convert most actions to EndAction as plans dont go away
        //isMoving = false;
        //CanJump = false;
        //ShowMoveCostDisplay();
        //isAttacking = false;
        //actionDone = true;
        //isTargetATile = false;
        //isTargetAEnemy = false;
        //if (preformingAction && actionsRemaining.Count > 0)
        //{
        //    actionsRemaining.Remove(actionsRemaining[0]);
        //    statsDisplayer.Plan(actionsRemaining);
        //}
        //nextAction = true;
        Debug.Log("oldsytem");
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
        actionEnded = true;
        string actionName = actionManager.ActionStackNames.Peek();
        //Debug.Log("Ended " + actionName);
        if (actionName == "Move")
        {
            //Debug.Log("Ended Move");
            isMoving = false;
            choosingTile = false;
            canJump = false;
            moveLeft = 0;
            //if (isMoving && moveCostDisplaySetting == "On Move")
            //{
            //    mapManager.showMoveCost(false);
            //}
            UpdateMoveCostDisplay();

        }
        else if (actionName == "Attack")
        {
            //Debug.Log("Ended Attack");
            isAttacking = false;
            choosingTargets = false;
            targetsLeft = 0;

        }
        else if (actionName == "Condition")
        {
            //Debug.Log("Ended condition");
            isAppliyingConditions = false;
            choosingTargets = false;
            targetsLeft = 0;
        }
        else if (actionName == "Augment")
        {
            //Debug.Log("Ended condition");
            choosingTargets = false;
            targetsLeft = 0;
        }
        else if (actionName == "Command")
        {
            //Debug.Log("Ended condition");
            choosingTargets = false;
            targetsLeft = 0;
        }
        //else if (actionName == "Skill" || actionName == "Block" || actionName == "Lockpick" || actionName == "LoseHealth")
        //{

        //}
        //else
        //{
        //    Debug.Log("Unspecified action ended");
        //}
        actionManager.ActionStackNames.Pop();
        if (actionsRemaining.Count > 0)
        {
            actionsRemaining.Remove(actionsRemaining[0]);
            statsDisplayer.Plan(actionsRemaining);
        }
        //actionsRemaining.Remove(actionsRemaining[0]);
        //statsDisplayer.Plan(actionsRemaining);
        //isTargetATile = false; //?
        //isTargetAEnemy = false; //?
        //nextAction = true; //?
        //actionDone = true; //?
    }

    public void UpdatePlan()
    {
        statsDisplayer.Plan(actionsRemaining);
    }



    public IEnumerator Skill(int skillValue, bool isVariable = false)
    {
        if (isVariable)
        {
            skillValue *= variableCardModifier;
        }
        if (isPlanning)
        {
            //string currentDescriptionString = "Skill " + finalSkill;
            //string currentDescriptionString = finalSkill + "<sprite name=Skill>";
            //actionManager.PlanToList.Add(currentDescriptionString);

            ActionDescription currentAction = new ActionDescription("Skill", new List<ActionModifier>() { new ActionModifier(this, null, skillValue, " <sprite name=Skill>", "Skill") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            int finalSkill = conditionEffects.ModifySkill(this, skillValue);
            actionManager.ActionStackNames.Push("Skill");
            abilityManager.AbilityPower += finalSkill;
            yield return StartCoroutine(abilityManager.SetSelectedPower(abilityManager.SelectedPower + finalSkill));
            EndAction();
        }
    }

    public IEnumerator Lockpick(int lockpickValue, bool isVariable = false)
    {
        if (isVariable)
        {
            lockpickValue *= variableCardModifier;
        }
        //Debug.Log(finalLockpick);
        if (isPlanning)
        {
            //string currentDescriptionString = finalLockpick + " <sprite name=Lockpick>";
            //actionManager.PlanToList.Add(currentDescriptionString);
            ActionDescription currentAction = new ActionDescription("Lockpick", new List<ActionModifier>() { new ActionModifier(this, null, lockpickValue, " <sprite name=Lockpick>", "Lockpick") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            int finalLockpick = conditionEffects.ModifySkill(this, lockpickValue);
            actionManager.ActionStackNames.Push("Lockpick");
            currentTile = mapManager.GetTileAtHex(oneToOnePos);
            if (currentTile.GetComponent<Lootable>())
            {
                yield return StartCoroutine(currentTile.GetComponent<Lootable>().Lockpick(finalLockpick));
            }
            //else
            //{
            //    //ActionDone();
            //    EndAction();
            //}
            EndAction();

        }
        yield break;
    }
    public IEnumerator Draw(int cardCount, bool isVariable = false)
    {
        if (isVariable)
        {

            cardCount *= variableCardModifier;
        }
        if (isPlanning)
        {
            //string currentDescriptionString = "Draw " + cardCount + " card";
            //actionManager.PlanToList.Add(currentDescriptionString);

            ActionDescription currentAction = new ActionDescription("Draw", new List<ActionModifier>() { new ActionModifier(this, "Draw ", cardCount, " card", "Draw") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("Draw");
            yield return StartCoroutine(deckManager.DrawCards(cardCount));
            EndAction();
        }
    }
    public IEnumerator GainTopEnergy(int amount, bool isVariable = false)
    {
        if (isVariable)
        {
            amount *= variableCardModifier;
        }
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("TopEnergy", new List<ActionModifier>() { new ActionModifier(this,"Gain ", amount, " top energy", "TopEnergy") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("TopEnergy");
            TopEnergy += amount;
            EndAction();

            //ActionDone();
        }
        yield break;
    }
    public IEnumerator GainBottomEnergy(int amount, bool isVariable = false)
    {
        if (isVariable)
        {
            amount *= variableCardModifier;
        }
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("BottomEnergy", new List<ActionModifier>() { new ActionModifier(this,"Gain ", amount, " bottom energy", "BottomEnergy") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("BottomEnergy");
            BottomEnergy += amount;
            EndAction();
        }
        yield break;

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
            currentDescriptionString += ": " + ability.Cost + " <sprite name=Skill> for " + planString;
            unmodifiedAction = false;

            ActionDescription currentAction = new ActionDescription("GainAbility", new List<ActionModifier>() { new ActionModifier(this, currentDescriptionString) });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("TopEnergy");
            yield return StartCoroutine(abilityManager.GainAbility(ability));
            EndAction();
            //ActionDone();
        }
        yield break;
    }
    public IEnumerator LoseAbility(Ability ability)
    {
        if (isPlanning)
        {
            string planString = string.Empty;
            yield return StartCoroutine(GetPlanString(ability.Abilities, (result) => { planString = result; }));
            string currentDescriptionString = "Lose ability: " + ability.Cost + "<sprite name=Skill> for " + planString;
            ActionDescription currentAction = new ActionDescription("Ability", new List<ActionModifier>() { new ActionModifier(this, currentDescriptionString) });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            abilityManager.LoseAbility(ability);
            //ActionDone();
        }
    }
    public IEnumerator AddKeyword(string keyWord, int keyWordValue = 1)
    {
        if (isPlanning)
        {
            playedCardScript.CurrentKeywords[keyWord] = keyWordValue;
            //if (keyWord == "Augment")
            //{
            //    effectedFigure = this;
            //}

        }
        else
        {
            actionEnded = false;
            actionManager.ActionStackNames.Push(keyWord);
            if (keyWord == "Augment")
            {
                effectedFigures = new List<Figure>();
                List<Figure> posibleTargets = new List<Figure>(currentSummons);
                targetsLeft = keyWordValue;
                while (targetsLeft > 0)
                {
                    Figure targetedFigure = null;
                    yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                    posibleTargets.Remove(targetedFigure);
                    if (targetsLeft > 0)
                    {
                        effectedFigures.Add(targetedFigure);
                        targetsLeft--;
                    }
                }
                playedCardScript.ActingFigures = new List<Figure>(effectedFigures);
                List<Func<bool>> boolList = new List<Func<bool>>();
                foreach (Figure effectedFigure in effectedFigures)
                {
                    boolList.Add(() => effectedFigure.Exists == false);
                }
                PlayedCardScript.PrepareExhaustAfterPlayed(() => boolList.All(condition => condition()), deckManager.Discard);
            }
            //if (keyWord == "Command")
            //{
            //    effectedFigures = new List<Figure>();
            //    List<Figure> posibleTargets = new List<Figure>(currentSummons);
            //    targetsLeft = keyWordValue;
            //    while (targetsLeft > 0)
            //    {
            //        Figure targetedFigure = null;
            //        yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
            //        posibleTargets.Remove(targetedFigure);
            //        if (targetsLeft > 0)
            //        {
            //            effectedFigures.Add(targetedFigure);
            //            targetsLeft--;
            //        }
            //    }
            //    playedCardScript.ActingFigures = new List<Figure>(effectedFigures);
            //    //List<Func<bool>> boolList = new List<Func<bool>>();
            //    //foreach (Figure effectedFigure in effectedFigures)
            //    //{
            //    //    boolList.Add(() => effectedFigure.Exists == false);
            //    //}
            //    //PlayedCardScript.PrepareExhaustAfterPlayed(() => boolList.All(condition => condition()), deckManager.Discard);
            //}
            if (keyWord == "Exausting")
            {
                int currentShuffles = OverallStatistics.shuffles;
                PlayedCardScript.PrepareExhaustAfterPlayed(() => OverallStatistics.shuffles > currentShuffles + keyWordValue, deckManager.Discard);
            }
            if (!actionEnded)
            {
                EndAction();

            }
        }
        //Debug.Log("done with action");
    }

    //all folowing actiions will have the commanded figures as targets
    public IEnumerator Command(int targets = 1, string targetType = "summon", int range = Variables.gameInfinityValue)
    {
        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            if (targetType == "summon")
            {
                if (targets == 1)
                {
                    actionModifiers.Add(new ActionModifier(this, "Command", valueType: "Targets"));
                }
                else
                {
                    actionModifiers.Add(new ActionModifier(this, "Command ", targets, valueType: "Targets"));
                }
            }
            else if (targetType == "enemy")
            {
                if (targets == 1)
                {
                    actionModifiers.Add(new ActionModifier(this, "Control ", targets, " enemy", valueType: "Targets"));
                }
                else
                {
                    actionModifiers.Add(new ActionModifier(this, "Control ", targets, " enemies", valueType: "Targets"));
                }
            }
            if (range != Variables.gameInfinityValue)
            {
                actionModifiers.Add(new ActionModifier(this, " ", range, " <sprite name=Range>", "Range"));
            }
            ActionDescription currentAction = new ActionDescription("Command", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
            //currentDescription.Insert(0, new ActionDescription("Command", new List<ActionModifier>() { new ActionModifier(playerControler, "Command") }));

        }
        else
        {
            effectedFigures = new List<Figure>();
            List<Figure> posibleTargets = FindPosibleTargets(targetType, range);
            targetsLeft = targets;
            while (targetsLeft > 0)
            {
                Figure targetedFigure = null;
                yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                posibleTargets.Remove(targetedFigure);
                if (targetsLeft > 0)
                {
                    effectedFigures.Add(targetedFigure);
                    targetsLeft--;
                }
            }
            playedCardScript.ActingFigures = new List<Figure>(effectedFigures);
        }


    }

    public IEnumerator Augment(Condition conditions, bool isManual = true)
    {
        if (isPlanning)
        {
            actionAbnormalities.Add("Augment");
            yield return StartCoroutine(ApplyCondition(conditions, "summon", Variables.gameInfinityValue, 1, false, true));
            actionAbnormalities.Remove("Augment");
            playedCardScript.CurrentKeywords["Augment"] = 1;
            //ActionDescription currentAction = new ActionDescription("Summon", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summon.name) });
        }
        else if (!isPreparingMove)
        {
            //ActionDone();
            yield return StartCoroutine(ApplyCondition(conditions,"summon",Variables.gameInfinityValue, 1 ,false,true));
            List<Func<bool>> conditionList = new List<Func<bool>>();
            foreach (Figure effectedFigure in effectedFigures)
            {
                conditionList.Add(() => effectedFigure.Exists == false);
            }
            PlayedCardScript.PrepareExhaustAfterPlayed(() => conditionList.All(condition => condition()), deckManager.Discard);

            //            foreach (Figure effectedFigure in effectedFigures)
            //            {

            //                conditionList.Add(() => effectedFigure.exists == false);

            //            }

            //            // Example conditions
            //            conditionList.Add(() => PlayerHasEnoughGold());
            //            // Check if every function returns true
            //            if (conditions.All(() => effectedFigure.exists == false))
            //            {
            //                Debug.Log("All conditions met! Proceeding...");
            //            }
            //        // Invokes each function and returns true only if ALL return true
            //        conditionList.All(func => func != null && func());
        }
        


        //if (isVariable)
        //{
        //    blockValue *= variableCardModifier;
        //}
        //int finalBlock = conditionEffects.ModifyBlock(this, blockValue);


    }
    //int x = 0;
    public IEnumerator Exhausting(int duration)
    {
        if (isPlanning)
        {
            playedCardScript.CurrentKeywords["Exhausting"] = duration;
            //ActionDescription currentAction = new ActionDescription("Summon", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summon.name) });
        }
        else if (!isPreparingMove)
        {
            int currentShuffles = OverallStatistics.shuffles;
            PlayedCardScript.PrepareExhaustAfterPlayed(() => OverallStatistics.shuffles > currentShuffles + duration, deckManager.Discard);
        }
        yield break;
    }




    public override IEnumerator LoseHealth(int amount)
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("LoseHealth", new List<ActionModifier>() { new ActionModifier(this, "Lose ", amount, " health", "LoseHealth") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("LoseHealth");
            yield return StartCoroutine(base.LoseHealth(amount));
            if (LostHealth != null)
            {
                LostHealth(this);
            }
            EndAction();

        }
        yield break;


        //if (adaptiveShieldCount > 0)
        //{
        //    //actionManager.QueueAction(playerControler.ApplyCondition(new StartOfTurnBlock(Variables.adaptiveShieldBlock * adaptiveShieldCount)));
        //    //Debug.Log("queued block nexty turn");
        //    //playerControler.ApplyCondition(new StartOfTurnSlow(Variables.frozenLensSpeedLoss, -1)), relicDescriptionList))
        //    //yield return StartCoroutine(actionManager.QueueAction(Block(Variables.adaptiveShieldBlock * adaptiveShieldCount)));
        //}

    }


    public override IEnumerator Die()
    {
        gameManager.EndGame();
        //Debug.Log("You Died");
        yield break;
    }

    public void UpdateMoveType()
    {
        UpdateMoveCostDisplay();
    }
    public override IEnumerator MoveOneSpace()
    {
        if (MovedSpaceFunc != null)
        {
            MovedSpaceFunc(this);
        }
        //if (MovedSpace != null)
        //{
        //    yield return StartCoroutine(MovedSpace(this));
        //}
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].ConditionName == "Untouchable")
            {
                unmodifiedAction = true;
                actionManager.PrepareAction(Block(conditions[i].Value));
                unmodifiedAction = false;
                //yield return StartCoroutine(actionManager.PreformAction(Block(conditions[i].Value)));
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);
            }
        }
        //if (kineticBatteryCount > 0)
        //{
        //    kineticBatterySteps++;
        //    if (kineticBatterySteps == Variables.kineticBatterySpaces)
        //    {
        //        actionManager.PrepareAction(ApplyCondition(new Vigor(kineticBatteryCount, Variables.kineticBatteryVigorDuration), "self", 1, 1, false, false));
        //        //yield return StartCoroutine(actionManager.PreformAction(ApplyCondition(new Vigor(kineticBatteryCount, Variables.kineticBatteryVigorDuration), "self", 1, 1, false, false)));

        //        kineticBatterySteps = 0;
        //    }

        //    //Debug.Log("Queued kineticBattery");
        //}
        yield break;
    }
    public IEnumerator KilledEnemy(int XPValue)
    {
        //if (crackedOpalCount > 0)
        //{
        //    yield return StartCoroutine(GainBottomEnergy(crackedOpalCount));
        //}
        if (KilledEnemyFunc != null)
        {
            KilledEnemyFunc(this);
        }
        GainXP(XPValue);
        yield break;
    }
    public void GainXP(int amount)
    {
        XP += amount;
        while (XP >= XPThreshold)
        {
            PotentialLevelUp();
        }
        statsDisplayer.SetLevelAndXP(level, potentialLevel, XP, XPThreshold);
    }
    public void PotentialLevelUp()
    {
        potentialLevel++;
        XP -= XPThreshold;
        XPThreshold += 2;
    }

    public IEnumerator LevelUp()
    {
        level++;
        statsDisplayer.SetLevelAndXP(level, potentialLevel, XP, XPThreshold);
        yield return StartCoroutine(rewardManager.LevelUpReward());
    }

}
