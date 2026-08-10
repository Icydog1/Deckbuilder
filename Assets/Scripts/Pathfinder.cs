using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ElevationsWatcher
{
    public List<Vector2> elevation;
}
public class Pathfinder : MonoBehaviour
{
    private MapManager mapManager;
    private PlayerControler playerControler;

    //private struct TileStruct
    //{
    //    Vector2 pos;
    //    GameObject tile;
    //    int currentElevation;
    //}

    private List<ElevationsWatcher> playerElevationsWatcher = new List<ElevationsWatcher>();
    private List<ElevationsWatcher> elevationsWatcher = new List<ElevationsWatcher>();


    private List<List<Vector2>> elevations = new List<List<Vector2>>();
    private List<Vector2> currentHeight = new List<Vector2>();
    private List<Vector2> checkedTiles = new List<Vector2>();
    //tiles figue can end their move on
    private List<Vector2> safeTiles = new List<Vector2>();
    //tile figure can move throug but not end on
    private List<Vector2> unsafeTiles = new List<Vector2>();
    //tile figure can not move through
    private List<Vector2> impassableTiles = new List<Vector2>();

    private List<List<Vector2>> originalElevations = new List<List<Vector2>>();
    private List<Vector2> originalSafeTiles = new List<Vector2>();
    private List<Vector2> posibleTiles = new List<Vector2>();
    private List<Vector2> posibleTilesPath = new List<Vector2>();

    private List<List<Vector2>> playerElevations = new List<List<Vector2>>();
    private List<Vector2> playerSafeTiles = new List<Vector2>();
    private List<Vector2> playerUnsafeTiles = new List<Vector2>();
    private List<Vector2> playerImpassableTiles = new List<Vector2>();

    private List<Vector2> actualPath = new List<Vector2>();
    public List<Vector2> ActualPath { get { return actualPath; } }

    private Vector2 furthestPoint;
    private int furthestElevation;

    private bool pathFound, inRange, noMove;
    private int endElevation;
    private bool isJump, isFly;
    private int moveValue;
    private int moveLeft;
    public int MoveLeft { get { return moveLeft; } set { moveLeft = value; } }

    private int currentElevation;
    private Vector2 currentPos;
    private Vector2 targetPos;
    private GameObject currentFigure;
    private int currentTeam;
    private int figureElevation;
    private float figureMoveDelay = 0.1f;
    private bool doneMoving;
    public bool DoneMoving { get { return doneMoving; } set { doneMoving = value; } }
    private Dictionary<Vector2,Tile> tiles = new Dictionary<Vector2,Tile>();
    public Dictionary<Vector2, Tile> Tiles { get { return tiles; } }
    void Awake()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
    }


    //findPosInRange and findPathToArea set target hexPos as 0 and build outwards until fining starting hexPos 
    //each tile assings value to adjasent tiles elevation
    //findPosiblePaths builds outwards from starting hexPos and finds the spot that is moveable to that is closest to 0 from previus functions
    //each tile adds its own value to elevation

    public void ResetPathfining()
    {
        elevations.Clear();
        checkedTiles.Clear();
        safeTiles.Clear();
        unsafeTiles.Clear();
        impassableTiles.Clear();
        pathFound = false;
    }
    public IEnumerator BuildPlayerElevationMap(int range = 1, bool jump = true) //jump true range 1
    {
        currentFigure = gameObject; //nothing
        currentTeam = 1;
        //Debug.Log("BuildPlayerElevationMap");
        isJump = jump;
        isFly = false;
        targetPos = playerControler.HexPos;
        currentPos = targetPos + Vector2.left * 117381.168f; //117381.168f is random number to make sure currentPos is encountered as it sold cover the entire map and never stop
        //finds posible spots that woul be good with ending on
        //Debug.Log("range done");
        //builds heightmap out from ending spots
        findPathToArea(new List<Vector2>() { targetPos });
        playerElevations = new List<List<Vector2>>(elevations);
        playerSafeTiles = new List<Vector2>(safeTiles);
        playerUnsafeTiles = new List<Vector2>(unsafeTiles);
        playerImpassableTiles = new List<Vector2>(impassableTiles);
        //for (int i = 0; i < playerElevations.Count; i++)
        //{
        //    foreach (Vector2 tileCords in playerElevations[i])
        //    {
        //        GameObject tile = mapManager.GetTileAtHex(tileCords);
        //        tile.GetComponent<Tile>().distance = i;
        //    }
        //}
        //Debug.Log("area done");
        yield return null;

    }
    //pathfinding for figres far away from player, less accurate, uses one global player hiehgtmap for everything
    public IEnumerator FarPathfindTowards(Vector2 selfPos, Vector2 newTargetPos, GameObject self, int newMoveValue, int range = 1, bool jump = false, bool fly = false)
    {
        //Debug.Log("FarPathfindTowards");

        currentFigure = self;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        for (int i = 0; i < playerElevations.Count; i++)
        {
            if (playerElevations[i].Contains(selfPos))
            {
                currentElevation = i;
            }
        }
        //Debug.Log("currentElevation " + currentElevation);

        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        inRange = false;
        noMove = false;
        currentPos = selfPos;
        GameObject currentTile = mapManager.GetTileAtHex(currentPos);
        targetPos = newTargetPos;
        //finds the path from the figure with current movement that gets them as close to player a posible
        findPosiblePaths(selfPos, playerElevations, playerSafeTiles);
        //Debug.Log("furthestPoint " + furthestPoint);
        findActualPath(selfPos);
        //moves along path
        yield return StartCoroutine(MoveAlongPath(currentFigure, selfPos));
        //yield return new WaitUntil(() => doneMoving == true);
        //doneMoving = false;
        Vector2 newPos = mapManager.RectToHex(currentFigure.transform.position);
        playerUnsafeTiles.Remove(newPos);
        playerSafeTiles.Remove(newPos);
        playerUnsafeTiles.Add(newPos);
        if (!currentTile.GetComponent<Obstacle>())
        {
            playerUnsafeTiles.Remove(selfPos);
            playerSafeTiles.Add(selfPos);
        }
        self.GetComponent<Figure>().ActionDone();
    }
    //pathfinding for figres close to player, more accurate, slower
    public IEnumerator PathfindTowards(Vector2 selfPos, Vector2 newTargetPos, GameObject self, int newMoveValue, int range = 1, bool jump = false, bool fly = false)
    {
        currentFigure = self;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        inRange = false;
        noMove = false;
        currentPos = selfPos;
        targetPos = newTargetPos;
        //finds posible spots that woul be good with ending on
        findPosInRange(targetPos, range);
        //Debug.Log("range done");
        List<Vector2> posibleLocations = new List<Vector2>(safeTiles);
        //builds heightmap out from ending spots
        findPathToArea(posibleLocations);
        //Debug.Log("area done");
        if (!noMove)
        {
            //finds the path from the figure with current movement that gets them as close to player a posible
            findPosiblePaths(selfPos, elevations, safeTiles);
            findActualPath(selfPos);
            //moves along path
            yield return StartCoroutine(MoveAlongPath(currentFigure, selfPos));
            //yield return new WaitUntil(() => doneMoving == true);
            //doneMoving = false;
        }

        self.GetComponent<Figure>().ActionDone();
    }
    //pathfinds to a specific tile
    public void PlanPathToTile(Vector2 selfPos, Vector2 newTargetPos, GameObject self, int newMoveValue, bool jump = false, bool fly = false)
    {
        currentFigure = self;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        currentPos = selfPos;
        targetPos = newTargetPos;
        //builds heightmap out from ending spots
        findPathToArea(new List<Vector2>(){targetPos});
        //finds the path from the figure with current movement that gets them as close to player a posible
        //Debug.Log("area done");
        //Debug.Log("safeTiles 1 " + mapManager.RectToHex(safeTiles[0]) + " 2 " + mapManager.RectToHex(safeTiles[1]));
        findPosiblePaths(selfPos, elevations, safeTiles);
        findActualPath(selfPos);
        //Debug.Log("path Found");
    }
    //returns tiles a specific move could get to
    public List<Vector2>[] PlanPosiblePaths(Vector2 selfPos, GameObject self, int newMoveValue, bool jump = false, bool fly = false)
    {
        currentFigure = self;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        moveValue = newMoveValue;
        isJump = jump;
        isFly = fly;
        currentPos = selfPos;
        //findPathToArea(selfPos, new List<Vector2>() { targetPos });
        findPosiblePaths(selfPos, elevations, safeTiles);
        List<Vector2>[] posibleTiles = new List<Vector2>[2];
        posibleTiles[0] = new List<Vector2>(safeTiles);
        posibleTiles[1] = new List<Vector2>(unsafeTiles);
        return posibleTiles;
    }
    //returns a list of tiles within a range of another set of tiles
    public List<Vector2> PlanTargetableLocations(List<Vector2> startingLocations, int range)
    {
        ResetPathfining();

        if (range == Var.infinityValue)
        {
            range = Var.maxValue;
        }
        for (int i = 0; i <= range; i++)
        {
            currentElevation = i - 1;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            //adds starting tile
            if (i == 0)
            {
                foreach (Vector2 tile in startingLocations)
                {
                    GetTileType(tile, 0, true);
                }
            }
            //each tile spreads to other tiles ignoring move costs
            else
            {
                foreach (Vector2 pos in elevations[currentElevation])
                {
                    buildElevation(pos, true, true);
                }

            }
            if (i > Var.maxValue)
            {
                range = i;
                Debug.Log("range pathfinding timed out");
            }
        }
        List<Vector2> reachableTiles = safeTiles;
        reachableTiles.AddRange(unsafeTiles);
        return reachableTiles;
    }
    //caluculates the movement it wold take the player to move to each tile from a given tile
    public void DisplayMoveField(Vector2 hexPos)
    {
        currentFigure = RefrenceStorage.player;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        ResetPathfining();
        isJump = false;
        isFly = false;
        endElevation = Var.maxValue;
        int currentTileMoveCost = 0;
        for (int i = 0; i <= endElevation + 1; i++)
        {
            currentElevation = i - 1;
            //adds staring tiles with their starting heights
            if (i == 0)
            {
                elevations.Add(new List<Vector2>());
                GetTileType(hexPos, 1);
                currentTileMoveCost = tiles[hexPos].MoveCost;

            }
            else
            {
                //if all tiles have bean checked stop
                if (elevations.Count - 1 >= currentElevation)
                {
                    for (int j = 0; j < elevations[currentElevation].Count; j++)
                    {
                        tiles[elevations[currentElevation][j]].MoveCostDisplay.DisplayVariable(currentElevation + 1 - currentTileMoveCost);
                        buildElevation(elevations[currentElevation][j], false, false);
                    }
                }
                else
                {
                    noMove = true;
                    endElevation = -1;
                }
            }
            //failsafe in case somting fais so it isnt a infinite loop
            if (i > Var.maxValue)
            {
                noMove = true;
                endElevation = -1;
                Debug.Log("area pathfinding timed out");
            }
        }

    }

    //finds all figrues within a specific range
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

    //public IEnumerator PathToTile(Vector2 selfPos, Vector2 targetPos, int newMoveValue, bool jump = false, bool fly = false)
    //{
    //    yield return new WaitUntil(() => doneMoving == true);

    //}
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




    //find the distance by range that one hexPos is from another

    public int GetDistanceTo(Vector2 newTargetPos, Vector2 selfPos)
    {
        currentPos = selfPos;
        targetPos = newTargetPos;
        ResetPathfining();
        if (newTargetPos == selfPos)
        {
            return 0;
        }
        //stops when 
        for (int i = 0; !pathFound; i++)
        {
            //elevation equals distance pevius tiles were on
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
                    buildElevation(pos, true, true);
                }

            }
            //need to add detection when all tiles have bean serched
            if (i > Var.maxValue)
            {
                pathFound = true;
                //Debug.Log("DistanceTo pathfinding timed out");
                return -1;
            }

        }
        return currentElevation + 1;
    }

    public int GetMoveCostTo(Vector2 newTargetPos, Vector2 selfPos, bool jump = false, bool fly = false)
    {
        currentFigure = RefrenceStorage.player;
        currentTeam = currentFigure.GetComponent<Figure>().Team;
        currentPos = selfPos;
        targetPos = newTargetPos;
        isJump = jump;
        isFly = fly;
        ResetPathfining();
        //stops when 
        findPathToArea(new List<Vector2> { newTargetPos });
        return endElevation;
    }
    
    //finds all tiles with a specifc range of a tile
    public void findPosInRange(Vector2 newTargetPos, int range)
    {
        ResetPathfining();
        if (range == Var.infinityValue)
        {
            range = Var.maxValue;
        }
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
                    buildElevation(pos, true, true);
                }

            }
            if (i > Var.maxValue)
            {
                range = i;
                Debug.Log("range pathfinding timed out");
            }
        }
    }
    //builds heightmap with an area as the starting height
    public void findPathToArea(List<Vector2> targetArea)
    {
        ResetPathfining();
        //if mover is already in a desired location
        if (targetArea.Contains(currentPos))
        {
            endElevation = 0;
            noMove = true;
        }
        else if (targetArea.Count == 0)
        {
            endElevation = -1;
            noMove = true;
            //Debug.Log("No valid hex to move to");
        }
        else
        {
            endElevation = Var.maxValue + 1;
        }
        for (int i = 0; i <= endElevation + 1; i++)
        {
            currentElevation = i - 1;
            //adds staring tiles with their starting heights
            if (i == 0)
            {
                elevations.Add(new List<Vector2>());
                foreach (Vector2 pos in targetArea)
                {
                    GetTileType(pos, 1);
                    /*
                    if (mapManager.GetTileAtHex(hexPos).GetComponent<Stair>())
                    {

                        checkedTiles.Add(hexPos);
                        safeTiles.Add(hexPos);
                        elevations[0].Add(hexPos);

                    }
                    else
                    {
                    }
                    */
                }
            }
            else
            {
                //if all tiles have bean checked stop
                if (elevations.Count - 1 >= currentElevation)
                {
                    for (int j = 0; j < elevations[currentElevation].Count; j++)
                    {
                        buildElevation(elevations[currentElevation][j], false, true);
                    }
                }
                else
                {
                    noMove = true;
                    endElevation = -1;
                }
            }
            //failsafe in case somting fais so it isnt a infinite loop
            if (i > Var.maxValue)
            {
                noMove = true;
                endElevation = -1;
                Debug.Log("area pathfinding timed out");
            }
        }
        figureElevation = currentElevation;
    }
    //builds a second hightmap starting at active figure to display all posible spaces that the fiugre can move to
    public void findPosiblePaths(Vector2 selfPos, List<List<Vector2>> oldElevations, List<Vector2> oldSafeTiles)
    {
        //Debug.Log("finding posible paths");
        originalElevations.Clear();
        originalSafeTiles.Clear();
        originalElevations = new List<List<Vector2>>(oldElevations);
        originalSafeTiles = new List<Vector2>(oldSafeTiles);
        furthestElevation = currentElevation;
        posibleTiles.Clear();
        posibleTilesPath.Clear();
        ResetPathfining();
        //starting at self spread uptward until you run out of movement
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
                    buildElevation(pos, false, false);
                }
            }
            if (i > Var.maxValue)
            {
                moveValue = i;
                Debug.Log("posible path pathfinding timed out");
            }
        }
    }
    //trances the path that the point in the movemnt map that is lowest in the global map took to get there
    public void findActualPath(Vector2 selfPos)
    {
        int killswitch = 0;
        GameObject tile = mapManager.GetTileAtHex(furthestPoint);
        GameObject border = tile.transform.Find("Border").gameObject;
        //border.GetComponent<SpriteRenderer>().color = Color.red;
        Vector2 currentLocaton = furthestPoint;
        actualPath.Clear();
        //Debug.Log(furthestPoint);
        while (currentLocaton != selfPos)
        {
            //Debug.Log("added " + currentLocaton + " actualPath");
            actualPath.Insert(0, currentLocaton);
            currentLocaton = posibleTilesPath[posibleTiles.IndexOf(currentLocaton)];
            killswitch++;
            if (killswitch > Var.maxValue)
            {
                Debug.Log(currentLocaton);
                currentLocaton = selfPos;
                Debug.Log("Finding move path timed out");
            }
        }
    }
    //adds a tile to the current elevation map, if spreading out from actiong figure uses the tiles move cost otherwize uses the move cost of the previus tile
    public void AddToElevation(Vector2 tilePos, GameObject tile, bool isRange = false, bool startAtTarget = true, int addedCost = 0)
    {
        int moveCost;
        //Debug.Log(isJump);
        if (isRange)
        {
            moveCost = 1;
        }
        else if (isJump || isFly)
        {
            moveCost = Mathf.Min(tiles[tilePos].MoveCost, mapManager.BaseMoveCost);

            ////moveCost = mapManager.BaseMoveCost;
            //if (!startAtTarget)
            //{
            //    //moveCost = Mathf.Min(tile.GetComponent<Tile>().MoveCost, mapManager.BaseMoveCost);
            //    moveCost = Mathf.Min(tiles[tilePos].MoveCost, mapManager.BaseMoveCost);

            //}
            //else
            //{
            //    moveCost = Mathf.Min(addedCost, mapManager.BaseMoveCost);
            //}
        }
        else
        {
            moveCost = tiles[tilePos].MoveCost;

            //if (!startAtTarget)
            //{
            //    moveCost = tiles[tilePos].MoveCost;
            //}
            //else
            //{
            //    moveCost = addedCost;
            //}
        }
        while (elevations.Count <= moveCost + currentElevation)
        {
            elevations.Add(new List<Vector2>());
        }
        elevations[moveCost + currentElevation].Add(tilePos);
        //tile.GetComponent<Tile>().MoveCostDisplay.DisplayVariable(moveCost + currentElevation + 1);
        //tile.GetComponent<Tile>().MoveCostDisplay.DisplayVariable(currentElevation + 1);

        if (!startAtTarget && moveCost + currentElevation > moveValue)
        {
            if (safeTiles.Contains(tilePos))
            {
                safeTiles.Remove(tilePos);
            }
            if (unsafeTiles.Contains(tilePos))
            {
                unsafeTiles.Remove(tilePos);
            }
            impassableTiles.Add(tilePos);
        }
    }
    //for a given tile checks all neibors and adds them to the elevation map if they arnt in it already
    public void buildElevation(Vector2 pos, bool range, bool startAtTarget)
    {
        Vector2 checktile = new Vector2();
        //GameObject originalTile = mapManager.GetTileAtHex(pos);
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
            if (checkedTiles.Contains(checktile))
            {
                
            }

                if (!checkedTiles.Contains(checktile))
            {
                GameObject tile = mapManager.GetTileAtHex(checktile);
                if (tile != null)
                {
                    GameObject entity = mapManager.GetEntityOnHex(checktile);
                    //if the tile is the tile the pathfinder is on
                    if (checktile == currentPos && startAtTarget)
                    {
                        pathFound = true;
                        safeTiles.Add(checktile);
                        checkedTiles.Add(checktile);
                        AddToElevation(checktile, tile, range, true , tiles[pos].MoveCost);
                        //endElevation = currentElevation + tiles[checktile].MoveCost;
                        endElevation = currentElevation + 1;
                        //Debug.Log("found path at " + tile + " at " + checktile);
                    }
                    else
                    {
                        //based on how you move determines if tile is safe,unsafe or impasible
                        GetTileType(checktile, tiles[pos].MoveCost, range, startAtTarget);
                    }
                    if (startAtTarget)
                    {
                        //tile.GetComponent<Tile>().MoveCostDisplay.DisplayVariable(currentElevation + 1);
                    }
                    //if checking posible paths 
                    if (!startAtTarget && !impassableTiles.Contains(checktile))
                    {
                        //if the tile is safe
                        if (originalSafeTiles.Contains(checktile))
                        {
                            //for each elevation lower than the current lowest one
                            for (int j = 0; j < furthestElevation; j++)
                            {
                                //if the tile is in that lower elevation make it the new closest point to goal
                                if (originalElevations[j].Contains(checktile))
                                {
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
    //detects special featurs of a tile and sorts it based on what it has
    public void GetTileType(Vector2 checktile, int addedCost, bool range = false, bool startAtTarget = false)
    {

        checkedTiles.Add(checktile);
        GameObject tile = mapManager.GetTileAtHex(checktile);
        if (tile != null)
        {
            GameObject entity = mapManager.GetEntityOnHex(checktile);
            if ((tile.GetComponent<Wall>() && !(currentFigure.GetComponent<PlayerControler>() && targetPos == checktile && (tile.GetComponent<Door>() || tile.GetComponent<Stair>())))  || (tile.GetComponent<Obstacle>() && !(range || isJump || isFly)) || (entity && entity.GetComponent<Figure>() && entity.GetComponent<Figure>().Team != currentTeam && !(range || isJump || isFly)))
            {
                impassableTiles.Add(checktile);
            }
            //if tile is unsafe
            else if ((tile.GetComponent<Obstacle>() && !isFly) || entity && entity.GetComponent<Figure>())
            {
                GameObject border = tile.transform.Find("Border").gameObject;
                //border.GetComponent<SpriteRenderer>().color = Color.yellow;
                unsafeTiles.Add(checktile);
                AddToElevation(checktile, tile, range, startAtTarget, addedCost);

            }
            //if tile is safe
            else
            {
                GameObject border = tile.transform.Find("Border").gameObject;
                //border.GetComponent<SpriteRenderer>().color = Color.blue;
                safeTiles.Add(checktile);
                AddToElevation(checktile, tile, range, startAtTarget, addedCost);
            }
        }
    }
    //moves a figure along the determined path
    public IEnumerator MoveAlongPath(GameObject figure, Vector2 figurePos)
    {
        Figure figureScript = figure.GetComponent<Figure>();
        figureScript.IsPreformingAnimation = true;
        float newFigureMoveDelay = figureMoveDelay;
        if (figureElevation >= 10)
        {
            newFigureMoveDelay = (figureMoveDelay * (10 + 20)) / (figureElevation + 20);
        }
        if (newFigureMoveDelay > figureMoveDelay)
        {
            Debug.Log(figure + " tried to move with delay " + figureMoveDelay + " delay");

        }
        //Debug.Log("waited for " + newFigureMoveDelay + " seconds");
        //Debug.Log(figureMoveDelay + "original delay");
        //Debug.Log(newFigureMoveDelay + "new delay");

        Vector2 hexPos = figurePos;
        Vector2 pos;
        for (int i = 0; i < actualPath.Count; i++)
        {
            hexPos = actualPath[i];
            if (isJump || isFly)
            {
                moveLeft -= mapManager.BaseMoveCost;
            }
            else
            {
                moveLeft -= tiles[actualPath[i]].MoveCost;
            }
            pos = mapManager.HexToRect(hexPos);
            figure.transform.position = new Vector3(pos.x, pos.y, figure.transform.position.z);
            figureScript.HexPos = hexPos;
            if (figure.name == "Player")
            {
                playerControler.CurrentTile = mapManager.GetTileAtHex(hexPos);
            }
            yield return StartCoroutine(figureScript.MoveOneSpace());
            yield return new WaitForSeconds(newFigureMoveDelay);
        }
        //doneMoving = true;
        figureScript.IsPreformingAnimation = false;
        yield break;
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
