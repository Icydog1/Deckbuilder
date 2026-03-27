using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    private int baseMoveCost = 5;
    private int moveValue;
    private int currentElevation;
    private Vector2 currentPos;
    private Enemy currentEnemy;
    private int enemyElevation;
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
        inRange = false;
        currentPos = selfPos;
        //finds posible spots that woul be good with ending on
        findPosInRange(targetPos, range);
        Debug.Log("range done");
        List<Vector2> posibleLocations = new List<Vector2>(safeTiles);
        //builds heightmap out from ending spots
        findPathToArea(selfPos, posibleLocations);
        Debug.Log("area done");
        if (!inRange)
        {
            //finds the path from the enemy with current movement that gets them as close to player a posible
            findPosiblePaths(selfPos);
            //moves along path
            StartCoroutine(MoveAlongPath(self, selfPos));
            yield return new WaitUntil(() => doneMoving == true);
            doneMoving = false;
        }

        self.GetComponent<Enemy>().ActionDone();
    }

    public IEnumerator PathToTile(Vector2 selfPos, Vector2 targetPos, int newMoveValue, bool jump = false, bool fly = false)
    {
        yield return new WaitUntil(() => doneMoving == true);

    }
    /*
    public void findPathFromToRange(Vector2 selfPos, Vector2 targetPos, int range)
    {
        inRange = false;
        currentPos = selfPos;
        findPosInRange(targetPos, range);
        List<Vector2> posibleLocations = new List<Vector2>(safeTiles);
        findPathToArea(selfPos, posibleLocations);
    }
    */




    //finds all tiles with a specifc range of a tile
    public void findPosInRange(Vector2 targetPos, int range)
    {
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        for (int i = 0; i <= range; i++)
        {
            currentElevation = i - 1;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            //adds starting tile
            if (i == 0)
            {
                elevations[0].Add(targetPos);
            }
            //each tile spreads to other tiles ignoring move costs
            else
            {
                foreach (Vector2 pos in elevations[currentElevation])
                {
                    buildElevation(pos, true, false);
                }

            }
            if (i >= 10000)
            {
                pathFound = true;
                Debug.Log("range pathfinding timed out");
            }
        }
    }
    //builds heightmap with an area as the starting height
    public void findPathToArea(Vector2 selfPos, List<Vector2> targetArea)
    {
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        if (targetArea.Contains(selfPos))
        {
            pathFound = true;
            inRange = true;
        }
        for (int i = 0; pathFound == false; i++)
        {
            currentElevation = i - 1;
            elevations.Add(new List<Vector2>());
            //adds staring tiles with their starting heights
            if (i == 0)
            {
                foreach (Vector2 pos in targetArea)
                {
                    GameObject tile = mapManager.GetTileAtHex(pos);
                    AddToElevation(pos, tile);
                    checkedTiles.Add(pos);
                    safeTiles.Add(pos);

                }
            }
            //builds elevations from previus height
            else
            {
                foreach (Vector2 pos in elevations[currentElevation])
                {
                    buildElevation(pos, false, false);
                }

            }
            //failsafe in case somting fais so it isnt a infinite loop
            if (i >= 10000)
            {
                pathFound = true;
                Debug.Log("area pathfinding timed out");
            }
        }
        enemyElevation = currentElevation;
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
            currentElevation = i - 1;
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
                foreach (Vector2 pos in elevations[currentElevation])
                {
                    buildElevation(pos, false, true);
                }
            }

            if (i >= 10000)
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

    public void AddToElevation(Vector2 tilePos, GameObject tile, bool isRange = false, bool pathFromEnemy = false)
    {
        int moveCost;
        if (isRange)
        {
            moveCost = 1;
        }
        else if (isJump || isFly)
        {
            moveCost = baseMoveCost;
        }
        else
        {
            moveCost = tile.GetComponent<Tile>().MoveCost;
        }
        while (elevations.Count <= moveCost + currentElevation)
        {
            elevations.Add(new List<Vector2>());
        }
        elevations[moveCost + currentElevation].Add(tilePos);
        if (pathFromEnemy && moveCost + currentElevation > moveValue)
        {
            impassableTiles.Add(tilePos);
        }
    }
    public void buildElevation(Vector2 pos, bool range, bool pathFromEnemy)
    {
        Vector2 checktile = new Vector2();
        //for each tile in the six directions
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
            //if it isnt already checked
            if (!checkedTiles.Contains(checktile))
            {
                GameObject tile = mapManager.GetTileAtHex(checktile);
                GameObject entity = mapManager.GetEntityOnHex(checktile);
                //if the tile is the tile the pathfinder is on
                if (checktile == currentPos && !pathFromEnemy)
                {
                    pathFound = true;
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.cyan;
                    safeTiles.Add(checktile);
                    AddToElevation(checktile, tile, range);
                    Debug.Log("path found");

                }
                //if tile is impasible
                else if (tile.GetComponent<Wall>() || (tile.GetComponent<Obstacle>() && !(range || isJump || isFly)) || (entity && entity.GetComponent<PlayerControler>() && !(range || isJump || isFly)))
                {
                    impassableTiles.Add(checktile);
                }
                //if tile is unsafe
                else if ((entity && entity.GetComponent<Enemy>()) || (tile.GetComponent<Obstacle>() && ((range || isJump) && !isFly)) || (entity && entity.GetComponent<PlayerControler>() && (range || isJump || isFly)))
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.yellow;
                    unSafeTiles.Add(checktile);
                    AddToElevation(checktile, tile, range, pathFromEnemy);

                }
                //if tile is safe
                else
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.blue;
                    safeTiles.Add(checktile);
                    AddToElevation(checktile, tile, range, pathFromEnemy);
                }
                checkedTiles.Add(checktile);

                if (pathFromEnemy && !impassableTiles.Contains(checktile))
                {
                    if (originalSafeTiles.Contains(checktile))
                    {
                        //for each elevation lower than the 
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
        float newEnemyMoveDelay = enemyMoveDelay;
        if (enemyElevation >= 10)
        {
            newEnemyMoveDelay = (enemyMoveDelay * (10 + 20)) / (enemyElevation + 20);
        }
        if (newEnemyMoveDelay > enemyMoveDelay)
        {
            Debug.Log(enemy + " tried to move with delay " + enemyMoveDelay + " delay");

        }
        //Debug.Log(enemyMoveDelay + "original delay");
        //Debug.Log(newEnemyMoveDelay + "new delay");

        Vector2 oneToOnePos = enemyPos;
        Vector2 pos;
        for (int i = 0; i < actualPath.Count; i++)
        {
            Debug.Log(actualPath[i]);
            oneToOnePos = actualPath[i];
            pos = mapManager.OneToOneToPos(oneToOnePos);
            enemy.transform.position = new Vector3(pos.x, pos.y, enemy.transform.position.z);
            yield return new WaitForSeconds(newEnemyMoveDelay);
        }
        doneMoving = true;
    }
    /*
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
            for (int j = 0; j < elevation; j++)
            {
                if (elevations[j].Contains(checktile))
                {
                    if (safeTiles.Contains(checktile))
                    {
                        return checktile;
                    }
                    else if (TestForSafeAround(checktile, j, moveLeft - 1))
                    {
                        //Debug.Log("checktile is safe, should move to" + checktile);
                        return checktile;
                    }
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
            for (int j = 0; j < elevation; j++)
            {
                if (elevations[j].Contains(checktile))
                {
                    if (safeTiles.Contains(checktile))
                    {
                        //Debug.Log("checktile is safe " + checktile);
                        return true;
                    }
                    else if (TestForSafeAround(checktile, j, moveLeft - j))
                    {
                        return true;
                    }
                }
            }

        }
        //Debug.Log("not safe");
        return false;

    }
    */
}
