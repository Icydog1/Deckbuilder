using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject wall;

    private GameObject roomNextTo = null;
    private Vector2 roomNextToCords;
    public Vector2 RoomNextToCords { get { return roomNextToCords; } }

    public int roomType;
    public int RoomType { get { return roomType; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        roomType = GameObject.Find("RoomSpawner").GetComponent<RoomSpawner>().GetRandomRoomType();
        GameObject roomPreview = transform.Find("TileUI").Find("RoomPreview").gameObject;
        VariableDisplayer previewImage = roomPreview.GetComponent<VariableDisplayer>();
        //previewImage.
        string displayedText = string.Empty;
        switch (roomType)
        {
            case 0: displayedText = "<sprite name=SpecialRoomIcon>"; break;
            case 1: displayedText = "<sprite name=NormalRoomIcon>"; break;
            case 2: displayedText = "<sprite name=HardRoomIcon>"; break;
            case 3: displayedText = "<sprite name=ReallyHardRoomIcon>"; break;

        }
        previewImage.DisplayString(displayedText);
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
