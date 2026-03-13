using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject wall;

    private int roomsNextTo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddRoom()
    {
        roomsNextTo++;
        if (roomsNextTo >= 2)
        {
            Instantiate(wall, transform.position, transform.rotation);
        }
    }
}
