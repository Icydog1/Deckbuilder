using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    private MapManager mapManager;
    private RoomSpawner roomSpawner;

    private Vector2 oneToOneCords;
    private int roomSize = 6;
    //private List<GameObject> doors = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();

        oneToOneCords = mapManager.PosToOneToOne(transform.position);

        Vector2 checktile = oneToOneCords;
        Vector2 checkRoom = oneToOneCords;
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = oneToOneCords + new Vector2(roomSize/2, -roomSize/2);
                    checkRoom = oneToOneCords + new Vector2(roomSize, -roomSize); break;
                case 1: checktile = oneToOneCords + new Vector2(roomSize, roomSize/2);
                    checkRoom = oneToOneCords + new Vector2(roomSize * 2, roomSize); break;
                case 2: checktile = oneToOneCords + new Vector2(roomSize/2, roomSize);
                    checkRoom = oneToOneCords + new Vector2(roomSize, roomSize * 2); break;
                case 3: checktile = oneToOneCords + new Vector2(-roomSize/2, roomSize/2);
                    checkRoom = oneToOneCords + new Vector2(-roomSize, roomSize); break;
                case 4: checktile = oneToOneCords + new Vector2(-roomSize, -roomSize/2);
                    checkRoom = oneToOneCords + new Vector2(-roomSize * 2, -roomSize); break;
                case 5: checktile = oneToOneCords + new Vector2(-roomSize / 2, -roomSize);
                    checkRoom = oneToOneCords + new Vector2(-roomSize, -roomSize * 2); break;
            }
            GameObject checkedObject = mapManager.GetTileAtHex(checktile);
            if (checkedObject && checkedObject.GetComponent<Door>())
            {
                checkedObject.GetComponent<Door>().AddRoom(gameObject, oneToOneCords);
                Debug.Log(checktile + "Door Cords");
                Debug.Log(checkRoom + "Other Room Cords");
                if (roomSpawner.BuiltRooms.Contains(checkRoom))
                {
                    checkedObject.GetComponent<Door>().AddRoom(null, checkRoom);
                }
            }
            //doors.Add(checkedObject);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
