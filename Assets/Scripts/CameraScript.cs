using System.Collections;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float scroll;
    private float scrollSpeed = 10;
    private float xSpeed, ySpeed, camSpeed = 3;
    private float maxZoom = 500, minZoom = 1f;
    private Camera cam;
    private DeckManager deckManager;
    public float standardHeight = 900, standardWidth = 1600, screenHeight, screenWidth, heightRatio, widthRatio, widthHeightRatio, standardWidthHeightRatio, zoom;
    public float FOVHeight, FOVWidth;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        standardWidthHeightRatio = standardWidth / standardHeight;
        zoom = cam.orthographicSize;
        StartCoroutine(resolutionChanged());

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
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
            
            StartCoroutine(resolutionChanged());
        }
        xSpeed = Input.GetAxis("Horizontal");
        ySpeed = Input.GetAxis("Vertical");
        transform.position = new Vector3(transform.position.x + xSpeed * zoom * Time.deltaTime * camSpeed, transform.position.y + ySpeed * cam.orthographicSize * Time.deltaTime * camSpeed, transform.position.z);
        if (screenHeight != Screen.height * zoom || screenWidth != Screen.width * zoom)
        {
            StartCoroutine(resolutionChanged());
        }
    }
    public IEnumerator resolutionChanged()
    {
        yield return new WaitForEndOfFrame();
        screenHeight = Screen.height * zoom;
        heightRatio = screenHeight / standardHeight;
        screenWidth = Screen.width * zoom;
        widthRatio = screenWidth / standardWidth;
        widthHeightRatio = widthRatio / heightRatio * zoom;
        deckManager.UpdateHand();
    }
}
