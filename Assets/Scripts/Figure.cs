using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;




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
    protected bool isEnemy, isPlayer, isAI;
    protected bool isPlanning, isPreparingMove;
    public bool IsPlanning { set { isPlanning = value; } get { return isPlanning; } }

    protected Vector2 oneToOnePos;
    public Vector2 OneToOnePos { get { return oneToOnePos; } set { oneToOnePos = value; } }

    protected int preferedRange;
    protected int distanceToFocus;
    protected bool actionEnded;
    public bool ActionEnded { set { actionEnded = value; } get { return actionEnded; } }

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

    protected int variableCardModifier;
    public int VariableCardModifier { get { return variableCardModifier; } set { variableCardModifier = value; } }

    protected List<GameObject> shownTileBorders = new List<GameObject>();
    protected List<string> actionAbnormalities = new List<string>();
    public List<string> ActionAbnormalities { set { actionAbnormalities = value; } get { return actionAbnormalities; } }

    protected bool isSummon;
    protected Figure summoner;
    protected List<Figure> currentSummons = new List<Figure>();
    protected List<Figure> effectedFigures = new List<Figure>();
    public List<Figure> EffectedFigures { get {  return effectedFigures; }  }
    public List<Figure> refEffectedFigures { get { return effectedFigures; } }

    protected Figure effectedFigure;
    public Figure EffectedFigure { get { return effectedFigure; } }

    protected bool hasFocus;
    protected GameObject focus;
    protected Figure focusScript;
    protected Vector2 focusPos;
    protected List<Vector2> movePosibilities = new List<Vector2>();

    //public static event Func<Figure, IEnumerator> Removed;
    public event Func<IEnumerator> Removed;

    public List<Condition> upkeptConditions = new List<Condition>();
    public bool hasUpkeep;

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

        yield return null;
    }

    public IEnumerator baseStartTurn()
    {
        //Debug.Log(gameObject + " is takeing turn");
        resetBlock();
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
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
    public virtual void resetBlock()
    {
        block = 0;
    }
    public IEnumerator baseEndTurn()
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
    public virtual void EndAction()
    {
        Debug.Log("Base EndAction ran");
    }
    
    public IEnumerator GetPlanString(List<Func<IEnumerator>> actions, System.Action<string> callback)
    {
        List<ActionDescription> planDescription = new List<ActionDescription>();
        foreach (Func<IEnumerator> action in actions)
        {
            yield return StartCoroutine(actionManager.PreformAction(action(), planDescription));
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
            blockValue *= variableCardModifier;
        }
        if (isPlanning)
        {

            ActionDescription currentAction = new ActionDescription("Block", new List<ActionModifier>() { new ActionModifier(this, null, blockValue, Variables.blockSprite, "Block") });
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

    public IEnumerator Attack(int attackValue, int attackRange = 1, int targets = 1, int repeats = 1, Condition[] attackConditions = null, bool isVariable = false, bool manualOverride = false)
    {
        if (isVariable)
        {
            attackValue *= variableCardModifier;
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
            actionModifiers.Add(new ActionModifier(this, null, attackValue, Variables.attackSprite, "Attack"));
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
            if (repeats > 1)
            {
                //currentDescriptionEnd += " " + repeats + " times ";
                actionModifiers.Add(new ActionModifier(this, " ", repeats, " times", "Repeats"));

            }
            if (targets > 1)
            {
                //currentDescriptionEnd += " target " + targets;
                actionModifiers.Add(new ActionModifier(this, " ", targets, Variables.targetSprite, "Targets"));
            }
            else if (targets == Variables.gameInfinityValue)
            {
                actionModifiers.Add(new ActionModifier(this, " all" + Variables.targetSprite, valueType: "Targets"));

            }
            if (attackRange > 1)
            {
                //currentDescriptionEnd += " " + attackRange + " <sprite name=Range>";
                actionModifiers.Add(new ActionModifier(this, " ", attackRange, Variables.rangeSprite, "Range"));
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
                movePosibilities.Add(oneToOnePos);
                GameObject tile = mapManager.GetTileAtHex(oneToOnePos);
                GameObject border = tile.transform.Find("Border").gameObject;
                shownTileBorders.Add(border);
                border.GetComponent<SpriteRenderer>().color = Color.blue;
            }
            List<Vector2> targetableLocations = pathfinder.PlanTargetableLocations(movePosibilities, finalRange);
            targetableLocations.RemoveAll(item => movePosibilities.Contains(item));
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
            if (isPlayer ^ manualOverride)
            {
                List<Figure> posibleTargets = FindPosibleTargets("enemy", finalRange);
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
                            statsDisplayer.ChangePlan(Variables.targetSprite, targetsLeft);
                        }
                    }
                }
                //Debug.Log("done");
                if (!actionEnded)
                {
                    //Debug.Log("ended action");
                    EndAction();
                }

                //yield return StartCoroutine(playerControler.ControledAttack(finalAttack, finalRange, targets, repeats, attackConditions));
            }
            else
            {
                foreach (Figure target in FindTargets("enemy", finalRange, targets))
                {
                    //Debug.Log(target);
                    yield return gameManager.StartCoroutine(target.AttackedFor(this, finalAttack, repeats, attackConditions));
                }
                //ActionDone();
                EndAction();
            }
            if (!unmodifiedAction)
            {
                yield return StartCoroutine(RemoveCondition("Vigor"));
            }
        }
        yield return null;
    }

    public IEnumerator Move(int moveValue, bool isJump = false, bool isVariable = false)
    {
        if (isVariable)
        {
            moveValue *= variableCardModifier;
        }
        bool finalJump = conditionEffects.ModifyJump(this, isJump);
        //Mathf(finalMove,0,)
        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            ActionModifier moveValueDescription = new ActionModifier(this, null, moveValue, Variables.moveSprite, "Move");
            actionModifiers.Add(moveValueDescription);
            //string planString = "Move " + finalMove;
            //string planString = finalMove + " <sprite name=Move>";

            if (finalJump && !canFly)
            {
                ActionModifier jumpDescription = new ActionModifier(this, " Jump");
                actionModifiers.Add(jumpDescription);

                //planString += " Jump";
            }
            //actionManager.PlanToList.Add(planString);
            ActionDescription currentAction = new ActionDescription("Move", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
        }
        else if (isPreparingMove)
        {
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            List<Vector2>[] posibleTiles = pathfinder.PlanPosiblePaths(oneToOnePos, gameObject, finalMove, finalJump, canFly);
            movePosibilities = posibleTiles[0];
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
            //actionEnded = false;
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            actionManager.ActionStackNames.Push("Move");


            if (isPlayer)
            {
                isMoving = true;
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
                        playerControler.PlanMove(targetedTile);
                        yield return StartCoroutine(playerControler.MoveAlongPath());
                        if (!isMoving && !actionEnded)
                        {
                            EndAction();
                        }
                    }
                }

                //yield return StartCoroutine(playerControler.ControledMove(finalMove, finalJump));
            }
            else
            {
                //Debug.Log(gameObject + " started pathfinding");
                if (distanceToFocus < 50)
                {
                    //Debug.Log("old pathfining");
                    yield return StartCoroutine(pathfinder.PathfindTowards(oneToOnePos, focusScript.OneToOnePos, gameObject, finalMove, preferedRange, finalJump, canFly));
                }
                else
                {
                    //Debug.Log("new pathfining");
                    yield return StartCoroutine(pathfinder.NewPathfindTowards(oneToOnePos, focusScript.OneToOnePos, gameObject, finalMove, preferedRange, finalJump, canFly));

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
                condition.Value *= variableCardModifier;

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
                        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainNewAbility(ability.Cost, ability.Abilities, condition.Duration), individualConditionText));
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
                            else if (condition.Duration == Variables.gameInfinityValue)
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
                        if (condition.Value == Variables.gameNullValue)
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
                    else if (condition.Duration != Variables.gameInfinityValue)
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
                //foreach (Condition test in newConditions)
                //{
                //    if (test.Value > 0)
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
                    if (targets == Variables.gameInfinityValue)
                    {
                        currentDescriptionEnd += " all";
                    }
                    else
                    {
                        currentDescriptionEnd += " " + targets;
                    }
                    currentDescriptionEnd += Variables.targetSprite;
                    
                }
                if (targetType == "ally")
                {
                    if (targets != 1)
                    {
                        currentDescriptionEnd += " ally";
                    }
                    else
                    {
                        currentDescriptionEnd += " allies";
                    }
                }
                if (targetType == "summon")
                {
                    if (targets != 1)
                    {
                        currentDescriptionEnd += " summon";
                    }
                    else
                    {
                        currentDescriptionEnd += " summons";
                    }
                }
                if (displayTarget)
                {
                    if (targetType == "enemy")
                    {
                        if (targets != 1)
                        {
                            currentDescriptionEnd += " enemy";
                        }
                        else
                        {
                            currentDescriptionEnd += " enemies";
                        }

                    }
                    if (targetType == "self or ally")
                    {
                        if (targets != 1)
                        {
                            currentDescriptionEnd += " ally or self";
                        }
                        else
                        {
                            currentDescriptionEnd += " allies or self";
                        }
                    }

                }
                if (range == Variables.gameInfinityValue)
                {
                    currentDescriptionEnd += " any<sprite name=Range>";
                }
                else if (range != 1)
                {
                    currentDescriptionEnd += " " + range + "<sprite name=Range>";
                }
                if (!isPlayer)
                {
                    if (preferedRange > range && targetType == "enemy")
                    {
                        preferedRange = range;
                    }
                }
            }
            string separator = ", ";
            //Debug.Log("string.joind doesnt work");
            string conditionText = currentDescriptionStart;
            for (int i = 0; i < individualConditionText.Count; i++)
            {
                if (i != 0)
                {
                    conditionText += separator;
                }
                conditionText += individualConditionText[i].GetDescription();
                //Debug.Log(conditionText);

            }
            conditionText += currentDescriptionEnd;
            //string conditionText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            //actionManager.PlanToList.Add(conditionText);

            ActionDescription currentAction = new ActionDescription("Ability", new List<ActionModifier>() { new ActionModifier(this, conditionText) });
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
            else if (isPlayer ^ manualOverride)
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
                            statsDisplayer.ChangePlan(Variables.targetSprite, targetsLeft);
                        }
                    }
                }
                if (!actionEnded)
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
            if (conditionValue == Variables.gameNullValue)
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

    public IEnumerator Summon(GameObject summon, int maxSummons = Variables.gameInfinityValue)
    {
        currentSummons.RemoveAll(item => item == null);
        if (currentSummons.Count < maxSummons || maxSummons == Variables.gameInfinityValue)
        {
            if (isPlanning)
            {
                //prepareActions.Add(() => Block(finalBlock));
                //string currentDescriptionString = "Block " + finalBlock;
                //string currentDescriptionString = "Summon " + summon.name;

                //Debug.Log("planned " + currentDescriptionString);
                //actionManager.PlanToList.Add(currentDescriptionString);
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
                        case 0: checktile = oneToOnePos + Vector2.up; break;
                        case 1: checktile = oneToOnePos + Vector2.down; break;
                        case 2: checktile = oneToOnePos + Vector2.right; break;
                        case 3: checktile = oneToOnePos + Vector2.left; break;
                        case 4: checktile = oneToOnePos + Vector2.up + Vector2.right; break;
                        case 5: checktile = oneToOnePos + Vector2.down + Vector2.left; break;
                    }
                    GameObject tile = mapManager.GetTileAtHex(checktile);
                    GameObject entity = mapManager.GetEntityOnHex(checktile);
                    if (entity == null)
                    {
                        if (!tile.GetComponent<Wall>() && !tile.GetComponent<Obstacle>())
                        {
                            summonPos = mapManager.OneToOneToPos(checktile);
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
                    yield return StartCoroutine(actionManager.PreformAction(summonScript.ApplyConditions(new Condition[] { new Summon(), new Stunned(1, false) })));

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
        //if (isVariable)
        //{
        //    blockValue *= variableCardModifier;
        //}
        //int finalBlock = conditionEffects.ModifyBlock(this, blockValue);


    }

    public IEnumerator Upkeep(Condition upkeep)
    {
        if (isPlanning)
        {
            yield return StartCoroutine(ApplyCondition(upkeep));
            actionManager.PlanToList[actionManager.PlanToList.Count-1].ActionModifiers.Insert(0, new ActionModifier(this, "Upkeep: "));
        }
        else
        {
            if (!hasUpkeep)
            {
                this.Removed += removeUpkeeps;
                hasUpkeep = true;
            }
            yield return StartCoroutine(summoner.ApplyCondition(upkeep));
            upkeptConditions.Add(upkeep);
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
                    conditions[i].Value += condition.Value;
                    conditions[i].Value = Mathf.Clamp(conditions[i].Value, Variables.gameMinValue, Variables.gameMaxValue);
                    isDuplicate = true;
                    if (conditions[i].Value == 0)
                    {
                        conditions.RemoveAt(i);
                    }
                    break;
                }
                if (condition.AddType == 2 && conditions[i].Value == condition.Value)
                {
                    if (conditions[i].Duration == Variables.gameInfinityValue || condition.Duration == Variables.gameInfinityValue)
                    {
                        Debug.Log("Warrning gained condtion already had one of");
                    }
                    else
                    {
                        conditions[i].Duration += condition.Duration;
                        conditions[i].Duration = Mathf.Clamp(conditions[i].Duration, 0, Variables.gameMaxValue);
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

    //public IEnumerator LoseCondition(Condition removedCondition)
    //{
    //    Condition condition = removedCondition.Clone();
    //    //Debug.Log("GainedCondition");
    //    bool isDuplicate = false;
    //    for (int i = 0; i < conditions.Count; i++)
    //    {
    //        if (conditions[i].ConditionName == condition.ConditionName)
    //        {
    //            //add type 0 = ceat new instance
    //            //if (condition.AddType == 0)
    //            //{

    //            //}
    //            if (condition.AddType == 1 && conditions[i].Duration == condition.Duration)
    //            {
    //                conditions[i].Value += condition.Value;
    //                conditions[i].Value = Mathf.Clamp(conditions[i].Value, Variables.gameMinValue, Variables.gameMaxValue);
    //                isDuplicate = true;
    //                if (conditions[i].Value == 0)
    //                {
    //                    yield return StartCoroutine(conditions[i].OnLoss(this));
    //                    conditions.RemoveAt(i);
    //                }
    //                break;
    //            }
    //            if (condition.AddType == 2 && conditions[i].Value == condition.Value)
    //            {
    //                if (conditions[i].Duration == Variables.gameInfinityValue || condition.Duration == Variables.gameInfinityValue)
    //                {
    //                    Debug.Log("Warrning gained condtion already had one of");
    //                }
    //                else
    //                {
    //                    conditions[i].Duration += condition.Duration;
    //                    conditions[i].Duration = Mathf.Clamp(conditions[i].Duration, 0, Variables.gameMaxValue);
    //                }
    //                isDuplicate = true;
    //                break;
    //            }
    //            if (condition.AddType == 3)
    //            {
    //                //Debug.Log("removed" + conditions[i].Name);
    //                conditions.RemoveAt(i);
    //                i--;
    //            }

    //        }
    //    }
    //    if (isDuplicate == false)
    //    {
    //        //Debug.Log("added" + condition.ConditionName);

    //        conditions.Add(condition);
    //        //Debug.Log("first condition: " + conditions[0].Name);

    //    }
    //    yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));
    //    if (condition.EffectedAction != "None")
    //    {
    //        if (isPlayer)
    //        {
    //            yield return StartCoroutine(deckManager.UpdateCardsDisplay(condition.EffectedAction));
    //        }
    //        else
    //        {
    //            yield return StartCoroutine(GetComponent<AIFigure>().UpdatePlan());

    //        }
    //    }
    //}

    //returns targets chosen by game (i think priority is closest then arbitrary)
    public List<Figure> FindTargets(string targetType, int range = 1, int targets = 1)
    {
        return ChooseTargets(FindPosibleTargets(targetType, range), targets);
    }
    public List<Figure> FindPosibleTargets(string targetType, int range = 1)
    {
        List<Figure> targetableFigures = new List<Figure>();
        List<Figure> posibleTargets = pathfinder.GetFiguresInRange(oneToOnePos, range, gameObject);
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
        else if (targetType == "self or ally")
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
        return targetableFigures;
    }

    public List<Figure> ChooseTargets(List<Figure> posibleTargets, int targets = 1)
    {
        if (targets == Variables.gameInfinityValue)
        {
            return posibleTargets;
        }
        List<Figure> targetedFigures = new List<Figure>();
        foreach (Figure posibletarget in posibleTargets)
        {
            if (targets > 0)
            {
                targetedFigures.Add(posibletarget);
                targets--;
            }
        }
        return targetedFigures;
    }
    public int FindValueOfCondition(string conditionName)
    {
        int value = 0;
        foreach (Condition condition in conditions)
        {
            if (condition.ConditionName == conditionName)
            {
                //if (value == Variables.gameNullValue)
                //{
                //    value = 0;
                //}
                value += condition.Value;
            }
        }
        if (value == 0)
        {
            return Variables.gameDoesNotExistIndcator;
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
            attackValue += playerControler.MortarCount * (pathfinder.GetDistanceTo(playerControler.OneToOnePos, oneToOnePos) - 1);
        }
        for (int i = 0; i < repeats; i++)
        {
            yield return gameManager.StartCoroutine(TakeDamage(attackValue));
            if (!isDead)
            {
                yield return StartCoroutine(GainConditions(newConditions));
            }
        }


    }
    public IEnumerator TakeDamage(int damageValue)
    {
        if (block > 0)
        {
            int damageBlocked = Mathf.Min(damageValue, block);
            damageValue -= damageBlocked;
            block -= damageBlocked;
        }
        OverallStatistics.damageDealt += damageValue;
        //Debug.Log(gameObject.name + " took damage");
        yield return gameManager.StartCoroutine(LoseHealth(damageValue));
    }
    public virtual IEnumerator LoseHealth(int amount)
    {
        //Debug.Log(gameObject.name + " Lost Health");
        health -= amount;
        statsDisplayer.SetHealthAndBlock(health, maxHealth, block);
        if (health <= 0)
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
        //    //actionManager.QueueAction(playerControler.ApplyCondition(new StartOfTurnBlock(Variables.adaptiveShieldBlock * adaptiveShieldCount)));
        //    //Debug.Log("queued block nexty turn");
        //    //playerControler.ApplyCondition(new StartOfTurnSlow(Variables.frozenLensSpeedLoss, -1)), relicDescriptionList))
        //    //yield return StartCoroutine(actionManager.QueueAction(Block(Variables.adaptiveShieldBlock * adaptiveShieldCount)));
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
