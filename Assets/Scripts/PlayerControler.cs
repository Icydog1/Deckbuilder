using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool cardPlayed;
    public bool actionDone, manualEnd;
    private bool isMoving, isAttacking;
    public GameObject player;
    public MouseManager mouseManager;
    private MapManager mapManager;
    public GameObject clickedTile;
    public GameObject playedCard;
    public Card playedCardScript;
    private Vector3 playerHexCords;
    private int moveLeft, targetsLeft;
    private int range;
    private bool isTargetTile, isTargetEnemy;

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

    }

    public void TileClicked(GameObject tile)
    {
        if (isTargetTile)
        {
            
            playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedTile.transform.position);

            if (mapManager.GetDistanceTo(clickedObjectHex, playerHexCords) <= moveLeft)
            {
                player.transform.position = mapManager.HexToPos(clickedObjectHex);
                moveLeft -= Mathf.RoundToInt(mapManager.GetDistanceTo(clickedObjectHex, playerHexCords));
            }
            if (moveLeft == 0 || manualEnd)
            {
                ActionDone();

            }
        }
        else if (isTargetEnemy)
        {
            //find enemy at clicked tile
        }
    }
    public void EnemyClicked(GameObject enemy)
    {
        if (isAttacking)
        {
            Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedTile.transform.position);
            if (mapManager.GetDistanceTo(clickedObjectHex, playerHexCords) <= range)
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
        isTargetTile = true;
        moveLeft = moveValue;

    }

    public void AttackX(int attackValue, int attackRange = 1, int targets = 1)
    {
        actionDone = false;
        isMoving = true;
        targetsLeft = targets;
        range = attackRange;

    }
}
