
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public MapManager mapManager;
    public MouseManager mouseManager;


    public Camera cam;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();

    }

    // Update is called once per frame
    void Update()
    {


    }




}
