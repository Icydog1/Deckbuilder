using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Enemy : Figure
{

    //protected Vector3 relativeHexPosToPlayer;
    private string enemyName;
    protected delegate void moveSetsMethod();
    //protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected List<List<Func<IEnumerator>>> moveSets = new List<List<Func<IEnumerator>>>();
    protected List<int> movesSetOrder = new List<int>() { -1};
    protected int currentmove = 0;
    protected List<Func<IEnumerator>> currentPlan = new List<Func<IEnumerator>>();
    protected List<Func<IEnumerator>> plannedMoveSet;
    protected List<string> displayedPlan = new List<string>();
    private Coroutine currentTurnRoutine;
    protected int actionNum;
    protected EnemyUi enemyStatsDisplayer;
    //protected FigureStorage figureStorage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        //figureStorage = GameObject.Find("FigureStorage").GetComponent<FigureStorage>();
        LevelManager.LevelCleared += Remove;
        base.Awake();
        enemyName = this.name;
        enemyName = enemyName.Replace("(Clone)", "");
        enemyName = Regex.Replace(enemyName, "(.)([A-Z,0-9])", "$1 $2");
        transform.Find("EnemyUI").transform.Find("NameText").gameObject.GetComponent<TextMeshProUGUI>().SetText(enemyName);
        //figureStorage.Enemies.Add(gameObject);
        
    }
    public override void Start()
    {

    }

    public override IEnumerator LoadFigure()
    {
        if (moveSets.Count == 0)
        {
            Debug.Log("Warning no planed movesets on " + gameObject);
        }
        enemyStatsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        statsDisplayer = enemyStatsDisplayer;
        isEnemy = true;
        yield return StartCoroutine(base.LoadFigure());

        team = 1;
        turnManager.TurnOrder.Add(gameObject);
        health = maxHealth;
        TurnManager.RoundStarted += GetNewPlan;
        yield return StartCoroutine(GetNewPlan(null));
    }
    public IEnumerator GetNewPlan(TurnManager turnManager)
    {
        oneToOnePos = mapManager.PosToOneToOne(transform.position);
        int distanceToPlayer = pathfinder.GetDistanceTo(playerControler.OneToOnePos, oneToOnePos);
        if (distanceToPlayer >= 20)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new DistanceSpeedBoost(distanceToPlayer - 20))));

        }
        if (distanceToPlayer >= 50)
        {
            yield return StartCoroutine(actionManager.PreformAction(GainCondition(new DistanceJump())));
            Debug.Log("need to ModifyJump");
            //GainCondition(new DistanceJump());
        }
        //actionManager.PlanToList = displayedPlan;
        currentPlan.Clear();
        displayedPlan.Clear();
        if (currentmove == movesSetOrder.Count)
        {
            currentmove = 0;
        }
        if (movesSetOrder[currentmove] == -1)
        {
            plannedMoveSet = moveSets[UnityEngine.Random.Range(0, moveSets.Count)];
        }
        else
        {
            //Debug.Log(movesSetOrder[currentmove]);
            plannedMoveSet = moveSets[movesSetOrder[currentmove]];
        }
        currentPlan = new List<Func<IEnumerator>>(plannedMoveSet);
        //Debug.Log("gotInitialPlan");
        levelManager.GetDifficultyModifier(this);
        yield return StartCoroutine(UpdatePlan());

        //UpdatePlan();
    }

    public IEnumerator UpdatePlan()
    {
        //Debug.Log("first condition: " + conditions[0].Name);
        //actionManager.PlanToList = displayedPlan;
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
    public IEnumerator StartOfTurn()
    {
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
        //yield return new WaitUntil(() => nextAction == true);
        //nextAction = false;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            yield return StartCoroutine(actionManager.PreformAction(currentPlan[i]()));
            //yield return new WaitUntil(() => nextAction == true);
            //nextAction = false;
        }
        currentmove++;
        yield return StartCoroutine(EndTurn());
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
        distanceToPlayer = mapManager.GetDistanceBetweenOneToOne(oneToOnePos, playerControler.OneToOnePos);
    }
    public override void ActionDone()
    {
        CalculateValues();
        nextAction = true;
    }
    public void showHideTooltip(bool show)
    {

    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void Remove(LevelManager levelManager = null)
    {
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        if (isMyTurn)
        {
            StartStopTurn(false);
            turnManager.NextTurn();
        }
        TurnManager.RoundStarted -= GetNewPlan;
        LevelManager.LevelCleared -= Remove;
        turnManager.RemoveFromTurnOrder(gameObject);

    }
}
