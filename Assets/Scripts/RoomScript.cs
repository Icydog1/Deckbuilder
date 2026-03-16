using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    private MapManager mapManager;
    private RoomSpawner roomSpawner;

    private Vector2Int oneToOneCords;
    private int roomSize = 6;

    [SerializeField]
    private bool isLeftEntrance;
    [SerializeField]
    private bool isTopLeftEntrance;
    [SerializeField]
    private bool isTopRightEntrance;
    [SerializeField]
    private bool isRightEntrance;
    [SerializeField]
    private bool isBottomRightEntrance;
    [SerializeField]
    private bool isBottomLeftEntrance;

    private List<int> startDirections = new List<int>();
    //private List<GameObject> doors = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();

        oneToOneCords = Vector2Int.RoundToInt(mapManager.PosToOneToOne(transform.position));
    }
    void Start()
    {

        Vector2Int checktile = oneToOneCords;
        Vector2Int checkRoom = oneToOneCords;
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = oneToOneCords + new Vector2Int(roomSize/2, -roomSize/2);
                    checkRoom = oneToOneCords + new Vector2Int(roomSize, -roomSize); break;
                case 1: checktile = oneToOneCords + new Vector2Int(roomSize, roomSize/2);
                    checkRoom = oneToOneCords + new Vector2Int(roomSize * 2, roomSize); break;
                case 2: checktile = oneToOneCords + new Vector2Int(roomSize/2, roomSize);
                    checkRoom = oneToOneCords + new Vector2Int(roomSize, roomSize * 2); break;
                case 3: checktile = oneToOneCords + new Vector2Int(-roomSize/2, roomSize/2);
                    checkRoom = oneToOneCords + new Vector2Int(-roomSize, roomSize); break;
                case 4: checktile = oneToOneCords + new Vector2Int(-roomSize, -roomSize/2);
                    checkRoom = oneToOneCords + new Vector2Int(-roomSize * 2, -roomSize); break;
                case 5: checktile = oneToOneCords + new Vector2Int(-roomSize / 2, -roomSize);
                    checkRoom = oneToOneCords + new Vector2Int(-roomSize, -roomSize * 2); break;
            }
            GameObject checkedObject = mapManager.GetTileAtHex(checktile);
            if (checkedObject && checkedObject.GetComponent<Door>())
            {
                checkedObject.GetComponent<Door>().AddRoom(gameObject, oneToOneCords);
                //Debug.Log(checktile + "Door Cords");
                //Debug.Log(checkRoom + "Other Room Cords");
                if (roomSpawner.BuiltRooms.Contains(checkRoom))
                {
                    //Debug.Log("other room exists");
                    checkedObject.GetComponent<Door>().AddRoom(gameObject, checkRoom);
                }
            }
            //doors.Add(checkedObject);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetSpawnRotation()
    {
        if (isLeftEntrance)
        {
            startDirections.Add(0);
        }
        if (isTopLeftEntrance)
        {
            startDirections.Add(60);
        }
        if (isTopRightEntrance)
        {
            startDirections.Add(120);
        }
        if (isRightEntrance)
        {
            startDirections.Add(180);
        }
        if (isBottomRightEntrance)
        {
            startDirections.Add(240);
        }
        if (isBottomLeftEntrance)
        {
            startDirections.Add(300);
        }
        if (startDirections.Count == 0)
        {
            Debug.Log("No Start Direction");
            startDirections.Add(0);
        }
        return startDirections[Random.Range(0, startDirections.Count)];
    }
}
