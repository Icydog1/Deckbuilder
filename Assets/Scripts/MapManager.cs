using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//calculates distances and converting between rect and hex cordinate systems and detecting what is at a location
//Rect is X-Y cordinates, cartesian
//Hex uses diffrent basis vectors, (1,0) is one tile upLeft and (0,1) is one tile upRight
public class MapManager : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject player;
    private float tileSize = 1, tileWidth, tileHeight;
    public float TileWidth { get { return tileWidth; } }
    public float TileHeight { get { return tileHeight; } }
    private int baseMoveCost = 5;
    public int BaseMoveCost { get { return baseMoveCost; } }

    private float tileXDistance, tileYDistance;

    void Awake()
    {
        gameManager = RefrenceStorage.gameManager;
        player = RefrenceStorage.player;
        tileWidth = tileSize * 2;
        tileHeight = tileSize * Mathf.Sqrt(3);
        tileXDistance = tileWidth * 3 / 4;
        tileYDistance = tileHeight;
    }
    public void showMoveCost(bool showOrHide, bool isJump = false, bool isfly = false)
    {
        Tile[] tiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        if (showOrHide)
        {
            if (isJump || isfly)
            {
                foreach (Tile tileScript in tiles)
                {
                    if (!(tileScript.gameObject.GetComponent<Wall>() && !(tileScript.gameObject.GetComponent<Stair>() || tileScript.gameObject.GetComponent<Door>())))
                    {
                        tileScript.MoveCostDisplay.DisplayVariable(Mathf.Min(tileScript.MoveCost, baseMoveCost));
                    }
                }
            }
            else
            {
                foreach (Tile tileScript in tiles)
                {
                    if (!(tileScript.gameObject.GetComponent<Wall>() && !(tileScript.gameObject.GetComponent<Stair>() || tileScript.gameObject.GetComponent<Door>())) && !tileScript.gameObject.GetComponent<Obstacle>())
                    {
                        tileScript.MoveCostDisplay.DisplayVariable(tileScript.MoveCost);
                    }
                    else
                    {
                        tileScript.MoveCostDisplay.Disable();
                    }
                }
            }
        }
        else
        {
            foreach (Tile tileScript in tiles)
            {
                tileScript.MoveCostDisplay.Disable();
            }
        }
    }
    public void showMoveCostOfTile(Tile tileScript)
    {
        PlayerControler player = RefrenceStorage.playerControler;
        if (player.MoveCostDisplaySetting == "Always" || (player.MoveCostDisplaySetting == "On Move" && player.PlanningMove))
        {
            if (player.CanFly || player.CanJump)
            {
                if (!(tileScript.gameObject.GetComponent<Wall>() && !(tileScript.gameObject.GetComponent<Stair>() || tileScript.gameObject.GetComponent<Door>())))
                {
                    tileScript.MoveCostDisplay.DisplayVariable(Mathf.Min(tileScript.MoveCost, baseMoveCost));
                }
            }
            else
            {
                if (!(tileScript.gameObject.GetComponent<Wall>() && !(tileScript.gameObject.GetComponent<Stair>() || tileScript.gameObject.GetComponent<Door>())) && !tileScript.gameObject.GetComponent<Obstacle>())
                {
                    tileScript.MoveCostDisplay.DisplayVariable(tileScript.MoveCost);
                }
            }
        }
        else
        {
            tileScript.MoveCostDisplay.Disable();

        }
    }
    //conver hex to pos
    public Vector2 HexToRect(Vector2 hexPos)
    {
        float xComponent;
        xComponent = hexPos.y * tileXDistance - hexPos.x * tileXDistance;
        float yComponent;
        yComponent = hexPos.y * tileYDistance / 2 + hexPos.x * tileYDistance / 2;
        return new Vector2(xComponent, yComponent);

    }

    public Vector2Int RectToHex(Vector2 rectPos)
    {
        float tilesRight = rectPos.x / tileXDistance;
        float tilesUp = rectPos.y / tileYDistance;
        float hexX = - tilesRight / 2 + tilesUp;
        float hexY = tilesRight / 2 + tilesUp;
        return new Vector2Int(Mathf.RoundToInt(hexX), Mathf.RoundToInt(hexY));
    }

    public int GetDistanceBetweenHex(Vector2 startHex, Vector2 endHex)
    {
        Vector2 vectorBetween = startHex - endHex;
        float distance = -1;
        if (Mathf.Sign(vectorBetween.x) == Mathf.Sign(vectorBetween.y))
        {
            if (Mathf.Abs(vectorBetween.x) > Mathf.Abs(vectorBetween.y))
            {
                distance = Mathf.Abs(vectorBetween.x);
            }
            else
            {
                distance = Mathf.Abs(vectorBetween.y);
            }
        }
        else
        {
            distance = Mathf.Abs(vectorBetween.x) + Mathf.Abs(vectorBetween.y);
        }
        return Mathf.RoundToInt(distance);
    }

    //public int GetDistanceBetweenPos(Vector2 startPos, Vector2 endPos)
    //{
    //    return GetDistanceBetweenHex(RectToHex(startPos), RectToHex(endPos));

    //}
   
    //public List<GameObject> GetObsticalAtHex(Vector2 HexPos, bool obstacle = true, bool enemy = true, bool player = true, bool wall = true)
    //{
    //    List<GameObject> obstacles = new List<GameObject>();
    //    Vector2 pos = HexToRect(HexPos);
    //    if (wall && Physics2D.OverlapPoint(pos, 64) != null && Physics2D.OverlapPoint(pos, 64).gameObject.GetComponent<Wall>() != null)
    //    {
    //        obstacles.Add(Physics2D.OverlapPoint(pos, 64).gameObject);
    //    }
    //    if (obstacle && Physics2D.OverlapPoint(pos, 64) != null && Physics2D.OverlapPoint(pos, 64).gameObject.GetComponent<Obstacle>() != null)
    //    {
    //        obstacles.Add(Physics2D.OverlapPoint(pos, 64).gameObject);
    //    }
    //    if (enemy && Physics2D.OverlapPoint(pos, 128) != null)
    //    {
    //        obstacles.Add(Physics2D.OverlapPoint(pos, 128).gameObject);
    //    }
    //    if (player && Physics2D.OverlapPoint(pos, 256) != null)
    //    {
    //        obstacles.Add(Physics2D.OverlapPoint(pos, 256).gameObject);
    //    }
    //    return obstacles;
    //}

    public GameObject GetTileAtHex(Vector2 HexPos)
    {
        Vector2 pos = HexToRect(HexPos);
        if (Physics2D.OverlapPoint(pos, 64) != null)
        {
            return Physics2D.OverlapPoint(pos, 64).gameObject;
        }
        else
        {
            //is checking beond doors currently dont think it is a problem though

            //Debug.Log("no Tile There");
            //Debug.Log("checked " + HexPos + " one to one");
            //Debug.Log("checked " + HexToRect(HexPos) + " hexPos");

            return null;
        }
    }
    public GameObject GetEntityOnHex(Vector2 HexPos)
    {
        Vector2 pos = HexToRect(HexPos);
        int layermask = 384;
        if (Physics2D.OverlapPoint(pos, layermask) != null)
        {
            return Physics2D.OverlapPoint(pos, layermask).gameObject;
        }
        else
        {
            return null;
        }
    }
    

    //old hexCord stuff
    /*
    public Vector3 GetPosInHexCords(Vector2 hexPos)
    {
        return (GetDisanceInHexCordsTo(hexPos,Vector2.zero));
    }

    public Vector3 GetDisanceInHexCordsTo(Vector2 hexPos, Vector2 targetPos)
    {
        //hex coridates (tiles left and up, tiles up, tiles right and up)
        // up = 1 up, leftup = -1 right + 0.5 up, rightup = 1 right + 0.5 up
        //retunes a vector 3 which is the cordinaets in hex cordinates
        float xComponent = hexPos.x - targetPos.x;
        float yComponent = hexPos.y - targetPos.y;
        //Debug.Log(-xComponent + "," + -yComponent + " start hexPos");
        float tilesUp = yComponent / tileYDistance;
        float tilesRight = xComponent / tileXDistance;
        tilesUp = Mathf.Round(tilesUp * 2) / 2;
        tilesRight = Mathf.Round(tilesRight);
        upLeft = 0;
        up = 0;
        upRight = 0;
        float killSwitch = 100;
        //Debug.Log(-tilesUp + "," + -tilesRight + " start tiles");
        while (tilesUp != 0 || tilesRight != 0)
        {
            //if in 1-5 oclock range or 7-11 oclock range
            if (Mathf.Abs(tilesRight) * 3 / 2 > Mathf.Abs(tilesUp))
            {
                //if in 1-3 oclock range or 7-9 oclock range
                if (tilesUp > 0 && tilesRight > 0 || tilesUp < 0 && tilesRight < 0)
                {
                    //if in 7-9 oclock range
                    if (tilesRight < 0)
                    {
                        upRight--;
                        tilesRight++;
                        tilesUp = tilesUp + 0.5f;
                    }
                    //if in 1-3 oclock range
                    else
                    {
                        upRight++;
                        tilesRight--;
                        tilesUp = tilesUp - 0.5f;
                    }

                }
                //if in 3-5 oclock range or 9-11 oclock range
                else
                {
                    //if in 9-11 oclock range
                    if (tilesRight < 0)
                    {
                        upLeft++;
                        tilesRight++;
                        tilesUp = tilesUp - 0.5f;
                    }
                    //if in 3-5 oclock range
                    else
                    {
                        upLeft--;
                        tilesRight--;
                        tilesUp = tilesUp + 0.5f;
                    }
                }
            }
            //if in 11-1 oclock range or 5-7 oclock range
            else
            {
                //if in 11-1 oclock range
                if (tilesUp > 0)
                {
                    up++;
                    tilesUp--;
                }
                //if in 5-7 oclock range
                else
                {
                    up--;
                    tilesUp++;
                }
            }
            killSwitch--;
            if (killSwitch < 0)
            {
                Debug.Log(tilesUp + " up, " + tilesRight + " right, timed out");
                Debug.Log(hexPos + " original hexPos, " + targetPos + " target hexPos");

                tilesUp = 0;
                tilesRight = 0;
            }
            //HexToRect(new Vector3(upLeft, up, upRight));
            //Debug.Log(-tilesUp + "," + -tilesRight + " cords, itiration" + killSwitch);

        }
        return new Vector3(upLeft, up, upRight);
    }

    public Vector2 PosWithHexOffset(Vector2 startPos, Vector3 Hexoffset)
    {
        Vector2 regularOffset = HexToRect(Hexoffset);
        //Debug.Log(Hexoffset + "Hexoffset, " + regularOffset + "regularOffset");
        return startPos + regularOffset;
    }
    public Vector2 HexToRect(Vector3 hexPos)
    {
        float xComponent;
        xComponent = hexPos.z * tileXDistance - hexPos.x * tileXDistance;
        float yComponent;
        yComponent = hexPos.y * tileYDistance + hexPos.z * tileYDistance / 2 + hexPos.x * tileYDistance / 2;
        //Debug.Log(new Vector2(xComponent, yComponent) + "back to regular");
        return new Vector2(xComponent, yComponent);
    }

    public Vector2 HexToHex(Vector3 HexPos)
    {
        //Hex coridates (tiles left and up, tiles right and up)
        return new Vector2(HexPos.x + HexPos.y, HexPos.z + HexPos.y);
    }
    public Vector3 HexToHex(Vector2 hexPos)
    {
        //Hex coridates (tiles left and up, tiles right and up)
        return new Vector3(hexPos.x, 0, hexPos.y);
    }

    public int GetDistanceToHex(Vector3 startHexPos, Vector3 endHexPos)
    {
        Vector3 cordsBetween = GetDisanceInHexCordsTo(HexToRect(startHexPos), HexToRect(endHexPos));
        //Debug.Log(cordsBetween + "cordsBetween");
        return Mathf.RoundToInt(Mathf.Abs(cordsBetween.x) + Mathf.Abs(cordsBetween.y) + Mathf.Abs(cordsBetween.z));

    }
    */
}
