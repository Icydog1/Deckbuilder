using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public bool cardPlayed;
    public bool actionDone;
    private bool isMoving, isAttacking;
    public GameObject player;
    public MouseManager mouseManager;
    private MapManager mapManager;
    public GameObject clickedTile;
    public GameObject playedCard;
    public Card playedCardScript;
    private Vector3 playerHexCords;
    private int moveLeft;

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
        if (isMoving)
        {
            playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedTile.transform.position);
            
            if (mapManager.GetDistanceTo(clickedObjectHex, playerHexCords) <= moveLeft)
            {
                player.transform.position = mapManager.HexToPos(clickedObjectHex);
                moveLeft -= Mathf.RoundToInt(mapManager.GetDistanceTo(clickedObjectHex, playerHexCords));
            }
            if (moveLeft == 0)
            {
                isMoving = false;
                actionDone = true;
                playedCardScript.currentStep++;
            }
        }
        if (isAttacking)
        {
            playerHexCords = mapManager.GetPosInHexCords(player.transform.position);
            Vector3 clickedObjectHex = mapManager.GetPosInHexCords(clickedTile.transform.position);
            
            if (mapManager.GetDistanceTo(clickedObjectHex, playerHexCords) <= moveLeft)
            {
                player.transform.position = mapManager.HexToPos(clickedObjectHex);
                moveLeft -= Mathf.RoundToInt(mapManager.GetDistanceTo(clickedObjectHex, playerHexCords));
            }
            if (moveLeft == 0)
            {
                isMoving = false;
                actionDone = true;
            }
        }
    }

    public void MoveX(int moveValue, bool isJump = false)
    {
        actionDone = false;
        isMoving = true;
        moveLeft = moveValue;

    }

    public void AttackX(int attackValue, int range = 1, int targets = 1)
    {
        actionDone = false;
        isMoving = true;
        moveLeft = attackValue;

    }
}
