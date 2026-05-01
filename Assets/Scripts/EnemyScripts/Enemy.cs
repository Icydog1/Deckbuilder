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
    protected List<List<System.Action>> moveSets = new List<List<System.Action>>();
    protected List<int> movesSetOrder = new List<int>() { -1};
    protected int currentmove = 0;
    protected List<System.Action> currentPlan = new List<System.Action>();
    protected List<System.Action> plannedMoveSet;
    protected List<string> displayedPlan = new List<string>();
    private Coroutine currentTurnRoutine;
    protected int actionNum;
    protected EnemyUi enemyStatsDisplayer;
    protected FigureStorage figureStorage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        figureStorage = GameObject.Find("FigureStorage").GetComponent<FigureStorage>();
        LevelManager.LevelCleared += Remove;
        base.Awake();
        enemyName = this.name;
        enemyName = enemyName.Replace("(Clone)", "");
        enemyName = Regex.Replace(enemyName, "(.)([A-Z,0-9])", "$1 $2");
        transform.Find("EnemyUI").transform.Find("NameText").gameObject.GetComponent<TextMeshProUGUI>().SetText(enemyName);
        figureStorage.Enemies.Add(gameObject);
    }
    public override void Start()
    {
        if (moveSets.Count == 0)
        {
            Debug.Log("Warning no planed movesets on " + gameObject);
        }
        enemyStatsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        statsDisplayer = enemyStatsDisplayer;
        isEnemy = true;
        base.Start();

        team = 1;
        turnManager.turnOrder.Add(gameObject);
        health = maxHealth;
        TurnManager.RoundStarted += GetNewPlan;
        GetNewPlan(null);

    }

    public void GetNewPlan(TurnManager turnManager)
    {
        oneToOnePos = mapManager.PosToOneToOne(transform.position);
        int distanceToPlayer = pathfinder.GetDistanceTo(playerControler.OneToOnePos, oneToOnePos);
        if (distanceToPlayer >= 20)
        {
            GainCondition(new DistanceSpeedBoost(distanceToPlayer-20));
        }
        if (distanceToPlayer >= 50)
        {
            GainCondition(new DistanceJump());
        }
        PlanDescription = displayedPlan;
        currentPlan.Clear();
        displayedPlan.Clear();
        if (currentmove == movesSetOrder.Count)
        {
            currentmove = 0;
        }
        if (movesSetOrder[currentmove] == -1)
        {
            plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        }
        else
        {
            //Debug.Log(movesSetOrder[currentmove]);
            plannedMoveSet = moveSets[movesSetOrder[currentmove]];
        }
        currentPlan = new List<System.Action>(plannedMoveSet);
        //Debug.Log("gotInitialPlan");
        levelManager.GetDifficultyModifier(this);
        //UpdatePlan();
    }

    public void UpdatePlan()
    {
        //Debug.Log("first condition: " + conditions[0].Name);
        PlanDescription = displayedPlan;
        displayedPlan.Clear();
        preferedRange = int.MaxValue;
        isPlanning = true;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            currentPlan[i]();
        }
        enemyStatsDisplayer.Plan(displayedPlan);
        isPlanning = false;
        //Debug.Log("first condition: " + conditions[0].Name);

    }
    public void StartOfTurn()
    {
        base.baseStartTurn();
        if (preferedRange == int.MaxValue)
        {
            preferedRange = 1;
        }
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.white;
        CalculateValues();
        nextAction = true;
    }

    public void EndTurn()
    {
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.black;
        base.baseEndTurn();
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
        StartOfTurn();
        yield return new WaitUntil(() => nextAction == true);
        nextAction = false;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            currentPlan[i]();
            yield return new WaitUntil(() => nextAction == true);
            nextAction = false;
        }
        currentmove++;
        EndTurn();
    }
    public IEnumerator DisplayMovePosibilities()
    {
        isPreparingMove = true;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            currentPlan[i]();
        }
        isPreparingMove = false;
        yield return new WaitUntil(() => mouseManager.selectedObject != gameObject);
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
