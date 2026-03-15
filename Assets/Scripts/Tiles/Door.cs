using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject wall;

    private GameObject roomNextTo = null;
    private Vector2 roomNextToCords;
    public Vector2 RoomNextToCords { get { return roomNextToCords; } }

    private MapManager mapManager;
    private RoomSpawner roomSpawner;

    private Vector2 oneToOneCords;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        roomSpawner = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>();

        oneToOneCords = mapManager.PosToOneToOne(transform.position);

        Vector2 checktile = oneToOneCords;
        for (int i = 0; i < 6; i++)
        {
            switch (i)
            {
                case 0: checktile = oneToOneCords + new Vector2(3, -3); break;
                case 1: checktile = oneToOneCords + new Vector2(6, 3); break;
                case 2: checktile = oneToOneCords + new Vector2(3, 6); break;
                case 3: checktile = oneToOneCords + new Vector2(-3, 3); break;
                case 4: checktile = oneToOneCords + new Vector2(-6, -3); break;
                case 5: checktile = oneToOneCords + new Vector2(-3, -6); break;
            }
            if (roomSpawner)
            GameObject checkedObject = mapManager.GetTileAtHex(checktile);
            if (checkedObject && checkedObject.GetComponent<Door>())
            {
                checkedObject.GetComponent<Door>().AddRoom(gameObject, oneToOneCords);
            }
        */
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
