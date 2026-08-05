using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    protected int XPValue;
    protected bool isBoss;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        //figureStorage = GameObject.Find("FigureStorage").GetComponent<FigureStorage>();
        FloorManager.FloorCleared += Remove;
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
        //if (moveSets.Count == 0)
        //{
        //    Debug.Log("Warning no planed movesets on " + gameObject);
        //}
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
                    turnManager.TurnOrder.Insert(i, gameObject);
                    hasTurnOrder = true;
                    break;
                }
            }
            if (hasTurnOrder == false)
            {
                turnManager.TurnOrder.Add(gameObject);
            }
        }
        TurnManager.RoundStarted += GetNewPlan;
        yield return StartCoroutine(GetNewPlan(null));
        //Debug.Log("Finished loading");

    }
    public IEnumerator FindFocus()
    {
        List<Figure> posibleTargets = FindTargets("enemy", Var.infinityValue);
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
        //if (!isEnemy)
        //{
        //    Debug.Log(gameObject + "Getting plan");
        //}
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
        //yield return StartCoroutine(floorManager.GetDifficultyModifier(this));
        yield return StartCoroutine(UpdatePlan());

        //UpdatePlan();
        //Debug.Log("Finished getting plan");

    }

    public IEnumerator UpdatePlan()
    {
        //Debug.Log("first condition: " + conditions[0].Name);
        //actionManager.PlanToList = displayedPlan;
        if (FindValueOfCondition("Stunned") != 0 || (turn == 0 && isSummon))
        {
            currentPlan = new List<Func<IEnumerator>>();
            //Debug.Log("is stunned");
        }
        displayedPlan.Clear();
        preferedRange = int.MaxValue;
        isPlanning = true;
        //if (!isEnemy)
        //{
        //    Debug.Log("has " + currentPlan.Count + " actions in plan");

        //}


        for (int i = 0; i < currentPlan.Count; i++)
        {
            yield return StartCoroutine(actionManager.PreformAction(currentPlan[i](), displayedPlan,this));
        }
        //if (!isEnemy)
        //{
        //    if (displayedPlan.Count > 0)
        //    {
        //        Debug.Log("first element in plan " + displayedPlan[0].GetDescription());
        //    }
        //    else
        //    {
        //        Debug.Log("no plan");
        //    }
        //}

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
                case "RangeValue":
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
        //nextAction = true;
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
        yield return StartCoroutine(StartOfTurn());
        if (!(turn == 1 && isSummon))
        {
            //yield return new WaitUntil(() => nextAction == true);
            //nextAction = false;
            //Debug.Log(gameObject + " started main action sequence");

            for (int i = 0; i < currentPlan.Count; i++)
            {
                if (exists)
                {
                    yield return StartCoroutine(actionManager.PreformAction(currentPlan[i]()));
                    displayedPlan.RemoveAt(0);
                    enemyStatsDisplayer.Plan(displayedPlan);
                }
            }

            if (FindValueOfCondition("Stunned") == 0)
            {
                currentmove++;
            }
        }
        yield return StartCoroutine(EndTurn());

    }
    public IEnumerator DisplayMovePosibilities()
    {
        isPreparingMove = true;
        movePosibilities.Clear();
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
        //nextAction = true;
    }
    //public override void EndAction()
    //{
    //    //CalculateValues();
    //    //nextAction = true;
    //    Debug.Log("ended action");
    //}
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
        yield return gameManager.StartCoroutine(base.Die());
    }

    public override IEnumerator Remove(FloorManager floorManager = null)
    {
        if (isMyTurn)
        {
            StartStopTurn(false);
            turnManager.NextTurn();
        }
        yield return gameManager.StartCoroutine(base.Remove());
    }
    public override IEnumerator StopExisting()
    {
        TurnManager.RoundStarted -= GetNewPlan;
        FloorManager.FloorCleared -= Remove;
        //playerControler.PlayedCardScript.ActingFigures.Remove(this);
        //Debug.Log(gameObject);
        turnManager.RemoveFromTurnOrder(gameObject);
        yield return gameManager.StartCoroutine(base.StopExisting());
    }

}

