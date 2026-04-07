using Unity.VisualScripting;
using UnityEngine;

public class Wall : MonoBehaviour
{
    [SerializeField]
    private GameObject openDoor;


    //private MapManager mapManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();

        gameObject.layer = 9;
        if (Physics2D.OverlapPoint(transform.position, 64))
        {
            GameObject otherTile = Physics2D.OverlapPoint(transform.position, 64).gameObject;
            if (!otherTile.GetComponent<Door>())
            {
                //Debug.Log(mapManager.PosToOneToOne(transform.position));
                Destroy(gameObject);
            }
            else if (!gameObject.GetComponent<Door>())
            {
                Destroy(otherTile);
                gameObject.layer = 6;
            }
            else
            {
                //Debug.Log(mapManager.PosToOneToOne(transform.position));
                Instantiate(openDoor, transform.position, transform.rotation);
                Destroy(otherTile);
                Destroy(gameObject);
            }
        }
        else
        {
            gameObject.layer = 6;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
