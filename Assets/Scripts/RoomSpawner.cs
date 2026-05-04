using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.BoolParameter;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class RoomSpawner : MonoBehaviour
{
    private float tileWidth = 2, tileHeight, zLayer = 1000, roomSize = 17;
    private MapManager mapManager;
    private PlayerControler playerControler;

    [SerializeField]
    private List<GameObject> rooms;
    private List<GameObject> specialRooms = new List<GameObject>(), normalRooms = new List<GameObject>(), hardRooms = new List<GameObject>(), reallyHardRoomIcon = new List<GameObject>();
    private List<GameObject>[] roomTypes => new List<GameObject>[] { specialRooms, normalRooms, hardRooms, reallyHardRoomIcon };

    //[SerializeField]
    private float[] roomTypeWeights = { 0, 1, 0.7f, 0.3f };
    private float[] roomTypeProbabilities;

    private float[] initialRoomWeights = {0,1,1,1,1,0.3f}; //, 0.6f, 0.05f 
    private float[] roomProbabilities;
    private float realativeRotation = 0;
    private List<GameObject> existingRooms = new List<GameObject>();

    private List<Vector2Int> builtRooms = new List<Vector2Int>();
    public List<Vector2Int> BuiltRooms { get { return builtRooms; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        tileWidth = mapManager.TileWidth;
        tileHeight = mapManager.TileHeight;

        PrepareRooms();
        if (initialRoomWeights.Length != rooms.Count)
        {

            float[] baseRoomWeights = new float[rooms.Count];
            for(int i = 0; i < rooms.Count; i++)
            {
                if (initialRoomWeights.Length > i)
                {
                    baseRoomWeights[i] = initialRoomWeights[i];
                }
                else
                {
                    baseRoomWeights[i] = 1f;
                }
            }
            initialRoomWeights = baseRoomWeights;

        }
        BuildRoomProbabilities(initialRoomWeights);
        LevelManager.LevelCleared += ClearRooms;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void PrepareRooms()
    {
        foreach (GameObject room in rooms)
        {
            RoomScript roomScript = room.GetComponent<RoomScript>();
            if (roomScript.roomType == 0)
            {
                specialRooms.Add(room);
            }
            else if (roomScript.roomType == 1)
            {
                normalRooms.Add(room);
            }
            else if (roomScript.roomType == 2)
            {
                hardRooms.Add(room);
            }
            else if (roomScript.roomType == 3)
            {
                reallyHardRoomIcon.Add(room);
            }
            else
            {
                Debug.Log("No room type assigned");
            }


        }
        float sumOfRoomTypeWeights = 0;
        for (int i = 0; i < roomTypeWeights.Length; i++)
        {
            sumOfRoomTypeWeights += roomTypeWeights[i];
        }
        roomTypeProbabilities = new float[roomTypeWeights.Length];
        roomTypeProbabilities[0] = roomTypeWeights[0] / sumOfRoomTypeWeights;
        for (int i = 1; i < roomTypeWeights.Length; i++)
        {
            roomTypeProbabilities[i] = roomTypeWeights[i] / sumOfRoomTypeWeights + roomTypeProbabilities[i - 1];
        }
    }


    public void ClearRooms(LevelManager levelManager)
    {
        foreach (GameObject room in existingRooms)
        {
            Destroy(room);
        }
        existingRooms.Clear();
        builtRooms.Clear();
    }
    private void BuildRoomProbabilities(float[] roomWeights)
    {
        if (roomWeights.Length != rooms.Count)
        {
            Debug.Log("Unequal Rooms And Room Probabilities");
        }
        float sumOfRoomWeights = 0;
        for (int i = 0; i < roomWeights.Length; i++)
        {
            sumOfRoomWeights += roomWeights[i];
        }
        roomProbabilities = new float[roomWeights.Length];
        roomProbabilities[0] = roomWeights[0] / sumOfRoomWeights;
        for (int i = 1; i < roomWeights.Length; i++)
        {
            roomProbabilities[i] = roomWeights[i] / sumOfRoomWeights + roomProbabilities[i - 1];
        }
    }
    public void SpawnRoomsNextToDoor(GameObject door, Vector2 currentRoom)
    {
        //Debug.Log(mapManager.OneToOneToPos(doorOneToOne - currentRoom));
        //Debug.Log(realativeRotation);
        Vector2 doorOneToOne = mapManager.PosToOneToOne(door.transform.position);
        realativeRotation = Vector2.SignedAngle(new Vector2(1, 0), mapManager.OneToOneToPos(doorOneToOne - currentRoom));
        realativeRotation = Mathf.RoundToInt(realativeRotation / 60) * 60;
        Vector2 newRoomPos = 2 * doorOneToOne - currentRoom;
        Vector2Int roundednewRoomPos = Vector2Int.RoundToInt(newRoomPos);
        if (!builtRooms.Contains(roundednewRoomPos))
        {
            SpawnRandomRoom(roundednewRoomPos, door.GetComponent<Door>().RoomType);
        }
        
    }

    public void SpawnStartingRoom()
    {
        realativeRotation = 0;
        SpawnRoom(Vector2Int.zero, rooms[0]);
    }
    public int GetRandomRoomType()
    {
        float randomRoomTypeNumber = Random.Range(0, 1f);
        for (int i = 0; i < roomTypeProbabilities.Length; i++)
        {
            if (randomRoomTypeNumber < roomTypeProbabilities[i])
            {
                return i;
            }
        }
        return 0;

    }

    private void SpawnRandomRoom(Vector2Int oneToOnePos, int roomtype = -1)
    {
        List<GameObject> roomPool = rooms;
        if (roomtype == -1)
        {
            float randomRoomTypeNumber = Random.Range(0, 1f);
            for (int i = 0; i < roomTypeProbabilities.Length; i++)
            {
                if (randomRoomTypeNumber < roomTypeProbabilities[i])
                {
                    roomPool = roomTypes[i];
                    break;
                }
            }
        }
        else
        {
            roomPool = roomTypes[roomtype];
        }
        SpawnRoom(oneToOnePos, roomPool[Random.Range(0, roomPool.Count)]);

        /*
        float randomNumber = Random.Range(0, 1f);

        for (int i = 0; i < roomPool.Count; i++)
        {
            if (randomNumber < roomProbabilities[i])
            {
                SpawnRoom(oneToOnePos, roomPool[i]);
                break;
            }
        }
        */
    }

    private void SpawnRoom(Vector2Int oneToOnePos, GameObject room)
    {
        //Debug.Log(oneToOnePos + "roompos");
        builtRooms.Add(oneToOnePos);
        Vector2 pos = mapManager.OneToOneToPos(oneToOnePos);
        int rotation = room.GetComponent<RoomScript>().GetSpawnRotation();
        transform.localEulerAngles = new Vector3(0, 0, realativeRotation + rotation);
        existingRooms.Add(Instantiate(room, new Vector3(pos.x, pos.y, zLayer), transform.rotation));

        playerControler.ShowMoveCostDisplay();

    }
}
