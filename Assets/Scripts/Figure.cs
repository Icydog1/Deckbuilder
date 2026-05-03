using System.Collections.Generic;
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

    protected int team;
    public int Team { get { return team; } }
    protected bool isPreformingAnimation;
    public bool IsPreformingAnimation { set { isPreformingAnimation = value; } get { return isPreformingAnimation; } }

    protected int maxHealth = 1, health, block = 0;
    public int MaxHealth { get { return maxHealth; } }

    protected bool canFly = false;

    protected List<string> planDescription = new List<string>();
    public List<string> PlanDescription { set { planDescription = value; } }
    //protected List<System.Action> prepareActions = new List<System.Action>();
    //public List<System.Action> PrepareActions { set { prepareActions = value; } }

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
        conditionEffects.StartOfTurnConditons(this);
        for (int i = 0; i < conditions.Count; i++)
        {
            if (conditions[i].IsStartOfTurn && conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
                //Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);
            }
            if (conditions[i].IsStartOfTurn && conditions[i].Duration == 0)
            {
                //Debug.Log("removed " + conditions[i].Name);

                conditions.RemoveAt(i);
                i--;
            }
        }
        statsDisplayer.DisplayConditions(conditions);
        block = 0;
        statsDisplayer.SetHealthAndBlock(health, block);

    }
    public void baseEndTurn()
    {
        for (int i = 0; i < conditions.Count; i++)
        {
            if (!conditions[i].IsStartOfTurn && conditions[i].Duration > 0)
            {
                conditions[i].Duration--;
                Debug.Log("counted down " + conditions[i].Name + " to " + conditions[i].Duration);

            }
            if (!conditions[i].IsStartOfTurn && conditions[i].Duration == 0)
            {
                Debug.Log("removed " + conditions[i].Name);
                conditions.RemoveAt(i);
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
        int finalBlock = conditionEffects.ModifyBlock(this, blockValue);
        if (isPlanning)
        {
            //prepareActions.Add(() => Block(finalBlock));
            //string currentDescriptionString = "Block " + finalBlock;
            string currentDescriptionString = "<sprite name=Block> " + finalBlock;
            planDescription.Add(currentDescriptionString);
        }
        else if (!isPreparingMove)
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
        int finalAttack = conditionEffects.ModifyAttack(this, attackValue);
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
            //currentDescriptionStart = "Attack " + finalAttack;
            currentDescriptionStart = "<sprite name=Attack> " + finalAttack;

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
        else if(!isPreparingMove)
        {
            if (isPlayer)
            {
                playerControler.ControledAttack(finalAttack, attackRange, targets, repeats, attackConditions);
            }
            else
            {
                foreach (Figure target in FindTargets("enemy", attackRange, targets))
                {
                    target.AttackedFor(finalAttack, repeats, attackConditions);
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
        int finalMove = conditionEffects.ModifyMove(this, moveValue);
        bool finalJump = conditionEffects.ModifyJump(this, isJump);
        finalMove = Mathf.Clamp(finalMove, 0, 9999999);
        //Mathf(finalMove,0,)
        if (isPlanning)
        {
            //string planString = "Move " + finalMove;
            string planString = "<sprite name=Move> " + finalMove;

            if (finalJump)
            {
                planString += " Jump";
            }
            planDescription.Add(planString);
        }
        else if (isPreparingMove)
        {
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

            foreach (Condition condition in newConditions)
            {
                if (condition.Abnormality != null)
                {
                    actionAbnormalities.Add(condition.Abnormality);
                }
                string currentDescriptionString = "";
                //Debug.Log(actionAbnormalities);
                if (condition.Plan != null)
                {
                    foreach (System.Action action in condition.Plan)
                    {
                        action();
                        if (condition.Plan[0] != action)
                        {
                            planDescription[planDescription.Count - 2] = planDescription[planDescription.Count - 2] + " and " + planDescription[planDescription.Count - 1];
                            planDescription.RemoveAt(planDescription.Count - 1);
                        }
                    }
                    //condition.Plan();
                }
                if (actionAbnormalities.Contains("Delayed Gain"))
                {
                    if (condition.Duration == 1)
                    {
                        //planDescription.Add("Next turn");
                        
                        planDescription[planDescription.Count - 1] = "Next turn " + planDescription[planDescription.Count - 1];
                    }
                    else if (condition.Duration != -1)
                    {
                        planDescription[planDescription.Count - 1] = "At the start of the next " + condition.Duration + " turns" + planDescription[planDescription.Count - 1];
                    }
                    individualConditionText.Add(currentDescriptionString);
                }

                if (actionAbnormalities.Contains("Delayed Gain"))
                {

                }
                else
                {
                    currentDescriptionString = condition.Value + " " + condition.Name;
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

                if (targetType == "self")
                {
                    if (!actionAbnormalities.Contains("Delayed Gain"))
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
            }
            string separator = ", ";
            string conditionText = currentDescriptionStart + string.Join(separator, individualConditionText) + currentDescriptionEnd;
            planDescription.Add(conditionText);

        }
        else if (!isPreparingMove)
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
                    conditions.RemoveAt(i);
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
    public void AttackedFor(int attackValue, int repeats, Condition[] newConditions)
    {
        for (int i = 0; i < repeats; i++)
        {
            TakeDamage(attackValue);
            if (!isDead)
            {
                GainConditions(newConditions);
            }
        }


    }
    public void TakeDamage(int damageValue)
    {
        if (block > 0)
        {
            int damageBlocked = Mathf.Min(damageValue, block);
            damageValue -= damageBlocked;
            block -= damageBlocked;
        }
        LoseHealth(damageValue);
    }
    public void LoseHealth(int amount)
    {
        health -= amount;
        statsDisplayer.SetHealthAndBlock(health, block);
        if (health <= 0)
        {
            isDead = true;
            Die();
        }
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
}
