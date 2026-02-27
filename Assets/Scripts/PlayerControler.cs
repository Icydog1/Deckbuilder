using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool cardPlayed;
    public bool actionDone, manualEnd;
    private bool isMoving, isAttacking;
    private GameObject player;
    private MouseManager mouseManager;
    private MapManager mapManager;
    public GameObject clickedTile, clickedEnemy;
    public GameObject playedCard;
    public Card playedCardScript;
    private Vector3 playerHexCords;
    private int moveLeft, targetsLeft, attackDamageValue;
    private int range;
    private bool isTargetATile, isTargetAEnemy;

    public int topEnergy, bottomEnergy;
    public int maxHealth = 100, health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        player = GameObject.Find("Player");

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (manualEnd)
        {
            ActionDone();
        }
    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetATile)
        {
            clickedTile = tile;
            playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            Vector2 clickedTileCords = clickedTile.transform.position;

            if (mapManager.GetDistanceTo(clickedTileCords, player.transform.position) <= moveLeft)
            {
                moveLeft -= mapManager.GetDistanceTo(clickedTileCords, player.transform.position);
                player.transform.position = clickedTileCords;
                //Debug.Log(moveLeft);
            }
            if (moveLeft == 0)
            {
                ActionDone();
                Debug.Log("Done Moving");
            }
        }
    }
    public void EnemyClicked(GameObject enemy)
    {
        clickedEnemy = enemy;
        if (isTargetAEnemy)
        {
            if (isAttacking)
            {
                Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedEnemy.transform.position);
                Vector2 clickedEnemyCords = clickedEnemy.transform.position;
                //sometimes doent work
                if (mapManager.GetDistanceTo(clickedEnemyCords, playerHexCords) <= range)
                {
                    clickedEnemy.GetComponent<Enemy>().AttackedForX(attackDamageValue);
                    targetsLeft--;
                }
                if (targetsLeft == 0)
                {
                    ActionDone();
                    Debug.Log("Done Attacking");
                }
            }
        }

    }

    public void ActionDone()
    {
        isMoving = false;
        isAttacking = false;
        manualEnd = false;
        actionDone = true;
        isTargetATile = false;
        isTargetAEnemy = false;
        playedCardScript.currentStep++;
    }

    public void MoveX(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        isTargetATile = true;
        moveLeft = moveValue;
    }

    public void AttackX(int attackValue, int attackRange = 1, int targets = 1)
    {
        actionDone = false;
        isAttacking = true;
        targetsLeft = targets;
        attackDamageValue = attackValue;
        range = attackRange;
        isTargetAEnemy = true;

    }

    public void AttackedForX(int attackValue)
    {
        health -= attackValue;
    }

}
