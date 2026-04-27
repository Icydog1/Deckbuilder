using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.EventSystems.EventTrigger;

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
    public List<Vector2> ActualPath { get { return actualPath; } }

    private Vector2 furthestPoint;
    private int furthestElevation;

    private bool pathFound, inRange;
    private int endElevation;
    private bool isJump, isFly;
    private int moveValue;
    private int moveLeft;
    public int MoveLeft { get { return moveLeft; } set { moveLeft = value; } }

    private int currentElevation;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private GameObject currentFigure;
    private int figureElevation;
    private float figureMoveDelay = 0.1f;
    private bool doneMoving;
    public bool DoneMoving { get { return doneMoving; } set { doneMoving = value; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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

    //findPosInRange and findPathToArea set target pos as 0 and build outwards until fining starting pos 
    //each tile assings value to adjasent tiles elevation
    //findPosiblePaths builds outwards from starting pos and finds the spot that is moveable to that is closest to 0 from previus functions
    //each tile adds its own value to elevation




    public IEnumerator PathfindTowards(Vector2 selfPos, Vector2 newTargetPos, GameObject self, int newMoveValue, int range = 1, bool jump = false, bool fly = false)
    {
        currentFigure = self;
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        inRange = false;
        currentPos = selfPos;
        targetPos = newTargetPos;
        //finds posible spots that woul be good with ending on
        findPosInRange(targetPos, range);
        //Debug.Log("range done");
        List<Vector2> posibleLocations = new List<Vector2>(safeTiles);
        //builds heightmap out from ending spots
        findPathToArea(selfPos, posibleLocations);
        //Debug.Log("area done");
        if (!inRange)
        {
            //finds the path from the figure with current movement that gets them as close to player a posible
            findPosiblePaths(selfPos);
            //moves along path
            StartCoroutine(MoveAlongPath(currentFigure, selfPos));
            yield return new WaitUntil(() => doneMoving == true);
            doneMoving = false;
        }

        self.GetComponent<Figure>().ActionDone();
    }

    public void PlanPathToTile(Vector2 selfPos, Vector2 newTargetPos, GameObject self, int newMoveValue, bool jump = false, bool fly = false)
    {
        currentFigure = self;
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        currentPos = selfPos;
        targetPos = newTargetPos;
        //builds heightmap out from ending spots
        findPathToArea(selfPos, new List<Vector2>(){targetPos});
        //finds the path from the figure with current movement that gets them as close to player a posible
        //Debug.Log("area done");
        findPosiblePaths(selfPos);
        //Debug.Log("path Found");
    }

    public List<Figure> GetFiguresInRange(Vector2 selfPos, int range, GameObject self)
    {
        List<Figure> figures = new List<Figure>();
        currentFigure = self;
        currentPos = selfPos;
        findPosInRange(selfPos, range);
        foreach (List<Vector2> elevation in elevations)
        {
            foreach (Vector2 pos in elevation)
            {
                GameObject entity = mapManager.GetEntityOnHex(pos);
                if (entity != null)
                {
                    figures.Add(entity.GetComponent<Figure>());
                }
            }
        }
        return figures;
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
    public void findPosInRange(Vector2 newTargetPos, int range)
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
                GetTileType(newTargetPos, 0, true);
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
            endElevation = 0;
            inRange = true;
        }
        else
        {
            endElevation = int.MaxValue - 1;
        }
        for (int i = 0; i <= endElevation + 1; i++)
        {
            currentElevation = i - 1;
            elevations.Add(new List<Vector2>());
            //adds staring tiles with their starting heights
            if (i == 0)
            {
                foreach (Vector2 pos in targetArea)
                {
                    GetTileType(pos, 1);
                    /*
                    if (mapManager.GetTileAtHex(pos).GetComponent<Stair>())
                    {

                        checkedTiles.Add(pos);
                        safeTiles.Add(pos);
                        elevations[0].Add(pos);

                    }
                    else
                    {
                    }
                    */
                }
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation].Count; j++)
                {
                    buildElevation(elevations[currentElevation][j], false, false);
                }


            }
            //failsafe in case somting fais so it isnt a infinite loop
            if (i >= 10000)
            {
                endElevation = 0;
                //Debug.Log("area pathfinding timed out");
            }
        }
        /*
        for (int i = 0; i < elevations.Count; i++)
        {
            Debug.Log(i + "test");
            foreach (Vector2 pos in elevations[i])
            {
                mapManager.GetTileAtHex(pos).transform.position += new Vector3(0, 0, i);
            }
        }
        */
        
        
        figureElevation = currentElevation;
    }

    public void findPosiblePaths(Vector2 selfPos)
    {
        //Debug.Log("finding posible paths");
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
        //border.GetComponent<SpriteRenderer>().color = Color.red;
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

    public void AddToElevation(Vector2 tilePos, GameObject tile, bool isRange = false, bool pathFromFigure = false, int addedCost = 0)
    {
        int moveCost;
        //Debug.Log(isJump);
        if (isRange)
        {
            moveCost = 1;
        }
        else if (isJump || isFly)
        {
            //moveCost = mapManager.BaseMoveCost;
            if (pathFromFigure)
            {
                moveCost = Mathf.Min(tile.GetComponent<Tile>().MoveCost, mapManager.BaseMoveCost);
            }
            else
            {
                moveCost = Mathf.Min(addedCost, mapManager.BaseMoveCost);
            }
        }
        else
        {
            if (pathFromFigure)
            {
                moveCost = tile.GetComponent<Tile>().MoveCost;
            }
            else
            {
                moveCost = addedCost;
            }
        }
        while (elevations.Count <= moveCost + currentElevation)
        {
            elevations.Add(new List<Vector2>());
        }
        elevations[moveCost + currentElevation].Add(tilePos);
        if (pathFromFigure && moveCost + currentElevation > moveValue)
        {
            impassableTiles.Add(tilePos);
        }
    }
    public void buildElevation(Vector2 pos, bool range, bool pathFromFigure)
    {
        Vector2 checktile = new Vector2();
        GameObject originalTile = mapManager.GetTileAtHex(pos);
        //for each tile in the six directions
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = pos + Vector2.up; break ;
                case 1: checktile = pos + Vector2.down; break;
                case 2: checktile = pos + Vector2.right; break;
                case 3: checktile = pos + Vector2.left; break;
                case 4: checktile = pos + Vector2.up + Vector2.right; break;
                case 5: checktile = pos + Vector2.down + Vector2.left; break;
            }
            //if it isnt already checked
            if (!checkedTiles.Contains(checktile))
            {
                checkedTiles.Add(checktile);
                GameObject tile = mapManager.GetTileAtHex(checktile);
                if (tile != null)
                {
                    GameObject entity = mapManager.GetEntityOnHex(checktile);
                    //if the tile is the tile the pathfinder is on
                    if (checktile == currentPos && !pathFromFigure)
                    {
                        pathFound = true;
                        GameObject border = tile.transform.Find("Border").gameObject;
                        //border.GetComponent<SpriteRenderer>().color = Color.cyan;
                        safeTiles.Add(checktile);
                        AddToElevation(checktile, tile, range);
                        endElevation = currentElevation + tile.GetComponent<Tile>().MoveCost;
                        //Debug.Log(currentElevation + tile.GetComponent<Tile>().MoveCost + " final elevation");
                    }
                    else
                    {
                        GetTileType(checktile, originalTile.GetComponent<Tile>().MoveCost, range, pathFromFigure);
                    }
                    if (pathFromFigure && !impassableTiles.Contains(checktile))
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
    }

    public void GetTileType(Vector2 checktile, int addedCost, bool range = false, bool pathFromFigure = false)
    {

        checkedTiles.Add(checktile);
        GameObject tile = mapManager.GetTileAtHex(checktile);
        if (tile != null)
        {
            GameObject entity = mapManager.GetEntityOnHex(checktile);
            if ((tile.GetComponent<Wall>() && !(currentFigure.GetComponent<PlayerControler>() && targetPos == checktile && (tile.GetComponent<Door>() || tile.GetComponent<Stair>())))  || (tile.GetComponent<Obstacle>() && !(range || isJump || isFly)) || (entity && entity.GetComponent<Figure>() && entity.GetComponent<Figure>().Team != currentFigure.GetComponent<Figure>().Team && !(range || isJump || isFly)))
            {
                impassableTiles.Add(checktile);
            }
            //if tile is unsafe
            else if ((tile.GetComponent<Obstacle>() && !isFly) || entity && entity.GetComponent<Figure>())
            {
                GameObject border = tile.transform.Find("Border").gameObject;
                //border.GetComponent<SpriteRenderer>().color = Color.yellow;
                unSafeTiles.Add(checktile);
                AddToElevation(checktile, tile, range, pathFromFigure, addedCost);

            }
            //if tile is safe
            else
            {
                GameObject border = tile.transform.Find("Border").gameObject;
                //border.GetComponent<SpriteRenderer>().color = Color.blue;
                safeTiles.Add(checktile);
                AddToElevation(checktile, tile, range, pathFromFigure, addedCost);
            }
        }
    }
    public IEnumerator MoveAlongPath(GameObject figure, Vector2 figurePos)
    {
        figure.GetComponent<Figure>().IsPreformingAnimation = true;
        float newFigureMoveDelay = figureMoveDelay;
        if (figureElevation >= 10)
        {
            newFigureMoveDelay = (figureMoveDelay * (10 + 20)) / (figureElevation + 20);
        }
        if (newFigureMoveDelay > figureMoveDelay)
        {
            Debug.Log(figure + " tried to move with delay " + figureMoveDelay + " delay");

        }
        //Debug.Log(figureMoveDelay + "original delay");
        //Debug.Log(newFigureMoveDelay + "new delay");

        Vector2 oneToOnePos = figurePos;
        Vector2 pos;
        for (int i = 0; i < actualPath.Count; i++)
        {
            oneToOnePos = actualPath[i];
            if (isJump || isFly)
            {
                moveLeft -= mapManager.BaseMoveCost;
            }
            else
            {
                moveLeft -= mapManager.GetTileAtHex(actualPath[i]).GetComponent<Tile>().MoveCost;
            }
            pos = mapManager.OneToOneToPos(oneToOnePos);
            figure.transform.position = new Vector3(pos.x, pos.y, figure.transform.position.z);
            yield return new WaitForSeconds(newFigureMoveDelay);
        }
        doneMoving = true;
        figure.GetComponent<Figure>().IsPreformingAnimation = false;
    }
    /*
    public Vector2 TakeStep(Vector2 figurePos, int moveLeft)
    {
        Vector2 checktile = new Vector2();
        Vector2 currentTile = figurePos;
        int elevation = currentElevation;
        if (currentElevation == 0)
        {
            return figurePos;
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
        return figurePos;
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
