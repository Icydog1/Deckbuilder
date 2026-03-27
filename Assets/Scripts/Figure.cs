using NUnit.Framework;
using System.Collections.Generic;
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


    protected bool isMyTurn;
    protected bool isEnemy, isPlayer;
    protected bool isPlanning;
    public bool IsPlanning { set { isPlanning = value; } }

    protected Vector2 oneToOnePos;
    protected int preferedRange;
    protected float distanceToPlayer;


    protected int maxHealth = 1, health, block = 0;
    protected bool canFly = false;

    protected List<string> planDescription = new List<string>();
    public List<string> PlanDescription { set { planDescription = value; } }
    protected List<System.Action> prepareActions = new List<System.Action>();
    public List<System.Action> PrepareActions { set { prepareActions = value; } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();

        //statsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();


        health = maxHealth;


        statsDisplayer.SetHealthAndBlock(health, block);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void baseStartTurn()
    {
        preferedRange = 1;
        block = 0;
        statsDisplayer.SetHealthAndBlock(health, block);
    }

    public virtual void ActionDone()
    {
        Debug.Log("Base ActionDone ran");
    }



    public void Block(int blockValue)
    {
        float additionalBlock = blockValue;

        int finalBlock = Mathf.FloorToInt(additionalBlock);

        if (isPlanning)
        {
            prepareActions.Add(() => Block(finalBlock));
            string currentDescriptionString = "Block" + finalBlock;
            planDescription.Add(currentDescriptionString);
        }
        else
        {
            block += finalBlock;
            statsDisplayer.SetHealthAndBlock(health, block);
            ActionDone();
        }
    }

    public void Attack(int attackValue, int attackRange = 1, int targets = 1)
    {
        float additionalAttack = attackValue;


        int finalAttack = Mathf.FloorToInt(additionalAttack);

        if (isPlanning)
        {
            if (!isPlayer)
            {
                if (preferedRange > attackRange)
                {
                    preferedRange = attackRange;
                }
            }
            prepareActions.Add(() => Attack(finalAttack, attackRange, targets));
            string planString = "Attack " + finalAttack;
            if (attackRange > 1)
            {
                planString += " range " + attackRange;
            }
            if (targets > 1)
            {
                planString += " target " + targets;
            }
            planDescription.Add(planString);
        }
        else
        {
            if (isPlayer)
            {
                playerControler.ControledAttack(finalAttack, attackRange, targets);
            }
            else
            {
                if (distanceToPlayer <= attackRange)
                {
                    playerControler.AttackedFor(finalAttack);
                }
                ActionDone();
            }

        }
    }
    public void Move(int moveValue, bool isJump = false)
    {
        float additionalMove = moveValue;


        int finalMove = Mathf.FloorToInt(additionalMove);
        if (isPlanning)
        {
            prepareActions.Add(() => Move(finalMove, isJump));
            string planString = "Move " + finalMove;
            if (isJump)
            {
                planString += " Jump";
            }
            planDescription.Add(planString);
        }
        else
        {
            if (isPlayer)
            {
                playerControler.ControledMove(finalMove, isJump);
            }
            else
            {
                StartCoroutine(pathfinder.PathfindTowards(oneToOnePos, playerControler.playerOneToOneCords, gameObject, finalMove, preferedRange, isJump, canFly));
            }
        }
    }
    public void AttackedFor(int attackValue)
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
    }
    public virtual void Die()
    {
        Debug.Log("Base Die ran");
    }

}
