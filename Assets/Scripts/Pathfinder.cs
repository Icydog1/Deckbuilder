using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.UI.Image;

public class Pathfinder : MonoBehaviour
{
    private MapManager mapManager;

    private List<List<Vector2>> elevations = new List<List<Vector2>>();
    private List<Vector2> currentHeight = new List<Vector2>();
    private List<Vector2> checkedTiles = new List<Vector2>();
    private List<Vector2> safeTiles = new List<Vector2>();
    private List<Vector2> unSafeTiles = new List<Vector2>();
    private List<Vector2> impassableTiles = new List<Vector2>();

    private List<List<Vector2>> originalElevations = new List<List<Vector2>>();
    private List<Vector2> originalSafeTiles = new List<Vector2>();
    private List<Vector2> posibleTiles = new List<Vector2>();
    private List<Vector2> posibleTilesPath = new List<Vector2>();

    private List<Vector2> actualPath = new List<Vector2>();

    private Vector2 furthestPoint;
    private int furthestElevation;

    private bool pathFound, inRange;
    private bool isJump, isFly;
    private int moveValue;
    private int currentElevation;
    private Vector2 currentPos;
    private Enemy currentEnemy;

    private float enemyMoveDelay = 0.1f;
    private bool doneMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

        //findPathFrom(Vector3.zero, Vector3.back * -2 + Vector3.right * -2);
        //need to fix problem that corrdinte system hase multipule vales for eash corrdinate (to vecot 3s will point to same spot)
        //dpmt think it will casue problems just performese issuse(fatest rount wont be going in circles)
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator PathfindTowards(Vector2 selfPos, Vector2 targetPos, GameObject self, int newMoveValue, int range = 1, bool jump = false, bool fly = false)
    {
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        //Debug.Log("attemteted to move");
        findPathFromToRange(selfPos, targetPos, range);
        //Debug.Log("elevation map");

        if (!inRange)
        {
            findPosiblePaths(selfPos);
            //Debug.Log("found Path");
            //Debug.Log(actualPath[0]);
            StartCoroutine(MoveAlongPath(self, selfPos));
            yield return new WaitUntil(() => doneMoving == true);
            doneMoving = false;
            //Debug.Log("Moved");
        }

        self.GetComponent<Enemy>().ActionDone();
    }


    public void findPathFromToRange(Vector2 selfPos, Vector2 targetPos, int range)
    {
        inRange = false;
        currentPos = selfPos;
        findPosInRange(targetPos, range);
        List<Vector2> posibleLocations = new List<Vector2>(safeTiles);
        findPathToArea(selfPos, posibleLocations);
    }


    public void findPathToArea(Vector2 selfPos, List<Vector2> targetArea)
    {
        //Debug.Log("pathfinding started initial cords" + targetPos + " to " + selfPos);
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        //Debug.Log("safeTiles " + targetArea[0]);
        if (targetArea.Contains(selfPos))
        {
            pathFound = true;
            inRange = true;
        }
        for (int i = 0; pathFound == false; i++)
        {
            currentElevation = i;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            if (i == 0)
            {
                //Debug.Log("started");
                //Debug.Log("safeTiles " + targetArea[0]);
                foreach (Vector2 pos in targetArea)
                {
                    elevations[0].Add(pos);
                    checkedTiles.Add(pos);
                    safeTiles.Add(pos);
                    //Debug.Log("start tile" + pos);
                    GameObject tile = mapManager.GetTileAtHex(pos);
                    tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, currentElevation);

                }
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation - 1].Count; j++)
                {
                    buildElevation(elevations[currentElevation - 1][j], false, false);
                }
            }

            if (i >= 1000)
            {
                pathFound = true;
                Debug.Log("area pathfinding timed out");
            }
        }
    }


    public void findPosInRange(Vector2 targetPos, int range)
    {
        //Debug.Log("pathfinding started initial cords" + targetPos + " to " + selfPos);
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        for (int i = 0; i <= range; i++)
        {
            currentElevation = i;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            if (i == 0)
            {
                elevations[i].Add(targetPos);
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation - 1].Count; j++)
                {
                    buildElevation(elevations[currentElevation - 1][j], true, false);
                }
            }
            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("range pathfinding timed out");
            }
        }
    }

    public void findPosiblePaths(Vector2 selfPos)
    {
        originalElevations.Clear();
        originalSafeTiles.Clear();
        originalElevations = new List<List<Vector2>>(elevations);
        originalSafeTiles = new List<Vector2>(safeTiles);
        furthestElevation = currentElevation;
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        posibleTiles.Clear();
        posibleTilesPath.Clear();
        pathFound = false;
        for (int i = 0; i <= moveValue; i++)
        {
            currentElevation = i;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            if (i == 0)
            {

                elevations[i].Add(selfPos);
                checkedTiles.Add(selfPos);
                safeTiles.Add(selfPos);
                posibleTiles.Add(selfPos);
                posibleTilesPath.Add(selfPos);
                furthestPoint = selfPos;
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation - 1].Count; j++)
                {
                    buildElevation(elevations[currentElevation - 1][j], false, true);
                }
            }

            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("posible path pathfinding timed out");
            }
        }
        int killswitch = 0;
        GameObject tile = mapManager.GetTileAtHex(furthestPoint);
        GameObject border = tile.transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.red;
        Vector2 currentLocaton = furthestPoint;
        actualPath.Clear();
        //Debug.Log(furthestPoint);
        while (currentLocaton != selfPos)
        {
            actualPath.Insert(0, currentLocaton);
            currentLocaton = posibleTilesPath[posibleTiles.IndexOf(currentLocaton)];
            killswitch++;
            if (killswitch > 100)
            {
                Debug.Log(currentLocaton);
                currentLocaton = selfPos;
                Debug.Log("Finding move path timed out");
            }
        }
    }

    public void buildElevation(Vector2 pos, bool range, bool pathFromEnemy)
    {
        Vector2 checktile = new Vector2();
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = pos + Vector2.up; break ;
                case 1: checktile = pos + Vector2.down; break;
                case 2: checktile = pos + Vector2.right; break;
                case 3: checktile = pos + Vector2.left; break;
                case 4: checktile = pos + Vector2.up + Vector2.right; ; break;
                case 5: checktile = pos + Vector2.down + Vector2.left; break;
            }
            if (!checkedTiles.Contains(checktile))
            {
                GameObject tile = mapManager.GetTileAtHex(checktile);
                GameObject entity = mapManager.GetEntityOnHex(checktile);

                if (checktile == currentPos && !pathFromEnemy)
                {
                    pathFound = true;
                    //Debug.Log("path found");
                    //Debug.Log("final elevation " + currentElevation);
                    //Debug.Log(mapManager.GetTileAtHex(checktile).transform.position);
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.cyan;
                    safeTiles.Add(checktile);
                    elevations[currentElevation].Add(checktile);

                }
                else if (tile.GetComponent<Wall>() || (tile.GetComponent<Obstacle>() && !(range || isJump || isFly)) || (entity && entity.GetComponent<PlayerControler>() && !(range || isJump || isFly)))
                {
                    impassableTiles.Add(checktile);
                    //Debug.Log("obstical found at " + checktile);
                }
                else if ((entity && entity.GetComponent<Enemy>()) || (tile.GetComponent<Obstacle>() && ((range || isJump) && !isFly)) || (entity && entity.GetComponent<PlayerControler>() && (range || isJump || isFly)))
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.yellow;
                    unSafeTiles.Add(checktile);
                    elevations[currentElevation].Add(checktile);
                    tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, currentElevation);
                    //Debug.Log("unsafe tile found at " + checktile);
                }
                else
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.blue;
                    safeTiles.Add(checktile);
                    elevations[currentElevation].Add(checktile);
                    tile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, currentElevation);
                }
                checkedTiles.Add(checktile);
                if (pathFromEnemy)
                {
                    if (originalSafeTiles.Contains(checktile))
                    {
                        for (int j = 0; j < furthestElevation; j++)
                        {
                            if (originalElevations[j].Contains(checktile))
                            {
                                //Debug.Log("new tile");
                                furthestPoint = checktile;
                                furthestElevation = j;
                            }
                        }
                    }
                    posibleTiles.Add(checktile);
                    posibleTilesPath.Add(pos);
                }

            }
        }
    }

    public IEnumerator MoveAlongPath(GameObject enemy, Vector2 enemyPos)
    {
        Vector2 oneToOnePos = enemyPos;
        Vector2 pos;
        for (int i = 0; i < actualPath.Count; i++)
        {
            //Debug.Log(actualPath[i]);
            oneToOnePos = actualPath[i];
            pos = mapManager.OneToOneToPos(oneToOnePos);
            enemy.transform.position = new Vector3(pos.x, pos.y, enemy.transform.position.z);
            yield return new WaitForSeconds(enemyMoveDelay);
        }
        doneMoving = true;
    }
    public Vector2 TakeStep(Vector2 enemyPos, int moveLeft)
    {
        Vector2 checktile = new Vector2();
        Vector2 currentTile = enemyPos;
        int elevation = currentElevation;
        if (currentElevation == 0)
        {
            return enemyPos;
        }
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = currentTile + Vector2.up; break;
                case 1: checktile = currentTile + Vector2.down; break;
                case 2: checktile = currentTile + Vector2.right; break;
                case 3: checktile = currentTile + Vector2.left; break;
                case 4: checktile = currentTile + Vector2.up + Vector2.right; ; break;
                case 5: checktile = currentTile + Vector2.down + Vector2.left; break;
            }
            if (elevations[elevation - 1].Contains(checktile))
            {
                if (safeTiles.Contains(checktile))
                {
                    return checktile;
                }
                else if (TestForSafeAround(checktile, elevation - 1, moveLeft - 1))
                {
                    //Debug.Log("checktile is safe, should move to" + checktile);
                    return checktile;
                }
            }

        }
        //Debug.Log("no tile to go to");
        //Debug.Log(elevation - 1);
        return enemyPos;
    }

    public bool TestForSafeAround(Vector2 tile, int elevation, int moveLeft)
    {
        Vector2 checktile = new Vector2();
        Vector2 currentTile = new Vector2();
        if (moveLeft == 0)
        {
            return false;
        }
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = currentTile + Vector2.up; break;
                case 1: checktile = currentTile + Vector2.down; break;
                case 2: checktile = currentTile + Vector2.right; break;
                case 3: checktile = currentTile + Vector2.left; break;
                case 4: checktile = currentTile + Vector2.up + Vector2.right; ; break;
                case 5: checktile = currentTile + Vector2.down + Vector2.left; break;
            }
            if (elevations[elevation - 1].Contains(checktile))
            {
                if (safeTiles.Contains(checktile))
                {
                    //Debug.Log("checktile is safe " + checktile);
                    return true;
                }
                else if (TestForSafeAround(checktile, elevation - 1, moveLeft - 1))
                {
                    return true;
                }
            }
        }
        //Debug.Log("not safe");
        return false;

    }
}
