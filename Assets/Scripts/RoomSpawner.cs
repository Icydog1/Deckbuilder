using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private float tileWidth = 2, tileHeight, zLayer = 1000, roomSize = 13;
    private MapManager mapManager;
    [SerializeField]
    private GameObject[] rooms;
    [SerializeField]
    private float[] initialRoomWeights = { 0.9f}; //, 0.6f, 0.05f 
    private float[] roomProbabilities;
    private float realativeRotation = 0;
    public List<Vector2Int> builtRooms = new List<Vector2Int>();
    public List<Vector2Int> BuiltRooms { get { return builtRooms; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        tileWidth = mapManager.TileWidth;
        tileHeight = mapManager.TileHeight;
        BuildRoomProbabilities(initialRoomWeights);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void BuildRoomProbabilities(float[] roomWeights)
    {
        if (roomWeights.Length != rooms.Length)
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
    public void SpawnRoomsNextToDoor(Vector2 doorOneToOne, Vector2 currentRoom)
    {
        //Debug.Log(mapManager.OneToOneToPos(doorOneToOne - currentRoom));
        //Debug.Log(realativeRotation);
        realativeRotation = Vector2.SignedAngle(new Vector2(1, 0), mapManager.OneToOneToPos(doorOneToOne - currentRoom));
        realativeRotation = Mathf.RoundToInt(realativeRotation / 60) * 60;
        Vector2 newRoomPos = 2 * doorOneToOne - currentRoom;
        Vector2Int roundednewRoomPos = Vector2Int.RoundToInt(newRoomPos);
        if (!builtRooms.Contains(roundednewRoomPos))
        {
            SpawnRandomRoom(roundednewRoomPos);
        }
        
    }

    public void SpawnStartingRoom()
    {
        SpawnRoom(Vector2Int.zero, rooms[0]);
    }
    private void SpawnRandomRoom(Vector2Int oneToOnePos)
    {
        float randomNumber = Random.Range(0, 1f);
        for (int i = 0; i < rooms.Length; i++)
        {
            if (randomNumber < roomProbabilities[i])
            {
                SpawnRoom(oneToOnePos, rooms[i]);
                break;
            }
        }
    }

    private void SpawnRoom(Vector2Int oneToOnePos, GameObject room)
    {
        //Debug.Log(oneToOnePos + "roompos");
        builtRooms.Add(oneToOnePos);
        Vector2 pos = mapManager.OneToOneToPos(oneToOnePos);
        int rotation = room.GetComponent<RoomScript>().GetSpawnRotation();
        transform.localEulerAngles = new Vector3(0, 0, realativeRotation + rotation);
        Instantiate(room, new Vector3(pos.x, pos.y, zLayer), transform.rotation);
    }
}
