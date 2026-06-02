using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    protected LevelManager levelManager;
    protected ActionManager actionManager;
    protected OverallStatistics overallStatistics;
    protected GameManager gameManager;



    protected bool isMyTurn;
    protected bool isEnemy, isPlayer;
    protected bool isPlanning, isPreparingMove;
    public bool IsPlanning { set { isPlanning = value; } get { return isPlanning; } }

    protected Vector2 oneToOnePos;
    public Vector2 OneToOnePos { get { return oneToOnePos; } set { oneToOnePos = value; } }

    protected int preferedRange;
    protected float distanceToPlayer;
    protected bool nextAction;
    protected bool isDead;
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
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
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
        statsDisplayer.SetHealthAndBlock(health, block);
        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));

        yield return null;
    }

    public IEnumerator baseStartTurn()
    {
        //Debug.Log(gameObject + " is takeing turn");
        block = 0;
        statsDisplayer.SetHealthAndBlock(health, block);
        yield return StartCoroutine(conditionEffects.StartOfTurnConditons(this));
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
        List<Action> planDescription = new List<Action>();
        foreach (Func<IEnumerator> action in actions)
        {
            yield return StartCoroutine(actionManager.PreformAction(action(), planDescription));
        }
        string displayedString = "";
        foreach (Action text in planDescription)
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

        if (isPlanning)
        {

            //string currentDescriptionString = finalBlock + " <sprite name=Block>";
            Action currentAction = new Action("Block", new List<ActionModifier>() { new ActionModifier(this, null, blockValue, " <sprite name=Block>", "Block") });
            actionManager.PlanToList.Add(currentAction);
        }
        else if (!isPreparingMove)
        {
            if (isVariable)
            {
                blockValue *= variableCardModifier;
            }
            int finalBlock = conditionEffects.ModifyBlock(this, blockValue);
            actionManager.ActionStackNames.Push("Block");
            block += finalBlock;
            statsDisplayer.SetHealthAndBlock(health, block);
            //ActionDone();
            EndAction();

        }
        yield return null;
    }

    public IEnumerator Attack(int attackValue, int attackRange = 1, int targets = 1, int repeats = 1, Condition[] attackConditions = null, bool isVariable = false, bool isManual = true)
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
        if (isPlanning)
        {
            List<ActionModifier> actionModifiers = new List<ActionModifier>();
            //ActionModifier attackValueDescription = new ActionModifier(this, null, attackValue, " <sprite name=Attack>", "Attack");
            actionModifiers.Add(new ActionModifier(this, null, attackValue, " <sprite name=Attack>", "Attack"));
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
            
            //currentDescriptionStart = finalAttack + " <sprite name=Attack>";

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
                actionModifiers.Add(new ActionModifier(this, " and apply " + conditionString, valueType: "Conditions"));

            }
            if (repeats > 1)
            {
                //currentDescriptionEnd += " " + repeats + " times ";
                actionModifiers.Add(new ActionModifier(this, " ", repeats, " times", "Repeats"));

            }
            if (targets > 1)
            {
                //currentDescriptionEnd += " target " + targets;
                actionModifiers.Add(new ActionModifier(this, " ", targets, " targets", "Targets"));
            }
            else if (targets == -1)
            {
                actionModifiers.Add(new ActionModifier(this, " all targets", valueType: "Targets"));

            }
            if (attackRange > 1)
            {
                //currentDescriptionEnd += " " + attackRange + " <sprite name=Range>";
                actionModifiers.Add(new ActionModifier(this, " ", attackRange, " <sprite name=Range>", "Range"));
            }
            //tring separator = ", ";
            //string attackText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            //actionManager.PlanToList.Add(attackText);

            Action currentAction = new Action("Attack", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
        }
        else if(!isPreparingMove)
        {
            actionManager.ActionStackNames.Push("Attack");
            if (isPlayer && isManual)
            {
                yield return StartCoroutine(playerControler.ControledAttack(finalAttack, attackRange, targets, repeats, attackConditions));
            }
            else
            {
                foreach (Figure target in FindTargets("enemy", attackRange, targets))
                {
                    Debug.Log(target);
                    yield return gameManager.StartCoroutine(target.AttackedFor(finalAttack, repeats, attackConditions));
                }
                //ActionDone();
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].ConditionName == "Vigor")
                    {
                        yield return StartCoroutine(conditions[i].OnLoss(this));
                        conditions.RemoveAt(i);
                        yield return StartCoroutine(GetComponent<Enemy>().UpdatePlan());
                        yield return StartCoroutine(statsDisplayer.DisplayConditions(conditions));

                    }
                }
                EndAction();
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
            ActionModifier moveValueDescription = new ActionModifier(this, null, moveValue, " <sprite name=Move>", "Move");
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
            Action currentAction = new Action("Move", actionModifiers);
            actionManager.PlanToList.Add(currentAction);
        }
        else if (isPreparingMove)
        {
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            List<Vector2>[] posibleTiles = pathfinder.PlanposiblePaths(oneToOnePos, gameObject, finalMove, finalJump, canFly);
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
            int finalMove = conditionEffects.ModifyMove(this, moveValue);
            actionManager.ActionStackNames.Push("Move");
            if (isPlayer)
            {
                yield return StartCoroutine(playerControler.ControledMove(finalMove, finalJump));
            }
            else
            {
                //Debug.Log(gameObject + " started pathfinding");
                if (distanceToPlayer < 50)
                {
                    //Debug.Log("old pathfining");
                    yield return StartCoroutine(pathfinder.PathfindTowards(oneToOnePos, playerControler.OneToOnePos, gameObject, finalMove, preferedRange, finalJump, canFly));
                }
                else
                {
                    //Debug.Log("new pathfining");
                    yield return StartCoroutine(pathfinder.NewPathfindTowards(oneToOnePos, playerControler.OneToOnePos, gameObject, finalMove, preferedRange, finalJump, canFly));

                }
                //Debug.Log(gameObject + " finished pathfinding");
                EndAction();

            }
        }
        //yield return null;

    }

    public IEnumerator ApplyCondition(Condition condition, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false, bool isManual = true)
    {
        yield return StartCoroutine(ApplyConditions(new Condition[] { condition }, targetType, range, targets, displayTarget, isManual));
    }

    public IEnumerator ApplyConditions(Condition[] newConditions, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false, bool isManual = true)
    {
        if (isPlanning)
        {
            List<Action> individualConditionText = new List<Action>();
            string currentDescriptionStart = "";
            string currentDescriptionEnd = "";
            bool abnormal = false;
            foreach (Condition condition in newConditions)
            {
                if (condition.Abnormality != null)
                {
                    foreach (string abnormality in condition.Abnormality)
                    {
                        actionAbnormalities.Add(abnormality);

                    }
                    abnormal = true;
                }
                if (actionAbnormalities.Contains("Ability"))
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
                    List<Action> conditionPlan = new List<Action>();

                    foreach (Func<IEnumerator> action in condition.Plan)
                    {
                        yield return StartCoroutine(actionManager.PreformAction(action(), conditionPlan));
                        //if (condition.Plan[0] != action)
                        //{
                        //    conditionPlan[conditionPlan.Count - 2] = conditionPlan[conditionPlan.Count - 2] + " and " + conditionPlan[conditionPlan.Count - 1];
                        //    conditionPlan.RemoveAt(conditionPlan.Count - 1);
                        //}
                    }
                    //currentDescriptionString = string.Join(" and ", conditionPlan);
                    //individualConditionText.Add(currentDescriptionString);
                    //condition.Plan();
                    if (actionAbnormalities.Contains("Delayed Gain"))
                    {
                        if (conditionPlan.Count > 0)
                        {
                            if (condition.Duration == 1)
                            {
                                //actionManager.PlanToList.Add("Next turn");

                                //individualConditionText[individualConditionText.Count - 1] = "Next turn " + individualConditionText[individualConditionText.Count - 1];
                                conditionPlan[0].ActionModifiers.Insert(0, new ActionModifier(this, "Next turn "));

                            }
                            else if (condition.Duration != -1)
                            {
                                //actionManager.PlanToList[individualConditionText.Count - 1] = "At the start of the next " + condition.Duration + " turns" + individualConditionText[individualConditionText.Count - 1];
                                conditionPlan[0].ActionModifiers.Insert(0, new ActionModifier(this, "At the start of the next ", condition.Duration, " turns"));

                                //Action currentAction = new Action("BottemEnergy", new List<ActionModifier>() { new ActionModifier("Gain ", amount, " bottom energy") });
                                //actionManager.PlanToList.Add(currentAction);
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
                    foreach (Action action in conditionPlan)
                    {
                        individualConditionText.Add(action);

                    }
                }

                if (!actionAbnormalities.Contains("Delayed Gain") && !actionAbnormalities.Contains("Ability"))
                {
                    if (actionAbnormalities.Contains("No Value Description"))
                    {
                        currentDescriptionString = condition.ActionName;
                    }
                    else
                    {
                        currentDescriptionString = condition.Value + " " + condition.ActionName;

                    }
                    if (condition.Duration == 1)
                    {
                        currentDescriptionString += " this turn";
                    }
                    else if (condition.Duration != -1)
                    {
                        currentDescriptionString += " for " + condition.Duration + " turns";
                    }
                    //individualConditionText.Add(currentDescriptionString);

                    Action currentAction2 = new Action("Ability", new List<ActionModifier>() { new ActionModifier(this, currentDescriptionString) });
                    individualConditionText.Add(currentAction2);
                }
            }
            if (targetType == "self")
            {
                if (!actionAbnormalities.Contains("No Self Target Description"))
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
            else
            {
                currentDescriptionStart += "Apply ";
                if (targets != 1)
                {
                    currentDescriptionEnd += " target ";
                }
                if (targets == 1)
                {
                    
                }
                else if (targets == int.MaxValue)
                {
                    currentDescriptionEnd += "all";
                }
                else
                {
                    currentDescriptionEnd += targets;
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
                if (range == -1)
                {
                    currentDescriptionEnd += " any <sprite name=Range>";
                }
                else if (range != 1)
                {
                    currentDescriptionEnd += " " + range + " <sprite name=Range>";
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

            Action currentAction = new Action("Ability", new List<ActionModifier>() { new ActionModifier(this, conditionText) });
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

            if (isPlayer && isManual)
            {
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
                else
                {
                    yield return StartCoroutine(playerControler.ControledApplyConditions(newConditions, targetType, range, targets));
                }
            }
            else
            {
                foreach (Figure target in FindTargets(targetType, range, targets))
                {
                    foreach (Condition condition in newConditions)
                    {
                        //Debug.Log("applieing Condition");

                        yield return StartCoroutine(target.GainCondition(condition));
                        //Debug.Log("applied Condition");

                    }
                }
                EndAction();
                //if (isAction)
                //{
                //    ActionDone();
                //}
            }
        }

    }
    public IEnumerator Summon(GameObject summon)
    {
        //if (isVariable)
        //{
        //    blockValue *= variableCardModifier;
        //}
        //int finalBlock = conditionEffects.ModifyBlock(this, blockValue);
        if (isPlanning)
        {
            //prepareActions.Add(() => Block(finalBlock));
            //string currentDescriptionString = "Block " + finalBlock;
            string currentDescriptionString = "Summon " + summon.name;
            //Debug.Log("planned " + currentDescriptionString);
            //actionManager.PlanToList.Add(currentDescriptionString);

            Action currentAction = new Action("Lockpick", new List<ActionModifier>() { new ActionModifier(this, "Summon " + summon.name) });
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
            if (canSummon)
            {
                GameObject newSummon = Instantiate(summon, new Vector3(summonPos.x, summonPos.y, summon.transform.position.z), Quaternion.identity);
            }
            //ActionDone();
            EndAction();

        }
        yield return null;

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
    public IEnumerator GainCondition(Condition condition)
    {
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
                    break;
                }
                if (condition.AddType == 2 && conditions[i].Value == condition.Value)
                {
                    if (conditions[i].Duration == -1 || condition.Duration == -1)
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
                yield return StartCoroutine(GetComponent<Enemy>().UpdatePlan());

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
        else if (targetType == "self and ally")
        {
            foreach (Figure posibletarget in posibleTargets)
            {
                if (posibletarget.team == team)
                {
                    targetableFigures.Add(posibletarget);
                }

            }
        }
        return targetableFigures;
    }

    public List<Figure> ChooseTargets(List<Figure> posibleTargets, int targets = 1)
    {
        if (targets == -1)
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
    public IEnumerator AttackedFor(int attackValue, int repeats, Condition[] newConditions)
    {
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
        //Debug.Log(gameObject.name + " took damage");
        yield return gameManager.StartCoroutine(LoseHealth(damageValue));
    }
    public virtual IEnumerator LoseHealth(int amount)
    {
        //Debug.Log(gameObject.name + " Lost Health");
        health -= amount;
        statsDisplayer.SetHealthAndBlock(health, block);
        if (health <= 0)
        {
            isDead = true;
            Die();
        }
        yield break;

    }
    public void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        statsDisplayer.SetHealthAndBlock(health, block);
    }
    public virtual void Die()
    {
        Debug.Log("Base Die ran");
    }
    public virtual void Remove(LevelManager levelManager = null)
    {
        Debug.Log("Base Remove ran");
    }

    public virtual IEnumerator MoveOneSpace()
    {
        yield break;
    }
}
