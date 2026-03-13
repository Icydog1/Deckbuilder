using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    private MapManager mapManager;

    private Vector2 oneToOneCords;
    private List<GameObject> doors = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

        oneToOneCords = mapManager.PosToOneToOne(transform.position);

        Vector2 checktile = oneToOneCords;
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = oneToOneCords + new Vector2(3,-3); break;
                case 1: checktile = oneToOneCords + new Vector2(6, 3); break;
                case 2: checktile = oneToOneCords + new Vector2(3, 6); break;
                case 3: checktile = oneToOneCords + new Vector2(3, -3); break;
                case 4: checktile = oneToOneCords + new Vector2(-6, -3); break;
                case 5: checktile = oneToOneCords + new Vector2(-3, -6); break;
            }
            GameObject checkedObject = mapManager.GetTileAtHex(checktile);
            if (checkedObject.GetComponent<Door>())
            {
                checkedObject.GetComponent<Door>().AddRoom();
            }
            doors.Add(checkedObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
