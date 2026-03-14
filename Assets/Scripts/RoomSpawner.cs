using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    private float tileWidth = 2, tileHeight, zLayer = 1000, roomSize = 13;
    private MapManager mapManager;
    [SerializeField]
    private GameObject[] rooms;
    private float[] initialRoomWeights = { 0.9f}; //, 0.6f, 0.05f 
    private float[] roomProbabilities;
    private List<Vector2> builtRooms = new List<Vector2>();
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
        Debug.Log(doorOneToOne + "Doorpos");
        Debug.Log(currentRoom + "OldRoomPos");
        float realativeRotation = Vector2.Angle(doorOneToOne - currentRoom, Vector2.right);
        transform.localEulerAngles = new Vector3(0, 0, realativeRotation);
        Vector2 newRoomPos = 2 * doorOneToOne - currentRoom;
        if (!builtRooms.Contains(newRoomPos))
        {
            spawnRandomRoom(newRoomPos);
        }
        
    }

    private void spawnRandomRoom(Vector2 oneToOnePos)
    {
        float randomNumber = Random.Range(0, 1f);
        for (int i = 0; i < rooms.Length; i++)
        {
            if (randomNumber < roomProbabilities[i])
            {
                spawnRoom(oneToOnePos, rooms[i]);
                break;
            }
        }
    }

    private void spawnRoom(Vector2 oneToOnePos, GameObject room)
    {
        Debug.Log(oneToOnePos + "roompos");
        builtRooms.Add(oneToOnePos);
        Vector2 pos = mapManager.OneToOneToPos(oneToOnePos);
        Instantiate(room, new Vector3(pos.x, pos.y, zLayer), transform.rotation);
    }
}
