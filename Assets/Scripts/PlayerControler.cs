using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool cardPlayed;
    public bool actionDone, manualEnd;
    private bool isMoving, isAttacking;
    private GameObject player;
    private MouseManager mouseManager;
    private MapManager mapManager;
    public GameObject clickedTile;
    public GameObject playedCard;
    public Card playedCardScript;
    private Vector3 playerHexCords;
    private int moveLeft, targetsLeft;
    private int range;
    private bool isTargetATile, isTargetAEnemy;

    public int topEnergy, bottomEnergy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        player = GameObject.Find("Player");

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
            }
        }
        else if (isTargetAEnemy)
        {
            //find enemy at clicked tile
        }
    }
    public void EnemyClicked(GameObject enemy)
    {
        if (isAttacking)
        {
            Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedTile.transform.position);
            Vector2 clickedTileCords = clickedTile.transform.position;
            if (mapManager.GetDistanceTo(clickedTileCords, playerHexCords) <= range)
            {
                //add detection if enemy is in hex
                //add damage (also maby conditions)
                //reduce number of targets
            }
            if (targetsLeft == 0)
            {
                ActionDone();
            }
        }

    }

    public void ActionDone()
    {
        isMoving = false;
        isAttacking = false;
        manualEnd = false;
        actionDone = true;
        playedCardScript.currentStep++;
    }

    public void MoveX(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        isTargetATile = true;
        moveLeft = moveValue;
        isTargetATile = true;
    }

    public void AttackX(int attackValue, int attackRange = 1, int targets = 1)
    {
        actionDone = false;
        isMoving = true;
        targetsLeft = targets;
        range = attackRange;
        isTargetAEnemy = true;

    }

    public void AttackedForX(int attackValue)
    {

    }

}
