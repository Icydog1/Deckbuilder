using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    private float scroll;
    private float scrollSpeed = 10;
    private float xSpeed, ySpeed, camSpeed = 3;
    private float maxZoom = 500, minZoom = 1f;
    private Camera cam;
    public float standardHeight = 900, standardWidth = 1600, screenHeight, screenWidth, heightRatio, widthRatio, zoom;
    public float FOVHeight, FOVWidth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        //resolutionChanged();

    }

    // Update is called once per frame
    void Update()
    {

        scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * scrollSpeed;
        if (cam.orthographicSize <= minZoom)
        {
            cam.orthographicSize = minZoom;
        }
        if (cam.orthographicSize >= maxZoom)
        {
            cam.orthographicSize = maxZoom;
        }
        zoom = cam.orthographicSize;
        xSpeed = Input.GetAxis("Horizontal");
        ySpeed = Input.GetAxis("Vertical");
        transform.position = new Vector3(transform.position.x + xSpeed * zoom * Time.deltaTime * camSpeed, transform.position.y + ySpeed * cam.orthographicSize * Time.deltaTime * camSpeed, transform.position.z);

        screenHeight = Screen.height * zoom;
        heightRatio = screenHeight / standardHeight;
        screenWidth = Screen.width * zoom;
        widthRatio = screenWidth / standardWidth;
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            //resolutionChanged();
        }
    }
    public void resolutionChanged()
    {
        screenHeight = Screen.height;
        heightRatio = screenHeight / standardHeight;
        screenWidth = Screen.width;
        widthRatio = screenWidth / standardWidth;
    }
}
