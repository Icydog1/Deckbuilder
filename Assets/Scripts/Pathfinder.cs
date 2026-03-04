using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private MapManager mapManager;


    private List<List<Vector2>> elevations = new List<List<Vector2>>();
    private List<Vector2> currentHeight = new List<Vector2>();
    private List<Vector2> checkedTiles = new List<Vector2>();
    private List<Vector2> safeTiles = new List<Vector2>();
    private List<Vector2> unSafeTiles = new List<Vector2>();
    private List<Vector2> impassableTiles = new List<Vector2>();


    private bool pathFound;
    private int currentElevation;
    private Vector2 finalTile;
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
    public void findPathFromToRange(Vector2 selfPos, Vector2 targetPos, int range = 1, bool jump = false, bool fly = false)
    {
        findPosInRange(targetPos, range, fly);
        //Debug.Log("found range");
        List<Vector2> posibleLocations = safeTiles;
        //Debug.Log("safeTiles " + posibleLocations[1]);
        findPathToArea(selfPos, posibleLocations, jump, fly);
    }


    public void findPathToArea(Vector2 selfPos, List<Vector2> targetArea, bool jump, bool fly)
    {
        //Debug.Log("pathfinding started initial cords" + targetPos + " to " + selfPos);
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        finalTile = selfPos;
        for (int i = 0; pathFound == false; i++)
        {
            currentElevation = i;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            if (i == 0)
            {
                foreach (Vector2 pos in targetArea)
                {
                    elevations[0].Add(pos);
                    Debug.Log("start tile" + pos);
                }
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation - 1].Count; j++)
                {
                    buildElevation(elevations[currentElevation - 1][j], false, jump, fly);
                }
            }

            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("pathfinding timed out");
            }
        }
    }

    public void findPosInRange(Vector2 targetPos, int range, bool fly)
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
                    buildElevation(elevations[currentElevation - 1][j], true, false, fly);
                }
            }

            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("pathfinding timed out");
            }
        }
    }

    public void findPathFrom(Vector2 selfPos, Vector2 targetPos, bool range = false, bool jump = false, bool fly = false)
    {
        //Debug.Log("pathfinding started initial cords" + targetPos + " to " + selfPos);
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unSafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
        finalTile = selfPos;
        for (int i = 0; pathFound == false; i++)
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
                    buildElevation(elevations[currentElevation - 1][j], range, jump, fly);
                }
            }

            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("pathfinding timed out");
            }
        }
    }


    public void buildElevation(Vector2 pos, bool range, bool jump, bool fly)
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
                if (checktile == finalTile)
                {
                    pathFound = true;
                    Debug.Log("path found");
                    //Debug.Log("final elevation " + currentElevation);
                    //Debug.Log(mapManager.GetTileAtHex(checktile).transform.position);

                }
                //detectobsticals
                GameObject tile = mapManager.GetTileAtHex(checktile);
                GameObject entity = mapManager.GetEntityOnHex(checktile);

                if (tile.GetComponent<Wall>() || (tile.GetComponent<Obstacle>() && !(range || jump || fly)) || (entity.GetComponent<PlayerControler>() && !(range || jump || fly)))
                {
                    impassableTiles.Add(checktile);
                    //Debug.Log("obstical found at " + checktile);
                }
                else if (entity.GetComponent<Enemy>() || (tile.GetComponent<Obstacle>() && (range || jump || fly)) || (entity.GetComponent<PlayerControler>() && (range || jump || fly)))
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.yellow;
                    unSafeTiles.Add(checktile);
                    elevations[currentElevation].Add(checktile);

                    //Debug.Log("unsafe tile found at " + checktile);
                }
                else
                {
                    GameObject border = tile.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.blue;
                    safeTiles.Add(checktile);
                    elevations[currentElevation].Add(checktile);
                }
                checkedTiles.Add(checktile);
            }
        }
    }

    public void MoveAlongPath(int moveValue, GameObject enemy, Vector2 enemyPos)
    {
        int moveLeft = moveValue;
        for (int i = 0; i < moveValue; i++)
        {
            enemy.transform.position = mapManager.OneToOneToPos(TakeStep(enemyPos, moveLeft));
            moveLeft--;
            Debug.Log("moved once");
        }


    }
    public Vector2 TakeStep(Vector2 enemyPos, int moveLeft)
    {
        Vector2 checktile = new Vector2();
        Vector2 currentTile = enemyPos;
        int elevation = currentElevation;
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
                    return checktile;
                }
            }
            else
            {
                Debug.Log("no tile to go to");
                Debug.Log(elevation - 1);
            }
        }
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
                    return true;
                }
                else if (TestForSafeAround(checktile, elevation - 1, moveLeft -1))
                {
                    return true;
                }
            }
        }
        Debug.Log("not safe");
        return false;

    }
}
