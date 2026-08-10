using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Humanizer;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;




public class Figure : MonoBehaviour
{
    protected TurnManager turnManager;
    protected MapManager mapManager;
    protected MouseManager mouseManager;
    protected FigureStats statsDisplayer;
    protected PlayerControler playerControler;
    protected Pathfinder pathfinder;
    protected ConditionEffects conditionEffects;
    protected DeckManager deckManager;
    protected FloorManager floorManager;
    protected ActionManager actionManager;
    protected OverallStatistics overallStatistics;
    protected GameManager gameManager;



    protected bool isMyTurn;
    protected bool isEnemy, isPlayer, isAI, isPlayerSummon;
    public bool IsPlayerSummon { get { return isPlayerSummon; } }

    protected bool isPlanning, isPreparingMove;
    public bool IsPlanning { set { isPlanning = value; } get { return isPlanning; } }

    protected Vector2 hexPos;
    public Vector2 HexPos { get { return hexPos; } set { hexPos = value; } }
    protected GameObject currentTile;
    public GameObject CurrentTile { get { return currentTile; } set { currentTile = value; } }
    protected int preferedRange;
    protected int distanceToFocus;
    //protected bool actionManager.ActionEnded;
    //public bool actionManager.ActionEnded { set { actionManager.ActionEnded = value; } get { return actionManager.ActionEnded; } }

    protected bool isDead, exists = true;
    public bool Exists { set { exists = value; } get { return exists; } }

    protected bool unmodifiedAction;
    public bool UnmodifiedAction { get { return unmodifiedAction; } set { unmodifiedAction = value; } }

    protected int team;
    public int Team { get { return team; } }
    protected bool isPreformingAnimation;
    public bool IsPreformingAnimation { set { isPreformingAnimation = value; } get { return isPreformingAnimation; } }

    protected int maxHealth = 1, health, block = 0;
    public int MaxHealth { get { return maxHealth; } }

    protected bool canFly = false;
    public bool CanFly { set { canFly = value; } get { return canFly; } }
    protected bool canJump;
    public bool CanJump { set { canJump = value; } get { return canJump; } }
    protected int targetsLeft, moveLeft;
    public int TargetsLeft { get { return targetsLeft; } set { targetsLeft = value; } }
    protected bool isMoving;
    public bool IsMoving { set { isMoving = value; } get { return isMoving; } }

    //private List<string> actionManager.PlanToList = new List<string>();
    //public List<string> actionManager.PlanToList { set { actionManager.PlanToList = value; } }
    //protected List<Func<IEnumerator>> prepareActions = new List<Func<IEnumerator>>();
    //public List<Func<IEnumerator>> PrepareActions { set { prepareActions = value; } }

    protected List<Condition> conditions = new List<Condition>();
    public List<Condition> Conditions { set { conditions = value; } get { return conditions; } }


    protected List<GameObject> shownTileBorders = new List<GameObject>();
    protected List<string> actionAbnormalities = new List<string>();
    public List<string> ActionAbnormalities { set { actionAbnormalities = value; } get { return actionAbnormalities; } }

    protected bool isSummon;
    protected Figure summoner;
    public Figure Summoner { get { return summoner; } }

    protected List<Figure> currentSummons = new List<Figure>();
    public List<Figure> CurrentSummons { get { return currentSummons; } }
    protected List<Figure> effectedFigures = new List<Figure>();
    public List<Figure> EffectedFigures { get {  return effectedFigures; }  }
    //public List<Figure> refEffectedFigures { get { return effectedFigures; } }

    protected Figure effectedFigure;
    public Figure EffectedFigure { get { return effectedFigure; } }

    protected bool hasFocus;
    protected GameObject focus;
    protected Figure focusScript;
    protected Vector2 focusPos;
    protected List<Vector2> movePosibilities = new List<Vector2>(), unsafeMovePosibilities = new List<Vector2>();

    //public static event Func<Figure, IEnumerator> Removed;
    public event Func<IEnumerator> Removed;

    public List<Condition> upkeptConditions = new List<Condition>();
    public bool hasUpkeep;
    protected int turn;
    protected bool controled;
    public bool Controled { get { return controled; } set { controled = value; } }
	public event Func<Figure, Figure, IEnumerator> Attacked;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	public virtual void Awake()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        conditionEffects = GameObject.Find("ConditionEffects").GetComponent<ConditionEffects>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();
        overallStatistics = GameObject.Find("OverallStatistics").GetComponent<OverallStatistics>();
        gameManager = RefrenceStorage.gameManager;
        //Debug.Log("figure awake ran");

        //statsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        actionManager.PrepareAction(LoadFigure());
    }
    public virtual void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual IEnumerator LoadFigure()
    {
        health = maxHealth;
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));

        yield break;
    }

    public virtual IEnumerator BaseStartTurn()
    {
        //Debug.Log(gameObject + " is takeing turn");
        turn++;
        ResetBlock();
        yield return StartCoroutine(conditionEffects.StartOfTurnConditions(this));
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].IsStartOfTurn && conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);
            }
            if (conditions[i].IsStartOfTurn && conditions[i].Duration == 0)
            {
                //Debug.Log("removed " + conditions[i].ConditionName + "at start of turn");
                yield return StartCoroutine(conditions[i].OnLoss(this));
                conditions.RemoveAt(i);
                i--;
            }
        }
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
    }
    public virtual void ResetBlock()
    {
        block = 0;
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
    }
    public IEnumerator BaseEndTurn()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (!conditions[i].IsStartOfTurn && conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);

            }
            if (!conditions[i].IsStartOfTurn && conditions[i].Duration == 0)
            {
                //Debug.Log("removed " + conditions[i].Name);
                //Debug.Log("removed " + conditions[i].ConditionName + "at end of turn");

                yield return StartCoroutine(conditions[i].OnLoss(this));
                conditions.RemoveAt(i);
                i--;
            }
        }
        if (isPlayer)
        {
            StartCoroutine(deckManager.UpdateCardsDisplay());

        }
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
        //Debug.Log(gameObject + " is ending turn");
        turnManager.NextTurn();
    }

    public virtual void ActionDone()
    {
        Debug.Log("Base ActionDone ran");
    }
    //public virtual void EndAction()
    //{
    //    Debug.Log("Base EndAction ran");
    //}
    public virtual void EndAction()
    {
        if (!actionManager.ActionEnded)
        {
            string actionName = actionManager.ActionStackNames.Peek();
            if (actionName == "Move")
            {
                isMoving = false;
                canJump = false;
                moveLeft = 0;

            }
            else if (actionName == "Attack")
            {
                //Debug.Log("Ended Attack");
                //isAttacking = false;
                //choosingTargets = false;
                targetsLeft = 0;

            }
            else if (actionName == "Condition")
            {
                targetsLeft = 0;
            }
            actionManager.ActionStackNames.Pop();
            if (playerControler.ActionsRemaining.Count > 0)
            {
                playerControler.ActionsRemaining.Remove(playerControler.ActionsRemaining[0]);
                playerControler.statsDisplayer.Plan(playerControler.ActionsRemaining);
            }
            actionManager.ActionEnded = true;
        }

    }


    public IEnumerator GetPlanString(List<Action> actions, System.Action<string> callback, Ability ability)
    {
        List<ActionDescription> planDescription = new List<ActionDescription>();
        foreach (Action action in actions)
        {
            yield return StartCoroutine(action.PreformAction(ability, planDescription));

            //yield return StartCoroutine(actionManager.PreformAction(action(), planDescription));
        }
        string displayedString = "";
        foreach (ActionDescription text in planDescription)
        {
            //Debug.Log("Come Back to this");

            displayedString += text.GetDescription();
            displayedString += " ";
        }
        //yeild return displayedString;
        //Debug.Log(displayedString);
        //Debug.Log(actionManager.PlanToList[0]);

        callback?.Invoke(displayedString);

    }



    public IEnumerator Block(int blockValue, bool isVariable = false)
    {
        if (isVariable)
        {
            blockValue *= playerControler.VariableCardModifier;
        }
        if (isPlanning)
        {

            ActionDescription currentAction = new ActionDescription("Block", new List<ActionModifier>() { new ActionModifier(this, "<sprite name=Block>", blockValue, null, "Block") });
            actionManager.PlanToList.Add(currentAction);
        }
        else if (!isPreparingMove)
        {
            int finalBlock = conditionEffects.ModifyBlock(this, blockValue);
            actionManager.ActionStackNames.Push("Block");
            block += finalBlock;
            statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
            //ActionDone();
            EndAction();

        }
        yield return null;
    }

    public IEnumerator Attack(int attackValue, int attackRange = 1, int targets = 1, int repeats = 1, Condition[] attackConditions = null, bool isVariable = false, bool manualOverride = false, string targetType = "enemy")
    {
        if (isVariable)
        {
            attackValue *= playerControler.VariableCardModifier;
        }
        if (attackConditions == null)
        {
            attackConditions = new Condition[0];
        }
        int finalAttack = conditionEffects.ModifyAttack(this, attackValue);
        int finalRange = conditionEffects.ModifyRange(this, attackRange);

        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            actionModifiers.Add(new ActionModifier(this, "<sprite name=Attack>", attackValue, null, "Attack"));
            //Debug.Log("Planned Attack");

            //string currentDescriptionStart = "";
            //string currentDescriptionEnd = "";
            if (!isPlayer)
            {
                if (preferedRange > attackRange)
                {
                    preferedRange = attackRange;
                }
            }


            //List<string> individualConditionText = new List<string>();
            if (attackConditions.Length != 0)
            {
                //currentDescriptionStart += " and apply ";
                string conditionString = " and apply ";
                foreach (Condition condition in attackConditions)
                {
                    string currentDescriptionString = condition.Value + " " + condition.ConditionName;
                    if (condition.Duration == 1)
                    {
                        currentDescriptionString += " this turn";
                    }
                    else if (condition.Duration != -1)
                    {
                        currentDescriptionString += " for " + condition.Duration + " turns";
                    }
                    //individualConditionText.Add(currentDescriptionString);
                    conditionString += currentDescriptionString;
                }
                actionModifiers.Add(new ActionModifier(this, conditionString, valueType: "Conditions"));

            }
            if (repeats != 1)
            {
                //currentDescriptionEnd += " " + repeats + " times ";
                actionModifiers.Add(new ActionModifier(this, " ", repeats, " times", "Repeats"));

            }
            if (targets == Var.infinityValue)
            {
                actionModifiers.Add(new ActionModifier(this, " <sprite name=Target>all", valueType: "Targets"));

            }
            else if(targets > 1)
            {
                //currentDescriptionEnd += " target " + targets;
                actionModifiers.Add(new ActionModifier(this, " <sprite name=Target>", targets, null, "Targets"));
            }
            if (targetType == "friendly")
            {
                if (targets == 1)
                {
                    //currentDescriptionEnd += " target " + targets;
                    actionModifiers.Add(new ActionModifier(this, " <sprite name=Target>", targets, null, "Targets"));
                }
                actionModifiers.Add(new ActionModifier(this, " friendly", valueType: "TargetType"));
            }
            else if (targetType == "any")
            {
                    if (targets == 1)
                    {
                        //currentDescriptionEnd += " target " + targets;
                        actionModifiers.Add(new ActionModifier(this, " <sprite name=Target>", targets, null, "Targets"));
                    }
                    actionModifiers.Add(new ActionModifier(this, " figure", valueType: "TargetType"));
            }
            if (attackRange > 1)
            {
                actionModifiers.Add(new ActionModifier(this, " <sprite name=Range>", attackRange, null, "Range"));
            }
            //tring separator = ", ";
            //string attackText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            //actionManager.PlanToList.Add(attackText);

            ActionDescription currentAction = new ActionDescription("Attack", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
        }
        else if (isPreparingMove)
        {
            if (movePosibilities.Count == 0)
            {
                movePosibilities.Add(hexPos);
                GameObject tile = mapManager.GetTileAtHex(hexPos);
                GameObject border = tile.transform.Find("Border").gameObject;
                shownTileBorders.Add(border);
                border.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            List<Vector2> targetableLocations = pathfinder.PlanTargetableLocations(movePosibilities, finalRange);
            targetableLocations.RemoveAll(item => movePosibilities.Contains(item) || unsafeMovePosibilities.Contains(item));
            foreach (Vector2 pos in targetableLocations)
            {
                GameObject tile = mapManager.GetTileAtHex(pos);
                GameObject border = tile.transform.Find("Border").gameObject;
                shownTileBorders.Add(border);
                border.GetComponent<SpriteRenderer>().color = Color.magenta;
            }
        }
        else
        {
            actionManager.ActionStackNames.Push("Attack");
            effectedFigures.Clear();
            //^ is xor
            if (((isPlayer || controled) && targets != Var.infinityValue) ^ manualOverride)
            {
                List<Figure> posibleTargets = FindPosibleTargets(targetType, finalRange);
                targetsLeft = targets;
                while (targetsLeft > 0 && posibleTargets.Count > 0)
                {
                    Figure targetedFigure = null;
                    yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                    posibleTargets.Remove(targetedFigure);
                    if (targetsLeft > 0)
                    {
                        targetsLeft--;
                        yield return gameManager.StartCoroutine(targetedFigure.AttackedFor(this, finalAttack, repeats, attackConditions));
                        if (targetsLeft <= 0)
                        {
                            EndAction();
                        }
                        else
                        {
                            statsDisplayer.ChangePlan("<sprite name=Target>", targetsLeft);
                        }
                    }
                }
                EndAction();
            }
            else
            {
                foreach (Figure target in FindTargets(targetType, finalRange, targets))
                {
                    //Debug.Log(target);
                    yield return gameManager.StartCoroutine(target.AttackedFor(this, finalAttack, repeats, attackConditions));
                }
                //ActionDone();
                EndAction();
            }
            if (!unmodifiedAction && exists)
            {
                yield return StartCoroutine(RemoveCondition("Vigor"));
            }
        }
        yield return null;
    }

    public IEnumerator Move(int moveValue, bool isJump = false, bool isVariable = false,bool manualOverride = false)
    {
        if (isVariable)
        {
            moveValue *= playerControler.VariableCardModifier;
        }
        bool finalJump = conditionEffects.ModifyJump(this, isJump);
        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            ActionModifier moveValueDescription = new ActionModifier(this, "<sprite name=Move>", moveValue, null, "Move");
            actionModifiers.Add(moveValueDescription);


            if (finalJump && !canFly)
            {
                ActionModifier jumpDescription = new ActionModifier(this, " Jump");
                actionModifiers.Add(jumpDescription);
            }
            ActionDescription currentAction = new ActionDescription("Move", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
        }
        else if (isPreparingMove)
        {
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            List<Vector2>[] posibleTiles = pathfinder.PlanPosiblePaths(hexPos, gameObject, finalMove, finalJump, canFly);
            movePosibilities = posibleTiles[0];
            unsafeMovePosibilities = posibleTiles[1];
            foreach (Vector2 safeTile in posibleTiles[0])
            {
                
                GameObject tile = mapManager.GetTileAtHex(safeTile);
                GameObject border = tile.transform.Find("Border").gameObject;
                shownTileBorders.Add(border);
                border.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            foreach (Vector2 unsafeTile in posibleTiles[1])
            {
                GameObject tile = mapManager.GetTileAtHex(unsafeTile);
                GameObject border = tile.transform.Find("Border").gameObject;
                shownTileBorders.Add(border);
                border.GetComponent<SpriteRenderer>().color = Color.red;
            }
            //posibleTiles[0].AddRange(posibleTiles[1]);
            //shownTiles = new List<Vector2>(posibleTiles[0]);
        }
        else
        {
            //actionManager.ActionEnded = false;
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            actionManager.ActionStackNames.Push("Move");

            //manualOverride flips whether player controls action
            if ((isPlayer || controled) ^ manualOverride)
            {
                //Debug.Log("controled move");
                isMoving = true;
                playerControler.PlanningMove = true;
                //actionManager.ActiveFigure = this;
                moveLeft = finalMove;
                canJump = finalJump;
                playerControler.UpdateMoveCostDisplay();

                //List<Figure> posibleTargets = FindPosibleTargets("enemy", finalRange);
                while (isMoving)
                {
                    GameObject targetedTile = null;
                    yield return StartCoroutine(playerControler.ControledChooseTile((result) => { targetedTile = result; }));
                    if (isMoving)
                    {
                        PlanMove(targetedTile);
                        yield return StartCoroutine(MoveAlongPath());
                        if (!isMoving && !actionManager.ActionEnded)
                        {
                            EndAction();
                        }
                    }
                }
                if (!actionManager.ActionEnded)
                {
                    EndAction();
                }
                isMoving = false;
                playerControler.PlanningMove = false;


                //yield return StartCoroutine(playerControler.ControledMove(finalMove, finalJump));
            }
            else
            {
                //Debug.Log("uncontroled move");
                //Debug.Log(gameObject + " started pathfinding");
                if (distanceToFocus < 50)
                {
                    //Debug.Log("old pathfining");
                    yield return StartCoroutine(pathfinder.PathfindTowards(hexPos, focusScript.HexPos, gameObject, finalMove, preferedRange, finalJump, canFly));
                }
                else
                {
                    //Debug.Log("new pathfining");
                    yield return StartCoroutine(pathfinder.FarPathfindTowards(hexPos, focusScript.HexPos, gameObject, finalMove, preferedRange, finalJump, canFly));

                }
                //Debug.Log(gameObject + " finished pathfinding");
                EndAction();
            }
            if (!unmodifiedAction)
            {
                yield return StartCoroutine(RemoveCondition("Burst"));
            }
        }
        //yield return null;

    }
    //calculates and hilights the path the player would take attemtping to move to a tile
    public virtual void PlanMove(GameObject tile)
    {
        //Debug.Log("moveing twords " + tile + " at " + mapManager.RectToHex(tile.transform.position));
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
            pathfinder.PlanPathToTile(hexPos, mapManager.RectToHex(tile.transform.position), gameObject, moveLeft, canJump, canFly);
            foreach (Vector2 tileCords in pathfinder.ActualPath)
            {
                GameObject newTile = mapManager.GetTileAtHex(tileCords);
                GameObject border = newTile.transform.Find("Border").gameObject;
                border.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
        }
    }
    //moves player along the planned path
    public virtual IEnumerator MoveAlongPath()
    {
        pathfinder.MoveLeft = moveLeft;
        yield return StartCoroutine(pathfinder.MoveAlongPath(gameObject, hexPos));
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
        if (playerControler.ActionsRemaining.Count > 0)
        {
            playerControler.statsDisplayer.ChangePlan("<sprite name=Move>", moveLeft);
        }
        //currentTile = mapManager.GetTileAtHex(hexPos);
        if (moveLeft <= 0)
        {
            isMoving = false;
            playerControler.PlanningMove = false;
        }
        else
        {
            Vector2 checkpos = Vector2.zero;
            bool couldMoveMore = false;
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: checkpos = hexPos + Vector2.up; break;
                    case 1: checkpos = hexPos + Vector2.down; break;
                    case 2: checkpos = hexPos + Vector2.right; break;
                    case 3: checkpos = hexPos + Vector2.left; break;
                    case 4: checkpos = hexPos + Vector2.up + Vector2.right; break;
                    case 5: checkpos = hexPos + Vector2.down + Vector2.left; break;
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
                playerControler.PlanningMove = false;
            }
        }
    }

    public IEnumerator ApplyCondition(Condition condition, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false, bool manualOverride = false, bool isVariable = false)
    {
        yield return StartCoroutine(ApplyConditions(new Condition[] { condition }, targetType, range, targets, displayTarget, manualOverride, isVariable));
    }

    public IEnumerator ApplyConditions(Condition[] newConditions, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false, bool manualOverride = false, bool isVariable = false)
    {
        if (isVariable)
        {
            foreach (Condition condition in newConditions)
            {
                condition.Value *= playerControler.VariableCardModifier;

            }
        }
        int finalRange = conditionEffects.ModifyRange(this, range);
        if (isPlanning)
        {
            List<ActionDescription> individualConditionText = new List<ActionDescription>();
            string currentDescriptionStart = "";
            string currentDescriptionEnd = "";
            bool abnormal = false;
            List<string> conditionAbnormalities = new List<string>();
            foreach (Condition condition in newConditions)
            {
                if (condition.Abnormality != null)
                {
                    foreach (string abnormality in condition.Abnormality)
                    {
                        actionAbnormalities.Add(abnormality);
                        conditionAbnormalities.Add(abnormality);
                    }
                    abnormal = true;
                }
                if (conditionAbnormalities.Contains("Ability"))
                {
                    Ability ability;
                    if (condition is GainAbility gainAbilityRef)
                    {
                        //List<string> conditionPlan = new List<string>();
                        //Debug.Log("About to gain ablility");
                        ability = gainAbilityRef.GainedAbility;
                        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainNewAbility(ability.Cost, ability.Actions, condition.Duration), individualConditionText));
                        //Debug.Log("finished gaining ablility");

                        //individualConditionText.Add(currentDescriptionString);
                    }
                }

                string currentDescriptionString = "";

                //Debug.Log(actionAbnormalities);
                if (condition.Plan != null)
                {
                    List<ActionDescription> conditionPlan = new List<ActionDescription>();

                    foreach (Func<IEnumerator> action in condition.Plan)
                    {
                        yield return StartCoroutine(actionManager.PreformAction(action(), conditionPlan));
                    }
                    if (conditionAbnormalities.Contains("Delayed Gain"))
                    {
                        if (conditionPlan.Count > 0)
                        {
                            if (condition.Duration == 1)
                            {
                                conditionPlan[0].ActionModifiers.Insert(0, new ActionModifier(this, "Next turn ", valueType: "duration"));

                            }
                            else if (condition.Duration == Var.infinityValue)
                            {
                                conditionPlan[0].ActionModifiers.Insert(0, new ActionModifier(this, "Start of turn ", valueType: "duration"));
                            }
                            else
                            {
                                conditionPlan[0].ActionModifiers.Insert(0, new ActionModifier(this, "At the start of the next ", condition.Duration, " turns ", valueType: "duration"));
                            }
                            if (conditionPlan.Count > 1)
                            {
                                conditionPlan[conditionPlan.Count - 1].ActionModifiers.Insert(0, new ActionModifier(this, "and "));

                            }
                        }
                        else
                        {
                            Debug.Log("no condition text");
                        }

                    }
                    foreach (ActionDescription action in conditionPlan)
                    {
                        individualConditionText.Add(action);

                    }
                }
                if (!conditionAbnormalities.Contains("Delayed Gain") && !conditionAbnormalities.Contains("Ability"))
                {
                    if (conditionAbnormalities.Contains("No Value Description"))
                    {
                        currentDescriptionString = condition.ActionName;
                    }
                    else
                    {
                        //if the condition has not abnormalitys
                        if (condition.Value == Var.nullValue)
                        {
                            currentDescriptionString = condition.ActionName;
                        }
                        else
                        {
                            currentDescriptionString = condition.Value + " " + condition.ActionName;
                        }

                    }
                    //if the condition has not abnormalitys
                    if (condition.Duration == 1)
                    {
                        currentDescriptionString += " this turn";
                    }
                    else if (condition.Duration != Var.infinityValue)
                    {
                        currentDescriptionString += " for " + condition.Duration + " turns";
                    }
                    //individualConditionText.Add(currentDescriptionString);

                    ActionDescription currentAction2 = new ActionDescription("Ability", new List<ActionModifier>() { new ActionModifier(this, currentDescriptionString) });
                    individualConditionText.Add(currentAction2);
                }
            }
            if (targetType == "self")
            {
                if (!conditionAbnormalities.Contains("No Self Target Description"))
                {
                    currentDescriptionStart += "Gain ";
                }



                //bool isPositive = false;
                //foreach (Condition MechanicalAutomaton in newConditions)
                //{
                //    if (MechanicalAutomaton.Value > 0)
                //    {
                //        isPositive = true;
                //        break;
                //    }
                //}
                //if (isPositive)
                //{
                //    currentDescriptionStart += "Gain ";
                //}
                //else
                //{
                //    currentDescriptionStart += "Lose ";
                //}

            }
            else if (conditionAbnormalities.Contains("Augment"))
            {
                currentDescriptionStart += "This summon gains ";
            }
            else 
            {
                currentDescriptionStart += "Apply ";
                if (targets != 1)
                {
                    currentDescriptionEnd += " <sprite name=Target>";
                    if (targets == Var.infinityValue)
                    {
                        currentDescriptionEnd += "all";
                    }
                    else
                    {
                        currentDescriptionEnd += targets;
                    }
                    
                }
                if (targetType == "ally")
                {
                    currentDescriptionEnd += targetType.ToQuantity(targets, ShowQuantityAs.None);

                    //if (targets == 1)
                    //{
                    //    currentDescriptionEnd += " ally";
                    //}
                    //else
                    //{
                    //    currentDescriptionEnd += " allies";
                    //}
                }
                else if (targetType == "summon")
                {
                    currentDescriptionEnd += targetType.ToQuantity(targets, ShowQuantityAs.None);

                    //if (targets == 1)
                    //{
                    //    currentDescriptionEnd += " summon";
                    //}
                    //else
                    //{
                    //    currentDescriptionEnd += " summons";
                    //}
                }
                else if (targetType == "any")
                {
                    if (targets == 1)
                    {
                        currentDescriptionEnd += " figure";
                    }
                    else
                    {
                        currentDescriptionEnd += " figures";
                    }
                }
                if (displayTarget)
                {
                    currentDescriptionEnd += targetType.ToQuantity(targets, ShowQuantityAs.None);

                    //if (targetType == "enemy")
                    //{
                    //    if (targets == 1)
                    //    {
                    //        currentDescriptionEnd += " enemy";
                    //    }
                    //    else
                    //    {
                    //        currentDescriptionEnd += " enemies";
                    //    }

                    //}
                    //if (targetType == "friendly")
                    //{
                    //    if (targets == 1)
                    //    {
                    //        currentDescriptionEnd += " friendly";
                    //    }
                    //    else
                    //    {
                    //        currentDescriptionEnd += " friendly";
                    //    }
                    //}
                }
                if (range == Var.infinityValue)
                {
                    currentDescriptionEnd += " any<sprite name=Range>";
                }
                else
                    //if (range != 1)
                {
                    currentDescriptionEnd += " <sprite name=Range>" + range;
                }
                if (!isPlayer)
                {
                    if (preferedRange > range && targetType == "enemy")
                    {
                        preferedRange = range;
                    }
                }
            }
            //Debug.Log("string.joind doesnt work");
            string conditionText = currentDescriptionStart;
            int numberOfConditions = individualConditionText.Count;
            for (int i = 1; i <= numberOfConditions; i++)
            {
                if (numberOfConditions == 1)
                {}
                else if (i == numberOfConditions)
                {
                    if (numberOfConditions == 2)
                    {
                        conditionText += " and ";
                    }
                    else
                    {
                        conditionText += ", and ";
                    }

                }
                else if (i != 1)
                {
                    conditionText += ", ";
                }
                conditionText += individualConditionText[i-1].GetDescription();
                //Debug.Log(conditionText);

            }
            conditionText += currentDescriptionEnd;
            //string conditionText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            //actionManager.PlanToList.Add(conditionText);

            ActionDescription currentAction = new ActionDescription("Condition", new List<ActionModifier>() { new ActionModifier(this, conditionText) });
            actionManager.PlanToList.Add(currentAction);
            if (abnormal)
            {
                actionAbnormalities.Clear();
            }
        }
        else if (!isPreparingMove)
        {
            //Debug.Log("Applied Condition");
            actionManager.ActionStackNames.Push("Condition");
            effectedFigures.Clear();
            if (targetType == "self")
            {
                //Debug.Log("Gained Condition");

                yield return StartCoroutine(GainConditions(newConditions));
                //if (isAction)
                //{
                //    ActionDone();
                //}
                EndAction();
            }
            else if (((isPlayer || controled) && targets != Var.infinityValue) ^ manualOverride)
            {
                List<Figure> posibleTargets = FindPosibleTargets(targetType, finalRange);
                targetsLeft = targets;
                while (targetsLeft > 0 && posibleTargets.Count > 0)
                {
                    Figure targetedFigure = null;
                    yield return playerControler.ControledChooseFigures(posibleTargets, (result) => { targetedFigure = result; });
                    posibleTargets.Remove(targetedFigure);
                    if (targetsLeft > 0)
                    {
                        targetsLeft--;
                        yield return StartCoroutine(ApplyConditionsTo(newConditions, targetedFigure));
                        //foreach (Condition condition in newConditions)
                        //{
                        //    yield return gameManager.StartCoroutine(targetedFigure.GainCondition(condition));
                        //}
                        if (targetsLeft <= 0)
                        {
                            EndAction();
                        }
                        else
                        {
                            statsDisplayer.ChangePlan("<sprite name=Target>", targetsLeft);
                        }
                    }
                }
                if (!actionManager.ActionEnded)
                {
                    EndAction();
                }
                //yield return StartCoroutine(playerControler.ControledApplyConditions(newConditions, targetType, range, targets));
            }
            else
            {
                foreach (Figure target in FindTargets(targetType, range, targets))
                {
                    yield return StartCoroutine(ApplyConditionsTo(newConditions, target));

                }
                EndAction();
                //if (isAction)
                //{
                //    ActionDone();
                //}
            }
        }

    }
    public IEnumerator RemoveCondition(string name)
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].ConditionName == name)
            {
                string effectedAction = conditions[i].EffectedAction;
                yield return StartCoroutine(conditions[i].OnLoss(this));
                conditions.RemoveAt(i);
                i--;
                if (effectedAction != "None")
                {
                    if (isAI)
                    {
                        yield return StartCoroutine(GetComponent<AIFigure>().UpdatePlanDiscription(effectedAction));
                    }
                    if (isPlayer)
                    {
                        yield return StartCoroutine(deckManager.UpdateCardsDisplay(effectedAction));
                    }
                }
                yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
            }
        }

    }
    public IEnumerator ApplyConditionsTo(Condition[] conditions, Figure target)
    {
        foreach (Condition condition in conditions)
        {
            yield return StartCoroutine(ApplyConditionTo(condition,target));
        }
    }
    public IEnumerator ApplyConditionTo(Condition condition, Figure target)
    {
        yield return StartCoroutine(target.GainCondition(condition));
    }
    public IEnumerator LoseConditions(List<Condition> newConditions, Figure target = null)
    {
        if (target == null)
        {
            target = this;
        }
        foreach (Condition condition in newConditions)
        {
            int conditionValue = condition.Value;
            if (conditionValue == Var.nullValue)
            {
                for (int i = 0; i < target.Conditions.Count; i++)
                {
                    if (target.Conditions[i].ConditionName == condition.ConditionName)
                    {
                        target.Conditions.RemoveAt(i);
                    }
                }
            }
            else
            {
                condition.Value = -conditionValue;
                yield return StartCoroutine(target.GainCondition(condition));
            }
        }
    }

    public IEnumerator Summon(GameObject summon, int maxSummons = Var.infinityValue)
    {
        currentSummons.RemoveAll(item => item == null);
        if (currentSummons.Count < maxSummons || maxSummons == Var.infinityValue)
        {
            if (isPlanning)
            {
                string summonName = summon.name;
                summonName = Regex.Replace(summonName, "(.)([A-Z,0-9])", "$1 $2");
                ActionDescription currentAction = new ActionDescription("Summon", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summonName) });
                actionManager.PlanToList.Add(currentAction);
            }
            else if (!isPreparingMove)
            {
                actionManager.ActionStackNames.Push("Summon");
                Vector2 checktile = Vector2.zero;
                Vector2 summonPos = Vector2.zero;
                bool canSummon = false;
                for (int i = 0; i < 6; i++)
                {
                    switch (i)
                    {
                        case 0: checktile = hexPos + Vector2.up; break;
                        case 1: checktile = hexPos + Vector2.down; break;
                        case 2: checktile = hexPos + Vector2.right; break;
                        case 3: checktile = hexPos + Vector2.left; break;
                        case 4: checktile = hexPos + Vector2.up + Vector2.right; break;
                        case 5: checktile = hexPos + Vector2.down + Vector2.left; break;
                    }
                    GameObject tile = mapManager.GetTileAtHex(checktile);
                    GameObject entity = mapManager.GetEntityOnHex(checktile);
                    if (entity == null)
                    {
                        if (!tile.GetComponent<Wall>() && !tile.GetComponent<Obstacle>())
                        {
                            summonPos = mapManager.HexToRect(checktile);
                            canSummon = true;
                            break;
                        }
                    }
                }
                effectedFigures = new List<Figure>();
                if (canSummon)
                {
                    GameObject newSummon = Instantiate(summon, new Vector3(summonPos.x, summonPos.y, summon.transform.position.z), Quaternion.identity);
                    Figure summonScript = newSummon.GetComponent<Figure>();
                    summonScript.isSummon = true;
                    summonScript.summoner = this;
                    currentSummons.Add(summonScript);
                    //yield return StartCoroutine(actionManager.PreformAction(summonScript.ApplyConditions(new Condition[] { new Summon(), new Stunned(1, false) })));

                    if (isPlayer)
                    {
                        
                        playerControler.PlayedCardScript.PrepareExhaustAfterPlayed(() => summonScript.exists == false, deckManager.Discard);

                        effectedFigures.Add(summonScript);
                    }
                    //yield return StartCoroutine(actionManager.PreformAction(summonScript.ApplyCondition(new Stunned(2, true))));
                }
                if (isPlayer)
                {
                    playerControler.PlayedCardScript.ActingFigures = new List<Figure>(effectedFigures);
                }
                //ActionDone();
                EndAction();

            }
        }
        else
        {
            if (isPlanning)
            {
                ActionDescription currentAction = new ActionDescription("Summon", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summon.name + "(at max)") });
                actionManager.PlanToList.Add(currentAction);
            }
        }
        yield break;
    }
    public IEnumerator Upkeep(Condition upkeep)
    {

        yield return StartCoroutine(Upkeeps(new Condition[] { upkeep }));

        //if (isPlanning)
        //{
        //    yield return StartCoroutine(ApplyCondition(upkeep));
        //    actionManager.PlanToList[actionManager.PlanToList.Count - 1].ActionModifiers.Insert(0, new ActionModifier(this, "Upkeep: "));
        //}
        //else
        //{
        //    if (!hasUpkeep)
        //    {
        //        this.Removed += removeUpkeeps;
        //        hasUpkeep = true;
        //    }
        //    yield return StartCoroutine(summoner.ApplyCondition(upkeep));
        //    upkeptConditions.Add(upkeep);
        //}
    }
    public IEnumerator Upkeeps(Condition[] upkeep)
    {
        if (isPlanning)
        {
            yield return StartCoroutine(ApplyConditions(upkeep));
            actionManager.PlanToList[actionManager.PlanToList.Count-1].ActionModifiers.Insert(0, new ActionModifier(this, "Upkeep: "));
        }
        else
        {
            if (!hasUpkeep)
            {
                this.Removed += removeUpkeeps;
                hasUpkeep = true;
            }
            yield return StartCoroutine(summoner.ApplyConditions(upkeep));
            foreach (Condition condition in upkeep)
            {
                upkeptConditions.Add(condition);
            }
        }
    }
    public IEnumerator removeUpkeeps()
    {
        //Debug.Log(summoner);
        this.Removed -= removeUpkeeps;
        yield return StartCoroutine(summoner.LoseConditions(upkeptConditions));
    }


    /*
    public void DelayedApplyConditions(Condition[] newConditions, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false)
    {
        actionAbnormality = "Delayed Gain";
        if (isPlanning)
        {
            ApplyConditions(newConditions, targetType, range, targets, displayTarget);
        }
        actionAbnormality = "";
        ActionDone();
    }
    public void GainPower(Condition power, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false)
    {
        actionAbnormality = "Delayed Gain";
        if (isPlanning)
        {

        }
        actionAbnormality = "";
        ActionDone();
    }
    */

    public IEnumerator GainConditions(Condition[] newConditions)
    {
        //Debug.Log("GainedConditions");
        //Debug.Log("Conditions count " + newConditions.Length);

        foreach (Condition condition in newConditions)
        {
            yield return StartCoroutine(GainCondition(condition));
        }
    }
    public IEnumerator GainCondition(Condition addedCondition)
    {
        Condition condition = addedCondition.Clone();
        //Debug.Log("GainedCondition");
        bool isDuplicate = false;
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].ConditionName == condition.ConditionName)
            {
                //add type 0 = ceat new instance
                //if (condition.AddType == 0)
                //{

                //}
                if (condition.AddType == 1 && conditions[i].Duration == condition.Duration)
                {
                    yield return StartCoroutine(condition.OnGain(this));
                    conditions[i].Value += condition.Value;
                    conditions[i].Value = Global.Clamp(conditions[i].Value);
                    isDuplicate = true;
                    if (conditions[i].Value == 0)
                    {
                        conditions.RemoveAt(i);
                    }
                    break;
                }
                if (condition.AddType == 2 && conditions[i].Value == condition.Value)
                {
                    if (conditions[i].Duration == Var.infinityValue || condition.Duration == Var.infinityValue)
                    {
                        Debug.Log("Warrning gained condtion already had one of");
                    }
                    else
                    {
                        conditions[i].Duration += condition.Duration;
                        conditions[i].Duration = Global.Clamp(conditions[i].Duration,0);
                    }
                    isDuplicate = true;
                    break;
                }
                if (condition.AddType == 3)
                {
                    //Debug.Log("removed" + conditions[i].Name);
                    conditions.RemoveAt(i);
                    i--;
                }

            }
        }
        if (isDuplicate == false)
        {
            //Debug.Log("added" + condition.ConditionName);

            conditions.Add(condition);
            yield return StartCoroutine(condition.OnGain(this));
            //Debug.Log("first condition: " + conditions[0].Name);

        }
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
        if (condition.EffectedAction != "None")
        {
            if (isPlayer)
            {
                yield return StartCoroutine(deckManager.UpdateCardsDisplay(condition.EffectedAction));
            }
            else
            {
                yield return StartCoroutine(GetComponent<AIFigure>().UpdatePlan());

            }
        }
    }
    //returns targets chosen by game (i think priority is closest then arbitrary)
    public List<Figure> FindTargets(string targetType, int range = 1, int targets = 1)
    {
        return ChooseTargets(FindPosibleTargets(targetType, range), targets);
    }
    public List<Figure> FindPosibleTargets(string targetType, int range = 1)
    {
        List<Figure> targetableFigures = new List<Figure>();
        List<Figure> posibleTargets = pathfinder.GetFiguresInRange(hexPos, range, gameObject);
        if (targetType == "self")
        {
            targetableFigures.Add(this);
        }
        else if (targetType == "enemy")
        {
            foreach (Figure posibletarget in posibleTargets)
            {
                if (posibletarget.team != team)
                {
                    targetableFigures.Add(posibletarget);
                }
            }
        }
        else if (targetType == "ally")
        {
            foreach (Figure posibletarget in posibleTargets)
            {
                if (posibletarget.team == team && posibletarget != this)
                {
                    targetableFigures.Add(posibletarget);
                }
            }
        }
        else if (targetType == "friendly")
        {
            foreach (Figure posibletarget in posibleTargets)
            {
                if (posibletarget.team == team)
                {
                    targetableFigures.Add(posibletarget);
                }

            }
        }
        else if (targetType == "summon")
        {
            foreach (Figure posibletarget in posibleTargets)
            {
                if (posibletarget.summoner == this)
                {
                    targetableFigures.Add(posibletarget);
                }
            }
        }
        else if (targetType == "any")
        {
            targetableFigures = new List<Figure>(posibleTargets);
        }
        else
        {
            Debug.Log("target type dosnt exist");
        }
        return targetableFigures;
    }

    public List<Figure> ChooseTargets(List<Figure> posibleTargets, int targets = 1)
    {
        if (targets == Var.infinityValue)
        {
            return posibleTargets;
        }
        List<Figure> targetedFigures = new List<Figure>();
        //sets targets to min of itself and the abilible number of targets
        if (posibleTargets.Count < targets)
        {
            targets = posibleTargets.Count;
        }
        for (int i = 0; i < targets; i++)
        //foreach (Figure posibletarget in posibleTargets)
        {
            Figure target = posibleTargets[i];
            //if target is player and there is a summon equal range prioritize the summon
            if (target == playerControler && posibleTargets.Count > targets)
            {
                int distanceToPlayer;
                int distanceToSummon;

                distanceToPlayer = pathfinder.GetDistanceTo(hexPos, target.hexPos);
                distanceToSummon = pathfinder.GetDistanceTo(hexPos, posibleTargets[targets].hexPos);
                if (distanceToPlayer == distanceToSummon)
                {
                    targetedFigures.Add(posibleTargets[targets]);
                }
                else
                {
                    targetedFigures.Add(target);
                }
            }
            else
            {
                targetedFigures.Add(target);
            }
        }
        return targetedFigures;
    }
    public int GetValueOfCondition(string conditionName)
    {
        int value = 0;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == conditionName)
            {
                if (condition.Value == Var.nullValue)
                {
                    value = Var.nullValue;
                    break;
                }
                else
                {
                    value += condition.Value;
                }
            }
        }
        if (value == 0)
        {
            return 0;
        }
        return value;
    }
    public IEnumerator GainMaxHealth(int amount)
    {
        health += amount;
        maxHealth += amount;
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
        yield break;
    }
    public IEnumerator AttackedFor(Figure attacker, int attackValue, int repeats, Condition[] newConditions)
    {
        if (attacker == playerControler)
        {
            attackValue += playerControler.MinutureMortarCount * (pathfinder.GetDistanceTo(playerControler.HexPos, hexPos) - 1);
        }
        for (int i = 0; i < repeats; i++)
        {
            if (!isDead)
            {
				yield return gameManager.StartCoroutine(TakeDamage(attackValue));
				if (!isDead)
				{
					yield return StartCoroutine(GainConditions(newConditions));
				}
				yield return gameManager.StartCoroutine(attacker.TakeDamage(GetValueOfCondition("Thorns")));
			}
		}


	}
    public virtual IEnumerator TakeDamage(int damageValue)
    {
        if (damageValue > 0)
        {
            if (block > 0)
            {
                int damageBlocked = Mathf.Min(damageValue, block);
                damageValue -= damageBlocked;
                block -= damageBlocked;
            }
            OverallStatistics.damageDealt += damageValue;
            //Debug.Log(gameObject.name + " took damage");
            if (damageValue > 0)
            {
                yield return gameManager.StartCoroutine(LoseHealth(damageValue));
            }
        }
    }
    public virtual IEnumerator LoseHealthAction(int amount)
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("LoseHealth", new List<ActionModifier>() { new ActionModifier(this, "Lose ", amount, " health", "LoseHealth") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("LoseHealth");
            yield return gameManager.StartCoroutine(LoseHealth(amount));
            EndAction();

        }
        yield break;
    }
    public virtual IEnumerator LoseHealth(int amount)
    {
        //Debug.Log(gameObject.name + " Lost Health");
        health -= amount;
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
        if (health <= 0 && !isDead)
        {
            isDead = true;
            yield return gameManager.StartCoroutine(Die());
        }
        yield break;

    }
    public IEnumerator Heal(int amount)
    {

        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("Heal", new List<ActionModifier>() { new ActionModifier(this, "Heal ", amount, valueType: "Heal") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            health += amount;
            health = Mathf.Min(health, maxHealth);
            statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
        }
        yield break;
    }
    public void HealDamage(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
    }

    public IEnumerator Sacrifice(Figure sacrificedFigure)
    {
        if (isPlanning)
        {
            ActionDescription currentAction = new ActionDescription("Die", new List<ActionModifier>() { new ActionModifier(this, "Die") });
            actionManager.PlanToList.Add(currentAction);
        }
        else
        {
            actionManager.ActionStackNames.Push("Die");
            yield return StartCoroutine(sacrificedFigure.Die());
            EndAction();

        }
        yield break;


        //if (adaptiveShieldCount > 0)
        //{
        //    //actionManager.QueueAction(playerControler.ApplyCondition(new StartOfTurnBlock(Var.adaptiveShieldBlock * adaptiveShieldCount)));
        //    //Debug.Log("queued block nexty turn");
        //    //playerControler.ApplyCondition(new StartOfTurnSlow(Var.frozenLensSpeedLoss, -1)), relicDescriptionList))
        //    //yield return StartCoroutine(actionManager.QueueAction(Block(Var.adaptiveShieldBlock * adaptiveShieldCount)));
        //}

    }

    public virtual IEnumerator Die()
    {
        //Debug.Log("Base Die ran");
        yield return gameManager.StartCoroutine(StopExisting());
        yield break;
    }
    public virtual IEnumerator Remove(FloorManager floorManager = null)
    {
        yield return gameManager.StartCoroutine(StopExisting());
        yield break;
        //Debug.Log("Base Remove ran");
    }
    public virtual IEnumerator StopExisting()
    {
        exists = false;
        if (Removed != null)
        {
            Delegate[] listeners = Removed.GetInvocationList();
            foreach (Delegate action in listeners)
            {
                //tells computer that action takes a TurnManager and outputs a IEnumerator
                var callback = (Func<IEnumerator>)action;
                //runs action now that it is the correct type
                yield return StartCoroutine(callback());
            }
        }
        Destroy(gameObject);
        yield break;
    }

    public virtual IEnumerator MoveOneSpace()
    {
        yield break;
    }
}
