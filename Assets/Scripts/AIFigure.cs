using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class AIFigure : Figure
{

    //protected Vector3 relativeHexPosToPlayer;
    protected string figureName;
    protected delegate void moveSetsMethod();
    //protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected List<List<Func<IEnumerator>>> moveSets = new List<List<Func<IEnumerator>>>();
    protected List<int> movesSetOrder = new List<int>() { -1 };
    protected List<int> initialMoves = new List<int>();
    protected int currentmove = 0;
    protected List<Func<IEnumerator>> currentPlan = new List<Func<IEnumerator>>();
    protected List<Func<IEnumerator>> plannedMoveSet;
    protected List<ActionDescription> displayedPlan = new List<ActionDescription>();
    protected Coroutine currentTurnRoutine;
    protected int actionNum;
    protected EnemyUi enemyStatsDisplayer;
    //protected FigureStorage figureStorage;
    protected int XPValue = 5;
    protected bool isBoss;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        //figureStorage = GameObject.Find("FigureStorage").GetComponent<FigureStorage>();
        LevelManager.LevelClearedFuntions += Remove;
        figureName = this.name;
        figureName = figureName.Replace("(Clone)", "");
        figureName = Regex.Replace(figureName, "(.)([A-Z,0-9])", "$1 $2");
        transform.Find("EnemyUI").transform.Find("NameText").gameObject.GetComponent<TextMeshProUGUI>().SetText(figureName);
        //figureStorage.Enemies.Add(gameObject);
        enemyStatsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        isAI = true;
        statsDisplayer = enemyStatsDisplayer;
        base.Awake();

    }

    public override IEnumerator LoadFigure()
    {
        if (moveSets.Count == 0)
        {
            Debug.Log("Warning no planed movesets on " + gameObject);
        }
        //yield return StartCoroutine(actionManager.PreformAction(GainCondition(new Flight())));
        if (isEnemy && !isBoss)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new NaturalScaling(OverallStatistics.enemyScaling))));
        }
        maxHealth = conditionEffects.ModifyMaxHealth(this, maxHealth);

        yield return StartCoroutine(base.LoadFigure());
        if (isEnemy)
        {
            turnManager.TurnOrder.Add(gameObject);
        }
        else
        {
            bool hasTurnOrder = false;
            for (int i = 0; i < turnManager.TurnOrder.Count; i++)
            {
                if (turnManager.TurnOrder[i].GetComponent<Enemy>())
                {
                    turnManager.TurnOrder.Insert(i - 1, gameObject);
                    hasTurnOrder = true;
                    break;
                }
            }
            if (hasTurnOrder == false)
            {
                turnManager.TurnOrder.Add(gameObject);
            }
        }

        //health = maxHealth;
        TurnManager.RoundStarted += GetNewPlan;
        yield return StartCoroutine(GetNewPlan(null));
        //Debug.Log("Finished loading");

    }
    public IEnumerator FindFocus()
    {
        List<Figure> posibleTargets = FindTargets("enemy", Variables.gameInfinityValue);
        if (posibleTargets.Count > 0)
        {
            focusScript = posibleTargets[0];
            focus = focusScript.gameObject;
            hasFocus = true;
        }
        else
        {
            focusScript = playerControler;
            focus = focusScript.gameObject;
            hasFocus = false;
        }
        //Debug.Log(focus);
        yield break;
    }

    public IEnumerator GetNewPlan(TurnManager turnManager)
    {
        //Debug.Log("Geting plan");
        oneToOnePos = mapManager.PosToOneToOne(transform.position);
        yield return StartCoroutine(FindFocus());
        distanceToFocus = pathfinder.GetDistanceTo(focusScript.OneToOnePos, oneToOnePos);
        if (distanceToFocus >= 20 && isEnemy)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new DistanceSpeedBoost(distanceToFocus - 20))));

        }
        if (distanceToFocus >= 50)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new DistanceJump())));
            //GainCondition(new DistanceJump());
        }
        //actionManager.PlanToList = displayedPlan;
        currentPlan.Clear();
        displayedPlan.Clear();
        //Debug.Log("new plan with move" + currentmove);
        if (initialMoves.Count > 0)
        {
            if (currentmove == initialMoves.Count)
            {
                //Debug.Log("Reset plan");
                currentmove = 0;
                initialMoves.Clear();
            }
        }
        else
        {
            if (currentmove == movesSetOrder.Count)
            {
                //Debug.Log("Reset plan");
                currentmove = 0;
            }
        }
        if (initialMoves.Count > 0)
        {
            //Debug.Log(gameObject + "Current enemy plan");
            if (initialMoves[currentmove] == -1)
            {
                //Debug.Log("random plan");

                plannedMoveSet = moveSets[UnityEngine.Random.Range(0, moveSets.Count)];
            }
            else
            {
                //Debug.Log(movesSetOrder[currentmove]);
                plannedMoveSet = moveSets[initialMoves[currentmove]];
            }
        }
        else
        {

            //Debug.Log(gameObject + "Current enemy plan");
            if (movesSetOrder[currentmove] == -1)
            {
                //Debug.Log("random plan");

                plannedMoveSet = moveSets[UnityEngine.Random.Range(0, moveSets.Count)];
            }
            else
            {
                //Debug.Log(movesSetOrder[currentmove]);
                plannedMoveSet = moveSets[movesSetOrder[currentmove]];
            }
        }
        currentPlan = new List<Func<IEnumerator>>(plannedMoveSet);
        //Debug.Log("gotInitialPlan");
        if (isEnemy && !isBoss)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new NaturalScaling(OverallStatistics.enemyScaling))));
        }
        //yield return StartCoroutine(levelManager.GetDifficultyModifier(this));
        yield return StartCoroutine(UpdatePlan());

        //UpdatePlan();
        //Debug.Log("Finished getting plan");

    }

    public IEnumerator UpdatePlan()
    {
        //Debug.Log("first condition: " + conditions[0].Name);
        //actionManager.PlanToList = displayedPlan;
        if (FindValueOfCondition("Stunned") != Variables.gameDoesNotExistIndcator)
        {
            currentPlan = new List<Func<IEnumerator>>();
        }
        displayedPlan.Clear();
        preferedRange = int.MaxValue;
        isPlanning = true;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            yield return StartCoroutine(actionManager.PreformAction(currentPlan[i](), displayedPlan));
        }
        enemyStatsDisplayer.Plan(displayedPlan);
        isPlanning = false;
        //Debug.Log("first condition: " + conditions[0].Name);

    }
    public IEnumerator UpdatePlanDiscription(string modifiedAction)
    {
        if (modifiedAction == "All")
        {
            yield return StartCoroutine(UpdatePlan());
        }
        else
        {
            //Debug.Log("changed card description " + modifiedAction);
            playerControler.UnmodifiedAction = false;
            string actionName = null;
            int modifierNum = 0;
            switch (modifiedAction)
            {
                case "BlockValue":
                    actionName = "Block";
                    modifierNum = 0;
                    break;
                case "AttackValue":
                    //Debug.Log("changed attack");
                    actionName = "Attack";
                    modifierNum = 0;
                    break;
                case "MoveValue":
                    //Debug.Log("changed Move");

                    actionName = "Move";
                    modifierNum = 0;
                    break;
                case "AbilityValue":
                    actionName = "Ability";
                    modifierNum = 0;
                    break;
                case "Range":
                    actionName = "Range";
                    modifierNum = 1;
                    break;
                default:
                    Debug.Log("Default");
                    modifierNum = 0;
                    break;
            }
            foreach (ActionDescription action in displayedPlan)
            {
                if (action.ActionName == actionName)
                {
                    action.ActionModifiers[modifierNum].UpdateValue();
                }
            }
            enemyStatsDisplayer.Plan(displayedPlan);
            yield break;
        }

    }
    public IEnumerator StartOfTurn()
    {
        //Debug.Log(gameObject + " started turn");
        yield return StartCoroutine(FindFocus());
        if (distanceToFocus >= 50)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new DistanceJump())));
            //GainCondition(new DistanceJump());
        }
        yield return StartCoroutine(baseStartTurn());
        if (preferedRange == int.MaxValue)
        {
            preferedRange = 1;
        }
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.white;
        CalculateValues();
        nextAction = true;
    }

    public IEnumerator EndTurn()
    {
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.black;
        //Debug.Log(gameObject + " ended turn");
        yield return StartCoroutine(baseEndTurn());
    }
    public void StartStopTurn(bool isStart)
    {
        if (isStart)
        {
            currentTurnRoutine = StartCoroutine(TakeTurn());
        }
        else if (currentTurnRoutine != null)
        {
            StopCoroutine(currentTurnRoutine);
            currentTurnRoutine = null;
        }
    }
    public IEnumerator TakeTurn()
    {
        //Debug.Log(gameObject + " started taking turn");
        yield return StartCoroutine(StartOfTurn());
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        //Debug.Log(gameObject + " started main action sequence");

        for (int i = 0; i < currentPlan.Count; i++)
        {
            yield return StartCoroutine(actionManager.PreformAction(currentPlan[i]()));
            displayedPlan.RemoveAt(0);
            enemyStatsDisplayer.Plan(displayedPlan);

            //Debug.Log(gameObject + " toook 1 action");

            //yield return new WaitUntil(() => nextAction == true);
            //nextAction = false;
        }
        //Debug.Log(gameObject + " ended main action sequence");
        if (FindValueOfCondition("Stunned") == Variables.gameDoesNotExistIndcator)
        {
            currentmove++;
        }
        yield return StartCoroutine(EndTurn());
        //Debug.Log(gameObject + " ended turn");

    }
    public IEnumerator DisplayMovePosibilities()
    {
        isPreparingMove = true;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            yield return StartCoroutine(actionManager.PreformAction(currentPlan[i]()));
        }
        isPreparingMove = false;
        yield return new WaitUntil(() => mouseManager.SelectedObject != gameObject);
        foreach (GameObject border in shownTileBorders)
        {
            border.GetComponent<SpriteRenderer>().color = Color.black;
        }
        shownTileBorders.Clear();
    }
    public void CalculateValues()
    {
        oneToOnePos = mapManager.PosToOneToOne(transform.position);
        distanceToFocus = pathfinder.GetDistanceTo(focusScript.OneToOnePos, oneToOnePos);
    }
    public override void ActionDone()
    {
        CalculateValues();
        nextAction = true;
    }
    public override void EndAction()
    {
        //CalculateValues();
        //nextAction = true;
    }
    public void showHideTooltip(bool show)
    {

    }

    public override IEnumerator Die()
    {
        if (isMyTurn)
        {
            StartStopTurn(false);
            turnManager.NextTurn();
        }
        TurnManager.RoundStarted -= GetNewPlan;
        LevelManager.LevelClearedFuntions -= Remove;
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
        yield return gameManager.StartCoroutine(base.Die());
    }

    public override void Remove(LevelManager levelManager = null)
    {
        Destroy(gameObject);
        if (isMyTurn)
        {
            StartStopTurn(false);
            turnManager.NextTurn();
        }
        TurnManager.RoundStarted -= GetNewPlan;
        LevelManager.LevelClearedFuntions -= Remove;
        turnManager.RemoveFromTurnOrder(gameObject);
        base.Remove();
    }
}
