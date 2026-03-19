using UnityEngine;

public class Figure : MonoBehaviour
{
    protected TurnManager turnManager;
    protected MapManager mapManager;
    protected MouseManager mouseManager;
    protected FigureStats statsDisplayer;


    protected bool isMyTurn;

    protected Vector2 oneToOnePos;

    protected int maxHealth, health, block;
    protected bool canFly = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        //statsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();


        health = maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void ActionDone()
    {
        Debug.Log("Base ActionDone ran");

    }



    public void Block(int blockValue)
    {
        block += blockValue;
        statsDisplayer.SetHealthAndBlock(health, block);
        ActionDone();
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
