using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject player;
    public float tileSize = 1, tileWidth, tileHeight;
    private float tileXDistance, tileYDistance;
    float upLeft, up, upRight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player");
        tileWidth = tileSize * 2;
        tileHeight = tileSize * Mathf.Sqrt(3);
        tileXDistance = tileWidth * 3/4;
        tileYDistance = tileHeight;
        //Debug.Log(GetDisanceInHexCordsTo(Vector2.one * 5,Vector2.zero));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void Move(GameObject movedObject,)
    public Vector3 GetPosInHexCords(Vector2 pos)
    {
        return (GetDisanceInHexCordsTo(pos,Vector2.zero));
    }

    public Vector3 GetDisanceInHexCordsTo(Vector2 pos, Vector2 targetPos)
    {
        //hex coridates (tiles left and up, tiles up, tiles right and up)
        // up = 1 up, leftup = -1 right + 0.5 up, rightup = 1 right + 0.5 up
        //retunes a vector 3 which is the cordinaets in hex cordinates
        float xComponent = pos.x - targetPos.x;
        float yComponent = pos.y - targetPos.y;
        //Debug.Log(-xComponent + "," + -yComponent + " start pos");
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
                Debug.Log(xComponent + " original x, " + yComponent + " original y");
                tilesUp = 0;
                tilesRight = 0;
            }
            //HexToPos(new Vector3(upLeft, up, upRight));
            //Debug.Log(-tilesUp + "," + -tilesRight + " cords, itiration" + killSwitch);

        }
        return new Vector3(upLeft, up, upRight);
    }

    public Vector2 PosWithHexOffset(Vector2 startPos, Vector3 Hexoffset)
    {
        Vector2 regularOffset = HexToPos(Hexoffset);
        //Debug.Log(Hexoffset + "Hexoffset, " + regularOffset + "regularOffset");
        return startPos + regularOffset;
    }
    public Vector2 HexToPos(Vector3 hexPos)
    {
        float xComponent;
        xComponent = hexPos.z * tileXDistance - hexPos.x * tileXDistance;
        float yComponent;
        yComponent = hexPos.y * tileYDistance + hexPos.z * tileYDistance / 2 + hexPos.x * tileYDistance / 2;
        //Debug.Log(new Vector2(xComponent, yComponent) + "back to regular");
        return new Vector2(xComponent, yComponent);
    }

    public int GetDistanceTo(Vector2 startPos, Vector2 endPos)
    {
        Vector3 cordsBetween = GetDisanceInHexCordsTo(startPos, endPos);
        //Debug.Log(cordsBetween + "cordsBetween");
        return Mathf.RoundToInt(Mathf.Abs(cordsBetween.x) + Mathf.Abs(cordsBetween.y) + Mathf.Abs(cordsBetween.z));

    }
    public int GetDistanceToHex(Vector3 startHexPos, Vector3 endHexPos)
    {
        Vector3 cordsBetween = GetDisanceInHexCordsTo(HexToPos(startHexPos), HexToPos(endHexPos));
        //Debug.Log(cordsBetween + "cordsBetween");
        return Mathf.RoundToInt(Mathf.Abs(cordsBetween.x) + Mathf.Abs(cordsBetween.y) + Mathf.Abs(cordsBetween.z));

    }

    public GameObject GetObjectOnHex(Vector2 pos, int layer)
    {
        return Physics2D.OverlapPoint(pos, layer, -Mathf.Infinity, Mathf.Infinity).gameObject;
    }
}
