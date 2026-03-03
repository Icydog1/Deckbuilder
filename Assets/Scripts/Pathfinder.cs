using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private MapManager mapManager;


    private List<List<Vector2>> elevations = new List<List<Vector2>>();
    private List<Vector2> currentHeight = new List<Vector2>();
    private List<Vector2> checkedTiles = new List<Vector2>();
    private bool pathFound;
    private int currentElevation;
    private Vector2 finalTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

        pathfindFrom(Vector3.zero, Vector3.left * -2 + Vector3.right * 2);
        //need to fix problem that corrdinte system hase multipule vales for eash corrdinate (to vecot 3s will point to same spot)
        //dpmt think it will casue problems just performese issuse(fatest rount wont be going in circles)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pathfindFrom(Vector3 startPos, Vector3 endPos)
    {

        finalTile = mapManager.HexToOneToOne(endPos);
        for (int i = 0; pathFound == false; i++)
        {
            currentElevation = i;
            List<Vector2> currentHeight = new List<Vector2>();
            elevations.Add(currentHeight);
            if (i == 0)
            {
                elevations[i].Add(mapManager.HexToOneToOne(startPos));
            }
            else
            {
                for (int j = 0; j < elevations[currentElevation - 1].Count; j++)
                {
                    buildElevation(elevations[currentElevation - 1][j]);
                }
            }

            if (i >= 100)
            {
                pathFound = true;
                Debug.Log("pathfinding timed out");
            }
        }
        elevations.Clear();
    }


    public void buildElevation(Vector2 pos, bool range = false, bool jump = false, bool fly = false)
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
                    Debug.Log("final elevation " + currentElevation);

                }
                //detectobsticals
                if (mapManager.GetObsticalAtHex(checktile, range && jump && fly, false, range && jump && fly).Count != 0)
                {
                    Debug.Log(mapManager.GetObsticalAtHex(checktile, range && jump && fly, false, range && jump && fly)[0]);
                    Debug.Log("obstical found at " + checktile);
                }
                else if (mapManager.GetObsticalAtHex(checktile, false, true, false, false).Count != 0)
                {
                    Debug.Log("enemy found at " + checktile);
                }
                else
                {
                    elevations[currentElevation].Add(checktile);
                }
                checkedTiles.Add(checktile);
            }
        }
    }
}
