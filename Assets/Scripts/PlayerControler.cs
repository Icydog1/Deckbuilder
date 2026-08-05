using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerControler : Figure
{
    private bool actionDone, manualEnd;
    private bool isAttacking, isAppliyingConditions;
    private bool isPlayerTurn, isPlayPhase;
    private GameObject player;
    private Character character;
    private string characterName;
    private bool isHunter, isTinkerer, isWizard;
    private RoomSpawner roomSpawner;
    private GameObject playedCard;
    public GameObject PlayedCard { get { return playedCard; } set { playedCard = value; } }
    private VariableDisplayer topEnergyDisplay, bottomEnergyDisplay, manaDisplay;
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
    private bool unskippableAction;
    private bool choosingTargets, choosingTile, choosingCard;
    private GameObject clickedTile, clickedFigure, clickedCard;
    private List<Figure> allowedTargets;
    private List<GameObject> allowedCards;
    public bool CanPlayCards { get { UpdatePlayer(); return canPlayCards; } }
    public bool CanPreformAbilities { get { UpdatePlayer(); return canPreformAbilities; } }
    public bool CardPlayed { get { return cardPlayed; } set { cardPlayed = value; UpdatePlayer(); } }

    public bool SpecialPreformingAction { get { return specialPreformingAction; } set { specialPreformingAction = value; UpdatePlayer(); } }
    public bool PreformingAbility { get { return preformingAbility; } set { preformingAbility = value; UpdatePlayer(); } }

    public bool GettingReward { get { return gettingReward; } set { gettingReward = value; UpdatePlayer(); } }
    private int attackDamageValue, repeats;
    private Condition[] appliedConditions;
    private bool canMove;
    public bool CanMove { get { UpdatePlayer(); return canMove; } set { canMove = value; } }

    private bool planningMove;
    public bool PlanningMove { get { return planningMove; } set { planningMove = value; } }

    private int range;
    private bool isTargetATile, isTargetAEnemy;
    private GameObject selectedCard;
    private GameObject interactButton;

    private Figure choosenTarget;

    private List<ActionDescription> actionsRemaining = new List<ActionDescription>();
    public List<ActionDescription> ActionsRemaining { get { return actionsRemaining; } set { actionsRemaining = value; statsDisplayer.Plan(actionsRemaining); } }


    private int topEnergy, bottomEnergy;
    public int TopEnergy { get { return topEnergy; } set { topEnergy = value; topEnergyDisplay.DisplayVariable(topEnergy); } }
    public int BottomEnergy { get { return bottomEnergy; } set { bottomEnergy = value; bottomEnergyDisplay.DisplayVariable(bottomEnergy); } }
    private int mana;
    public int Mana { get { return mana; } set { mana = value; UpdateManaDisplay(); } }
    private int manaCapacity;
    public int ManaCapacity { get { return manaCapacity; } set { manaCapacity = value; UpdateManaDisplay(); } }
    //baseValus that valuse reset to at the start of the turn
    private int startingCards, startingTopEnergy, startingBottomEnergy;
    public int StartingCards { get { return startingCards; } set { startingCards = value;} }
    public int StartingTopEnergy { get { return startingTopEnergy; } set { startingTopEnergy = value; } }
    public int StartingBottomEnergy { get { return startingBottomEnergy; } set { startingBottomEnergy = value; } }

    //public bool NextAction { get { return nextAction; } set { nextAction = value; } }
    public event Action<PlayerControler> PlayerTurnStartedFuntions;
    public event Func<PlayerControler, IEnumerator> PlayerTurnStarted;
    public event Func<PlayerControler, IEnumerator> PlayerTurnEnded;
    public event Action<PlayerControler> PlayerTurnEndedFunc;

    public event Action<PlayerControler> OpenedDoorFunc;

    public event Action<PlayerControler> MovedSpaceFunc;
    //public event Func<PlayerControler, IEnumerator> MovedSpaceIEnumerator;

    public event Action<PlayerControler> KilledEnemyFunc;
    //public event Func<PlayerControler, IEnumerator> KilledEnemyIEnumerator;
    public event Action<PlayerControler> LostHealth;

    public event Action<PlayerControler> StartedAttackingEnemyFunc, DoneAttackingEnemyFunc;
    public event Action<PlayerControler> CommandedEnemy;
    public event Action<PlayerControler> CommandedAlly;

    //private int kineticBatteryCount, kineticBatterySteps;
    //public int KineticBatteryCount { get { return kineticBatteryCount; } set { kineticBatteryCount = value;} }
    private int waxHandCount;
    public int WaxHandCount { get { return waxHandCount; } set { waxHandCount = value; } }
    private int minutureMortarCount;
    public int MinutureMortarCount { get { return minutureMortarCount; } set { minutureMortarCount = value; } }
    private int enchantedBoltsCount;
    public int EnchantedBoltsCount { get { return enchantedBoltsCount; } set { enchantedBoltsCount = value; } }
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
        manaDisplay = GameObject.Find("ManaDisplay").GetComponent<VariableDisplayer>();
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
        actionManager.ActiveFigure = this;
        //Debug.Log(playerStats);
        GameManager.LateGameStarted += PreparePlayer;
        GameManager.ResetGame += ResetPlayer;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //dev mode
            StartCoroutine(CheatMode());
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //dev mode
            actionManager.QueueAction(CheatCard());
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            //dev mode
            actionManager.QueueAction(CheatRelic());
            //StartCoroutine(CheatRelic());
        }
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    yield return StartCoroutine(actionManager.PreformAction(Attack(1000, 100, 100,manualOverride:true)));
        //}
    }
    //give player overpowerd abilities which helps for debuging
    public IEnumerator CheatMode()
    {
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Move(1000, true, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Block(1000, true) })));
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Attack(1000, 100, 100, 1, null, true) })));
    }
    public IEnumerator CheatCard()
    {
        yield return StartCoroutine(rewardManager.CustomReward(true));
    }
    public IEnumerator CheatRelic()
    {        
        yield return StartCoroutine(rewardManager.CustomReward(false));
    }
    //resets the block the player has to base at the start of turn
    public override void resetBlock()
    {
        block = Mathf.Min(block, Var.waxHandRetainedBlock * waxHandCount);
    }
    //moves the player to the correct place when they climb the stairs
    public IEnumerator GoUpFloor()
    {
        player.transform.position = new Vector3(0, 0, player.transform.position.z);
        playerControler.OneToOnePos = Vector2.zero;
        //yield return StartCoroutine(("Vigor"));
        yield break;
    }
    //stuff that happens to reset the player to the base state
    public void ResetPlayer(GameManager gameManager)
    {
        //cardPlayed = false;
        preformingAbility = false;

        waxHandCount = 0;
        minutureMortarCount = 0;
        enchantedBoltsCount = 0;
        interactButton.SetActive(true);
        conditions.Clear();
    }

    //stuff that happens to the player when it first spawns in the world
    public IEnumerator PreparePlayer(GameManager gameManager)
    {
        character = gameManager.CurrentCharacter;
        characterName = character.characterName;
        isHunter = false;
        isTinkerer = false;
        isWizard = false;
        //Debug.Log(characterName);
        yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(1, new List<Func<IEnumerator>>() { () => Lockpick(1, true) })));
        if (characterName == "Hunter")
        {
            isHunter = true;
            yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(2, new List<Func<IEnumerator>>() { () => Move(1, false, true) })));
        }
        else if (characterName == "Tinkerer")
        {
            isTinkerer = true;
            yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(2, new List<Func<IEnumerator>>() { () => Block(1, true) })));
        }
        else if (characterName == "Wizard")
        {
            isWizard = true;
            yield return StartCoroutine(actionManager.PreformAction(GainNewAbility(2, new List<Func<IEnumerator>>() { () => GainMana(1, true) })));
        }
        if (isWizard)
        {
            manaCapacity = 10;
            mana = manaCapacity;
        }
        else
        {
            manaCapacity = 0;
            mana = 0;
        }
        UpdateManaDisplay();
        level = 0;
        potentialLevel = 1;
        XP = 0;
        XPThreshold = Var.initialXPToLevel;
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


    //calculates and hilights the path the player would take attemtping to move to a tile
    public override void PlanMove(GameObject tile)
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
    //moves player along the planned path
    public override IEnumerator MoveAlongPath()
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
        if (actionsRemaining.Count > 0)
        {
            statsDisplayer.ChangePlan("<sprite name=Move>", moveLeft);
        }
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
            OverallStatistics.roomsExplored++;
            if (OpenedDoorFunc != null)
            {
                OpenedDoorFunc(this);
            }
            yield return StartCoroutine(actionManager.PreformPreparedActions());
        }
        //way to make it so enemies do spawning stuff without having to go through a long chain of coroutines
        //loads enemies that spawned
        //also preformes all queued actions from moving
        if (currentTile.GetComponent<Stair>())
        {
            yield return StartCoroutine(floorManager.GoUpFloor());
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

    //shows/hides numbers on every tile that is their move cost depenting on the player setting
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

    //runs when a figure is clicked
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
        else if (figure.GetComponent<AIFigure>())
        {
            StartCoroutine(figure.GetComponent<AIFigure>().DisplayMovePosibilities());
        }
        yield break;
    }
    //runs when a tile is clicked
    public void TileClicked(GameObject tile)
    {
        if (choosingTile)
        {
            clickedTile = tile;
            choosingTile = false;
        }
    }
    //runs when a card is clicked
    public void CardClicked(GameObject card)
    {
        if (choosingCard && allowedCards.Contains(card))
        {
            clickedCard = card;
            choosingCard = false;
        }
    }

    //waits until a figure from a list is clicked and returns that figure
    public IEnumerator ControledChooseFigures(List<Figure> posibleTargets, System.Action<Figure> callback)
    {
        //actionDone = false;
        //isAppliyingConditions = true;
        allowedTargets = posibleTargets;
        choosingTargets = true;
        yield return new WaitUntil(() => choosingTargets == false);
        callback?.Invoke(choosenTarget);
    }
    //waits until a tile is clicked and returns that tile
    public IEnumerator ControledChooseTile(System.Action<GameObject> callback)
    {
        //actionDone = false;
        //isAppliyingConditions = true;
        choosingTile = true;
        yield return new WaitUntil(() => choosingTile == false);
        callback?.Invoke(clickedTile);
    }
    //waits until a card from a list is clicked and returns that card
    public IEnumerator ControledChooseCard(List<GameObject> posibleTargets, System.Action<GameObject> callback)
    {
        //actionDone = false;
        //isAppliyingConditions = true;
        allowedCards = posibleTargets;
        choosingCard = true;
        yield return new WaitUntil(() => choosingCard == false);
        callback?.Invoke(clickedCard);
    }
    //updates player variables based on other variables
    public void UpdatePlayer()
    {
        if (!cardPlayed && !gettingReward && isPlayPhase && !deckManager.IsDisplayingList && !isPreformingAnimation && !preformingAbility)
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
        if (!gettingReward && isPlayerTurn && !deckManager.IsDisplayingList && !isPreformingAnimation)
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
    //runs at the start of the turn
    public IEnumerator StartTurn()
    {
        //Debug.Log("started turn");
        startingCards = 5;
        startingTopEnergy = 2;
        startingBottomEnergy = 2;
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
        yield return StartCoroutine(baseStartTurn());
        startingCards = Mathf.Clamp(startingCards, 0, 10);
        startingTopEnergy = Global.Clamp(startingTopEnergy,0);
        startingBottomEnergy = Global.Clamp(startingBottomEnergy,0);

        yield return StartCoroutine(deckManager.DrawCards(startingCards));
        TopEnergy = startingTopEnergy;
        BottomEnergy = startingBottomEnergy;
        if (mana < manaCapacity)
        {
            //regenerates 20% of mana capacity each turn rounded down
            mana += Mathf.FloorToInt(manaCapacity * Var.manaRegenPercentage);
            mana = Mathf.Min(mana, manaCapacity);
        }
        else if (mana > manaCapacity)
        {
            //removes 20% of mana that is above capacity each turn rounded up
            mana -= Mathf.CeilToInt((mana - manaCapacity) * Var.manaLossPercentage);
            mana = Mathf.Max(mana, manaCapacity);
        }
        UpdateManaDisplay();
        isPlayPhase = true;
    }

    //forces the player to immediatly end the turn
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
    //attemts to end the turn if able
    public IEnumerator AtemptToEndTurn()
    {
        UpdatePlayer();
        if (canEndTurn)
        {
            yield return StartCoroutine(EndTurn());
        }
    }
    //runs when the turn ends
    public IEnumerator EndTurn()
    {
        isPlayPhase = false;
        if (PlayerTurnEndedFunc != null)
        {
            PlayerTurnEndedFunc(this);
        }
        if (PlayerTurnEnded != null)
        {
            Delegate[] listeners = PlayerTurnEnded.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<PlayerControler, IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback(this));
            }
            //yield return StartCoroutine(GameStarted(this));
        }
        UpdatePlayer();
        yield return StartCoroutine(deckManager.DiscardHand());
        yield return StartCoroutine(pathfinder.BuildPlayerElevationMap());
        yield return StartCoroutine(base.baseEndTurn());
        isPlayerTurn = false;
    }
    //ends a action before it is complete if able
    public void ManualEnd()
    {
        UpdatePlayer();
        if (preformingAction && isPlayerTurn && !unskippableAction)
        {
            //targetsLeft = 0;
            EndAction();
            //ActionDone();
        }
    }

    public override void ActionDone()
    {
        Debug.Log("oldsytem");
    }
    //force ends whatever action the player is currently doing
    public void ForceEndAction()
    {
        if (!actionManager.ActionEnded)
        {
            EndAction();
            actionsRemaining.Clear();
            statsDisplayer.Plan(actionsRemaining);
        }

        //nextAction = true;
    }
    //the prosses of ending a action
    public override void EndAction()
    {
        if (!actionManager.ActionEnded)
        {
            //Debug.Log("ended action");
            string actionName = actionManager.ActionStackNames.Peek();
            //Debug.Log("Ended " + actionName);
            if (actionName == "Move")
            {
                //Debug.Log("Ended Move");
                isMoving = false;
                planningMove = false;
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
            if (actionManager.ActiveFigure && actionManager.ActiveFigure != this)
            {
                actionManager.ActiveFigure.EndAction();
                //Debug.Log(actionManager.ActiveFigure);
            }
            else
            {
                actionManager.ActionStackNames.Pop();
                //Debug.Log("actionsRemaining = " + actionsRemaining.Count);
                if (actionsRemaining.Count > 0)
                {
                    //Debug.Log("removed action plan");
                    actionsRemaining.Remove(actionsRemaining[0]);
                    statsDisplayer.Plan(actionsRemaining);
                }
                //Debug.Log("actionsRemaining = " + actionsRemaining.Count);
            }
            actionManager.ActionEnded = true;
            //actionsRemaining.Remove(actionsRemaining[0]);
            //statsDisplayer.Plan(actionsRemaining);
            //isTargetATile = false; //?
            //isTargetAEnemy = false; //?
            //nextAction = true; //?
            //actionDone = true; //?
        }
    }
    //updates what the plan that is displayed says
    public void UpdatePlan()
    {
        statsDisplayer.Plan(actionsRemaining);
    }

    //action to gain skill
    public IEnumerator Skill(int skillValue, bool isVariable = false)
    {
        if (isVariable)
        {
            skillValue *= variableCardModifier;
        }
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("Skill", new List<ActionModifier>() { new ActionModifier(this, "<sprite name=Skill>", skillValue, null, "Skill") });
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
    //action to gain lockpick
    public IEnumerator Lockpick(int lockpickValue, bool isVariable = false)
    {
        if (isVariable)
        {
            lockpickValue *= variableCardModifier;
        }
        //Debug.Log(finalLockpick);
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("Lockpick", new List<ActionModifier>() { new ActionModifier(this, "<sprite name=Lockpick>", lockpickValue, null, "Lockpick") });
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
    //action to draw cards
    public IEnumerator Draw(int cardCount, bool isVariable = false)
    {
        if (isVariable)
        {

            cardCount *= variableCardModifier;
        }
        if (isPlanning)
        {
            string pluralCards = " card";
            if (cardCount != 1)
            {
                pluralCards = " cards";
            }
            ActionDescription currentAction = new ActionDescription("Draw", new List<ActionModifier>() { new ActionModifier(this, "Draw ", cardCount, pluralCards, "Draw") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("Draw");
            yield return StartCoroutine(deckManager.DrawCards(cardCount));
            EndAction();
        }
    }
    //action to discard cards

    public IEnumerator Discard(int cardCount, bool isVariable = false)
    {
        if (isVariable)
        {
            cardCount *= variableCardModifier;
        }
        if (isPlanning)
        {
            if (cardCount == Var.infinityValue)
            {
                ActionDescription currentAction = new ActionDescription("Discard", new List<ActionModifier>() { new ActionModifier(this, "Discard your hand", valueType:"Discard") });
                actionManager.PlanToList.Add(currentAction);
            }
            else
            {

                string pluralCards = " card";
                if (cardCount != 1)
                {
                    pluralCards = " cards";
                }
                ActionDescription currentAction = new ActionDescription("Discard", new List<ActionModifier>() { new ActionModifier(this, "Discard ", cardCount, pluralCards, "Discard") });
                actionManager.PlanToList.Add(currentAction);
            }

        }
        else
        {
            unskippableAction = true;
            actionManager.ActionStackNames.Push("Discard");
            if (cardCount >= deckManager.HandContents.Count || cardCount == Var.infinityValue)
            {
                yield return StartCoroutine(deckManager.DiscardHand());
                EndAction();
            }
            else
            {
                List<GameObject> posibleTargets = deckManager.HandContents;
                targetsLeft = cardCount;
                while (targetsLeft > 0)
                {
                    GameObject selectedCard = null;
                    yield return StartCoroutine(ControledChooseCard(posibleTargets, (result) => { selectedCard = result; }));
                    posibleTargets.Remove(selectedCard);
                    if (targetsLeft > 0)
                    {
                        targetsLeft--;
                        yield return StartCoroutine(deckManager.DiscardCard(selectedCard));
                        if (targetsLeft <= 0 || posibleTargets.Count == 0)
                        {
                            EndAction();
                        }
                    }
                }
            }
            unskippableAction = false;
            //EndAction();
        }
    }
    //action to gain top energy
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
    //action to gain bottom energy
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
    public IEnumerator GainMana(int amount, bool isVariable = false)
    {
        if (isVariable)
        {
            amount *= variableCardModifier;
        }
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("Mana", new List<ActionModifier>() { new ActionModifier(this, "Gain ", amount, " mana", "Mana") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("Mana");
            Mana += amount;
            EndAction();
        }
        yield break;

    }
    //action to gain a new ability (constructs ability)
    public IEnumerator GainNewAbility(int cost, List<Func<IEnumerator>> abilities, int duration = -1)
    {
        //Debug.Log("start of gaining ablility");
        //Debug.Log(abilities[0]());
        yield return StartCoroutine(GainAbility(new Ability(cost, abilities), duration));
        //Debug.Log("end of gaining ablility");

    }
    //action to gain a new ability (takes constructed ability)
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
            currentDescriptionString += ": <sprite name=Skill>" + ability.Cost + " for " + planString;
            unmodifiedAction = false;

            ActionDescription currentAction = new ActionDescription("GainAbility", new List<ActionModifier>() { new ActionModifier(this, currentDescriptionString) });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("GainAbility");
            yield return StartCoroutine(abilityManager.GainAbility(ability));
            EndAction();
            //ActionDone();
        }
        yield break;
    }
    //action to lose a ability
    public IEnumerator LoseAbility(Ability ability)
    {
        abilityManager.LoseAbility(ability);
        yield break;
    }
    //add keywords to a card
    public IEnumerator AddKeyword(string keyWord, int keyWordValue = 1)
    {
        if (isPlanning)
        {
            playedCardScript.CurrentKeywords[keyWord] = keyWordValue;
            if (keyWord == "Augment")
            {
                ActionDescription currentAction = new ActionDescription("Augment", new List<ActionModifier>() { new ActionModifier(playerControler, "Augment") });
                actionManager.PlanToList.Add(currentAction);
            }
            //if (keyWord == "Upkeep")
            //{
            //    ActionDescription currentAction = new ActionDescription("Upkeep", new List<ActionModifier>() { new ActionModifier(playerControler, "Upkeep") });
            //    actionManager.PlanToList.Add(currentAction);
            //}
        }
        else
        {
            //actionEnded = false;
            actionManager.ActionStackNames.Push(keyWord);
            if (keyWord == "Augment")
            {
                effectedFigures = new List<Figure>();
                List<Figure> posibleTargets = new List<Figure>(currentSummons);
                targetsLeft = keyWordValue;
                while (targetsLeft > 0 && posibleTargets.Count > 0)
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
            //if (keyWord == "Exausting")
            //{
            //    int currentShuffles = OverallStatistics.shuffles;
            //    PlayedCardScript.PrepareExhaustAfterPlayed(() => OverallStatistics.shuffles > currentShuffles + keyWordValue, deckManager.Discard);
            //}
            //if (!actionEnded)
            {
                EndAction();

            }
        }
        //Debug.Log("done with action");
    }

    //action for commanding figures
    //all folowing actiions will have the commanded figures as targets
    public IEnumerator Command(int targets = 1, string targetType = "summon", int range = Var.infinityValue, bool manualOverride = false)
    {
        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            string target = null;
            target = targetType.ToQuantity(targets, ShowQuantityAs.None);
            //if (targetType == "summon")
            //{
            //    if (targets == 1)
            //    {
            //        target = "summon";
            //    }
            //    else
            //    {
            //        target = "summons";
            //    }
            //}
            //else if (targetType == "enemy")
            //{
            //    if (targets == 1)
            //    {
            //        target = "enemy";
            //    }
            //    else
            //    {
            //        target = "enemies";
            //    }
            //}
            if (targets == Var.infinityValue)
            {
                actionModifiers.Add(new ActionModifier(this, "Control <sprite name=Target>all " + target, valueType: "Targets"));
            }
            else
            {
                actionModifiers.Add(new ActionModifier(this, "Control <sprite name=Target>", targets, " " + target, valueType: "Targets"));
            }
            //if (targetType == "summon")
            //{
            //    if (targets == 1)
            //    {
            //        actionModifiers.Add(new ActionModifier(this, "Command", valueType: "Targets"));
            //    }
            //    else
            //    {
            //        actionModifiers.Add(new ActionModifier(this, "Command <sprite name=Target>", targets, valueType: "Targets"));
            //    }
            //}
            //else if (targetType == "enemy")
            //{
            //    if (targets == 1)
            //    {
            //        actionModifiers.Add(new ActionModifier(this, "Control <sprite name=Target>", targets, " enemy", valueType: "Targets"));
            //    }
            //    else if (targets == Var.infinityValue)
            //    {
            //        actionModifiers.Add(new ActionModifier(this, "Control <sprite name=Target>all enemies", valueType: "Targets"));
            //    }
            //    else
            //    {
            //        actionModifiers.Add(new ActionModifier(this, "Control <sprite name=Target>", targets, " enemies", valueType: "Targets"));
            //    }
            //}
            if (range != Var.infinityValue)
            {
                actionModifiers.Add(new ActionModifier(this, " <sprite name=Range>", range, null, "Range"));
            }
            ActionDescription currentAction = new ActionDescription("Command", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
            //currentDescription.Insert(0, new ActionDescription("Command", new List<ActionModifier>() { new ActionModifier(playerControler, "Command") }));

        }
        else
        {
            actionManager.ActionStackNames.Push("Command");
            effectedFigures = new List<Figure>();
            if (!manualOverride)
            {
                List<Figure> posibleTargets = FindPosibleTargets(targetType, range);
                targetsLeft = targets;
                if (targets == Var.infinityValue)
                {
                    targetsLeft = posibleTargets.Count;
                }
                //while (targetsLeft > 0 && posibleTargets.Count > 0)
                //{
                //    Figure targetedFigure = null;
                //    yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                //    posibleTargets.Remove(targetedFigure);
                //    if (targetsLeft > 0)
                //    {
                //        effectedFigures.Add(targetedFigure);
                //        targetsLeft--;
                //    }
                //}
                while (targetsLeft > 0 && posibleTargets.Count > 0)
                {
                    Figure targetedFigure = null;
                    yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                    posibleTargets.Remove(targetedFigure);
                    if (targetsLeft > 0)
                    {
                        effectedFigures.Add(targetedFigure);
                        targetsLeft--;
                        if (targetsLeft <= 0)
                        {
                            EndAction();
                        }
                    }
                }
            }
            else
            {
                foreach (Figure target in FindTargets(targetType, range, targets))
                {
                    //Debug.Log(target);
                    effectedFigures.Add(target);
                }
            }
            //ActionDone();
            if (targetType == "summon")
            {
                for (int i = 0; i < effectedFigures.Count; i++)
                {
                    if (CommandedAlly != null)
                    {
                        CommandedAlly(this);
                    }
                }
            }
            else if (targetType == "enemy")
            {
                for (int i = 0; i < effectedFigures.Count; i++)
                {
                    if (CommandedEnemy != null)
                    {
                        CommandedEnemy(this);
                    }
                }
            }
            foreach (Figure figure in effectedFigures)
            {
                figure.Controled = true;
            }
            EndAction();
            playedCardScript.ActingFigures = new List<Figure>(effectedFigures);
        }
    }
    public IEnumerator StopCommanding()
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("StopCommanding", new List<ActionModifier>() { new ActionModifier(this, "End Command", valueType: "StopCommanding") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("StopCommanding");
            playedCardScript.StopCommanding();
            EndAction();
        }
        yield break;
    }


    public IEnumerator Augment(Condition conditions, bool isManual = true)
    {
        if (isPlanning)
        {
            actionAbnormalities.Add("Augment");
            yield return StartCoroutine(ApplyCondition(conditions, "summon", Var.infinityValue, 1, false, true));
            actionAbnormalities.Remove("Augment");
            playedCardScript.CurrentKeywords["Augment"] = 1;
            //ActionDescription currentAction = new ActionDescription("Summon", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summon.name) });
        }
        else if (!isPreparingMove)
        {
            //ActionDone();
            yield return StartCoroutine(ApplyCondition(conditions,"summon",Var.infinityValue, 1 ,false,true));
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
    //action for exauseting
    public IEnumerator Exhausting(int duration)
    {
        if (isPlanning)
        {
            playedCardScript.CurrentKeywords["Exhausting"] = duration;
        }
        else if (!isPreparingMove)
        {
            actionManager.ActionStackNames.Push("Exhausting");
            int currentShuffles = OverallStatistics.shuffles;
            PlayedCardScript.PrepareExhaustAfterPlayed(() => OverallStatistics.shuffles >= currentShuffles + duration, deckManager.Discard);
            EndAction();
        }
        yield break;
    }




    public override IEnumerator LoseHealthAction(int amount)
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("LoseHealth", new List<ActionModifier>() { new ActionModifier(this, "Lose ", amount, " health", "LoseHealth") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("LoseHealth");
            yield return StartCoroutine(LoseHealth(amount));
            if (LostHealth != null)
            {
                LostHealth(this);
            }
            EndAction();

        }
        yield break;
    }
    public IEnumerator TakeDamageAction(int amount)
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("TakeDamage", new List<ActionModifier>() { new ActionModifier(this, "Take ", amount, " damage", "TakeDamage") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("TakeDamage");
            yield return StartCoroutine(TakeDamage(amount));
            EndAction();

        }
        yield break;
    }
    public override IEnumerator LoseHealth(int amount)
    {

        yield return StartCoroutine(base.LoseHealth(amount));
        if (LostHealth != null)
        {
            LostHealth(this);
        }
    }
    public override IEnumerator Die()
    {
        gameManager.Death();
        //Debug.Log("You Died");
        yield break;
    }

    public void UpdateManaDisplay()
    {
        if (isWizard || mana != 0 || manaCapacity != 0)
        {
            manaDisplay.DisplayString(mana + "/" + manaCapacity);
        }
        else
        {
            manaDisplay.Disable();

        }

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
        //    if (kineticBatterySteps == Var.kineticBatterySpaces)
        //    {
        //        actionManager.PrepareAction(ApplyCondition(new Vigor(kineticBatteryCount, Var.kineticBatteryVigorDuration), "self", 1, 1, false, false));
        //        //yield return StartCoroutine(actionManager.PreformAction(ApplyCondition(new Vigor(kineticBatteryCount, Var.kineticBatteryVigorDuration), "self", 1, 1, false, false)));

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
        XPThreshold += Var.XPToLevelIncrease;
    }

    public IEnumerator LevelUp()
    {
        level++;
        statsDisplayer.SetLevelAndXP(level, potentialLevel, XP, XPThreshold);
        yield return StartCoroutine(rewardManager.LevelUpReward());
    }
}
