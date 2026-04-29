using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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
    protected ActionModifier actionModifier;
    protected DeckManager deckManager;
    protected LevelManager levelManager;


    protected bool isMyTurn;
    protected bool isEnemy, isPlayer;
    protected bool isPlanning;
    public bool IsPlanning { set { isPlanning = value; } get { return isPlanning; } }

    protected Vector2 oneToOnePos;
    public Vector2 OneToOnePos { get { return oneToOnePos; } set { oneToOnePos = value; } }

    protected int preferedRange;
    protected float distanceToPlayer;
    protected bool nextAction;

    protected int team;
    public int Team { get { return team; } }
    protected bool isPreformingAnimation;
    public bool IsPreformingAnimation { set { isPreformingAnimation = value; } get { return isPreformingAnimation; } }

    protected int maxHealth = 1, health, block = 0;
    protected bool canFly = false;

    protected List<string> planDescription = new List<string>();
    public List<string> PlanDescription { set { planDescription = value; } }
    protected List<System.Action> prepareActions = new List<System.Action>();
    public List<System.Action> PrepareActions { set { prepareActions = value; } }

    protected List<Condition> conditions = new List<Condition>();
    public List<Condition> Conditions { set { conditions = value; } get { return conditions; } }

    protected int variableCardModifier;
    public int VariableCardModifier { get { return variableCardModifier; } set { variableCardModifier = value; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        actionModifier = GameObject.Find("ActionModifier").GetComponent<ActionModifier>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        //statsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
    }
    public virtual void Start()
    {
        health = maxHealth;


        statsDisplayer.SetHealthAndBlock(health, block);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    public void baseStartTurn()
    {

        block = 0;
        statsDisplayer.SetHealthAndBlock(health, block);

    }
    public void baseEndTurn()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
            }
            if (conditions[i].Duration == 0)
            {
                conditions.Remove(conditions[i]);
                i--;
            }
        }
        if (isPlayer)
        {
            deckManager.UpdateCardsDisplay();
        }
        statsDisplayer.DisplayConditions(conditions);
        turnManager.NextTurn();
    }

    public virtual void ActionDone()
    {
        Debug.Log("Base ActionDone ran");
    }


    public string GetPlanString(List<System.Action> actions)
    {
        List<string> currentPlanDescription = planDescription;
        bool currentPlanningState = isPlanning;
        planDescription = new List<string>();
        isPlanning = true;
        foreach (System.Action action in actions)
        {
            action();
        }
        string displayedString = "";
        foreach (string text in planDescription)
        {
            displayedString += text;
            displayedString += " ";
        }
        planDescription = currentPlanDescription;
        isPlanning = currentPlanningState;
        return displayedString;
    }
    public void Block(int blockValue, bool isVariable = false)
    {
        if (isVariable)
        {
            blockValue *= variableCardModifier;
        }
        int finalBlock = actionModifier.ModifyBlock(this, blockValue);
        if (isPlanning)
        {
            //prepareActions.Add(() => Block(finalBlock));
            string currentDescriptionString = "Block " + finalBlock;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            block += finalBlock;
            statsDisplayer.SetHealthAndBlock(health, block);
            ActionDone();
        }
    }

    public void Attack(int attackValue, int attackRange = 1, int targets = 1, int repeats = 1, Condition[] attackConditions = null, bool isVariable = false)
    {
        if (isVariable)
        {
            attackValue *= variableCardModifier;
        }
        if (attackConditions == null)
        {
            attackConditions = new Condition[0];
        }
        int finalAttack = actionModifier.ModifyAttack(this, attackValue);
        if (isPlanning)
        {
            string currentDescriptionStart = "";
            string currentDescriptionEnd = "";
            if (!isPlayer)
            {
                if (preferedRange > attackRange)
                {
                    preferedRange = attackRange;
                }
            }
            //prepareActions.Add(() => Attack(finalAttack, attackRange, targets));
            currentDescriptionStart = "Attack " + finalAttack;
            if (repeats > 1)
            {
                currentDescriptionEnd += " " + repeats + " times ";
            }
            if (attackRange > 1)
            {
                currentDescriptionEnd += " range " + attackRange;
            }
            if (targets > 1)
            {
                currentDescriptionEnd += " target " + targets;
            }
            List<string> individualConditionText = new List<string>();
            if (attackConditions.Length != 0)
            {
                currentDescriptionStart += " and apply ";
                foreach (Condition condition in attackConditions)
                {
                    string currentDescriptionString = currentDescriptionString = condition.Value + " " + condition.Name;
                    if (condition.Duration == 1)
                    {
                        currentDescriptionString += " this turn";
                    }
                    else if (condition.Duration != -1)
                    {
                        currentDescriptionString += " for " + condition.Duration + " turns";
                    }
                    individualConditionText.Add(currentDescriptionString);
                }
            }
            string separator = ", ";
            string attackText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            planDescription.Add(attackText);
        }
        else
        {
            if (isPlayer)
            {
                playerControler.ControledAttack(finalAttack, attackRange, targets, attackConditions);
            }
            else
            {
                foreach (Figure target in FindTargets("enemy", attackRange, targets))
                {
                    target.AttackedFor(finalAttack, attackConditions);
                }
                ActionDone();
            }

        }
    }
    public void Move(int moveValue, bool isJump = false, bool isVariable = false)
    {
        if (isVariable)
        {
            moveValue *= variableCardModifier;
        }
        int finalMove = actionModifier.ModifyMove(this, moveValue);
        bool finalJump = actionModifier.ModifyJump(this, isJump);
        //Mathf(finalMove,0,)
        if (isPlanning)
        {
            //prepareActions.Add(() => Move(finalMove, isJump));
            string planString = "Move " + finalMove;
            if (finalJump)
            {
                planString += " Jump";
            }
            planDescription.Add(planString);
        }
        else
        {
            if (isPlayer)
            {
                playerControler.ControledMove(finalMove, finalJump);
            }
            else
            {
                StartCoroutine(pathfinder.PathfindTowards(oneToOnePos, playerControler.OneToOnePos, gameObject, finalMove, preferedRange, finalJump, canFly));
            }
        }
    }
    public void ApplyCondition(Condition condition, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false)
    {
        ApplyConditions(new Condition[] { condition }, targetType, range, targets, displayTarget);
    }

    public void ApplyConditions(Condition[] newConditions, string targetType = "self", int range = 1, int targets = 1, bool displayTarget = false)
    {
        if (isPlanning)
        {
            List<string> individualConditionText = new List<string>();
            string currentDescriptionStart = "";
            string currentDescriptionEnd = "";
            if (targetType == "self")
            {
                currentDescriptionStart += "Gain ";
                
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

                currentDescriptionEnd += " target ";

                if (targets == int.MaxValue)
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
                currentDescriptionEnd += " range " + range;
                if (!isPlayer)
                {
                    if (preferedRange > range && targetType == "enemy")
                    {
                        preferedRange = range;
                    }
                }
            }
            foreach (Condition condition in newConditions)
            {
                string currentDescriptionString = currentDescriptionString = condition.Value + " " + condition.Name;
                if (condition.Duration == 1)
                {
                    currentDescriptionString += " this turn";
                }
                else if (condition.Duration != -1)
                {
                    currentDescriptionString += " for " + condition.Duration + " turns";
                }
                individualConditionText.Add(currentDescriptionString);
            }
            string separator = ", ";
            string conditionText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            planDescription.Add(conditionText);
        }
        else
        {
            if (isPlayer)
            {
                if (targetType == "self")
                {
                    GainConditions(newConditions);
                    ActionDone();
                }
                else
                {
                    playerControler.ControledApplyConditions(newConditions, targetType, range, targets);
                }
            }
            else
            {
                foreach (Figure target in FindTargets(targetType, range, targets))
                {
                    foreach (Condition condition in newConditions)
                    {
                        target.GainCondition(condition);
                    }
                }
                ActionDone();
            }
        }
    }

    public void GainConditions(Condition[] newConditions)
    {
        foreach (Condition condition in newConditions)
        {
            GainCondition(condition);
        }
    }
    public void GainCondition(Condition condition)
    {
        //Debug.Log("GainedCondition");
        bool isDuplicate = false;
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].Name == condition.Name)
            {
                if (condition.AddType == 1 && conditions[i].Duration == condition.Duration)
                {
                    conditions[i].Value += condition.Value;
                    isDuplicate = true;
                    break;
                }
                if (condition.AddType == 2 && conditions[i].Value == condition.Value)
                {
                    conditions[i].Duration += condition.Duration;
                    isDuplicate = true;
                    break;
                }
                if (condition.AddType == 3)
                {
                    //Debug.Log("removed" + conditions[i].Name);
                    conditions.Remove(conditions[i]);
                    i--;
                }
            }

        }
        if (isDuplicate == false)
        {
            //Debug.Log("added" + condition.Name);

            conditions.Add(condition);
            //Debug.Log("first condition: " + conditions[0].Name);

        }
        if (isPlayer)
        {
            deckManager.UpdateCardsDisplay();
        }
        //Debug.Log("first condition: " + conditions[0].Name);

        statsDisplayer.DisplayConditions(conditions);
        //Debug.Log("first condition: " + conditions[0].Name);

        if (!isPlayer)
        {
            GetComponent<Enemy>().UpdatePlan();
        }
    }
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
    public void AttackedFor(int attackValue, Condition[] newConditions)
    {
        if (block > 0)
        {
            int damageBlocked = Mathf.Min(attackValue, block);
            attackValue -= damageBlocked;
            block -= damageBlocked;
        }
        health -= attackValue;
        statsDisplayer.SetHealthAndBlock(health, block);
        if (health <= 0)
        {
            Die();
        }
        else
        {
            GainConditions(newConditions);
        }
    }
    public virtual void Die()
    {
        Debug.Log("Base Die ran");
    }
    public virtual void Remove(LevelManager levelManager = null)
    {
        Debug.Log("Base Remove ran");
    }
}
