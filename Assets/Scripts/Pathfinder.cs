using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private List<List<Vector3>> elevations = new List<List<Vector3>>();
    private List<Vector3> currentHeight = new List<Vector3>();
    private List<Vector3> checkedTiles = new List<Vector3>();
    private bool pathFound;
    private int currentElevation;
    private Vector3 finalTile;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathfindFrom(Vector3.zero, Vector3.up * 2 + Vector3.right * 2);
        //need to fix problem that corrdinte system hase multipule vales for eash corrdinate (to vecot 3s will point to same spot)
        //dpmt think it will casue problems just performese issuse(fatest rount wont be going in circles)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void pathfindFrom(Vector3 startPos, Vector3 endPos)
    {
        finalTile = endPos;
        for (int i = 0; pathFound == false; i++)
        {
            currentElevation = i;
            List<Vector3> currentHeight = new List<Vector3>();
            elevations.Add(currentHeight);
            if (i == 0)
            {
                elevations[i].Add(startPos);
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


    public void buildElevation(Vector3 pos)
    {
        Vector3 checktile = new Vector3();
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = pos + Vector3.up; break ;
                case 1: checktile = pos + Vector3.down; break;
                case 2: checktile = pos + Vector3.left; break;
                case 3: checktile = pos + Vector3.right; break;
                case 4: checktile = pos + Vector3.forward; break;
                case 5: checktile = pos + Vector3.back; break;
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
                elevations[currentElevation].Add(checktile);
                checkedTiles.Add(checktile);

            }
        }

    }
}
