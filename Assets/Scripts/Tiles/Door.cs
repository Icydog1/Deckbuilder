using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject wall;

    private GameObject roomNextTo = null;
    private Vector2 roomNextToCords;
    public Vector2 RoomNextToCords { get { return roomNextToCords; } }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddRoom(GameObject room, Vector2 roomCords)
    {
        if (roomNextTo)
        {
            Instantiate(wall, transform.position, transform.rotation);
        }
        else
        {
            roomNextTo = room;
            roomNextToCords = roomCords;
        }

    }
}
