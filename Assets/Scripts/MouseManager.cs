using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class MouseManager : MonoBehaviour
{
    private bool mouseDown, shortClick, dragableClicked;
    private float bottomPlayLine = 0.3f, topPlayLine = 0.6f;
    private Vector2 mousePos, worldMousePos;
    public GameObject selectedObject, clickedObject;
    private float selectedHeight = -Mathf.Infinity;
    private List<GameObject> mouseOver = new List<GameObject>();
    private List<GameObject> mouseOverList = new List<GameObject>();
    private List<float> mouseOverHeights = new List<float>();
    private MapManager mapManager;
    private GameManager gameManager;
    private PlayerControler playerControler;
    private CameraScript cameraScript;
    private DeckManager deckManager;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();

    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        worldMousePos = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0)).x, Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0)).y);

        if (Input.GetMouseButtonDown(0))
        {
            mouseDown = true;
            MouseClicked();
        }
        if (Input.GetMouseButtonUp(0))
        {
            mouseDown = false;
            MouseReleased();
        }
        if (clickedObject != null && clickedObject.GetComponent<Dragable>() != null)
        {
            //Debug.Log(worldMousePos);
            clickedObject.transform.position = new Vector3(worldMousePos.x, worldMousePos.y, clickedObject.transform.position.z);
        }


    }
    public void MouseOnObject(float newheight, GameObject newObject)
    {

        if (mouseOver.Count == 0)
        {
            //Debug.Log(newObject + "on");
        }
        mouseOver.Add(newObject);
        mouseOverHeights.Add(newheight);
        foreach (GameObject item in mouseOver)
        {
            if (selectedHeight < mouseOverHeights[mouseOver.IndexOf(item)])
            {
                selectedObject = item;
                selectedHeight = mouseOverHeights[mouseOver.IndexOf(item)];
            }
        }
        if (selectedObject.GetComponent<Tile>())
        {
            selectedObject.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
    public void MouseOffObject(float newheight, GameObject newObject)
    {
        mouseOver.Remove(newObject);
        mouseOverHeights.Remove(newheight);
        if (newObject.GetComponent<Tile>())
        {
            newObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (mouseOver.Count == 0)
        {
            //Debug.Log(selectedObject + "off");
            selectedObject = null;
            selectedHeight = -Mathf.Infinity;
        }
        else
        {
            foreach (GameObject item in mouseOver)
            {
                if (selectedHeight < mouseOverHeights[mouseOver.IndexOf(item)])
                {
                    selectedObject = item;
                    selectedHeight = mouseOverHeights[mouseOver.IndexOf(item)];
                }
            }
        }
    }
    public GameObject getObjectAtPoint(Vector2 point)
    {
        return Physics2D.OverlapPoint(point).gameObject;
        /*
        Debug.Log(worldMousePos);
        Debug.Log(Physics2D.OverlapPointAll(worldMousePos));
        mouseOverList.Clear();
        foreach (Collider2D collider in Physics2D.OverlapPointAll(worldMousePos))
        {
            mouseOverList.Add(collider.gameObject);
            Debug.Log(collider);
        }
        selectedObject = null;
        if (mouseOverList.Count != 0)
        {
            foreach (GameObject item in mouseOverList)
            {
                if (item.GetComponent<Clickable>())
                {
                    selectedObject = item;
                }
            }
            Debug.Log(selectedObject);

        }
        */

    }
    public void MouseClicked()
    {

        clickedObject = selectedObject;
        if (clickedObject.GetComponent<Dragable>() != null && !dragableClicked && playerControler.cardPlayed == false)
        {
            dragableClicked = true;
            StartCoroutine(ShortFirstClick());
        }

    }
    private IEnumerator ShortFirstClick()
    {
        //Debug.Log("test");
        shortClick = true;
        yield return new WaitForSeconds(0.25f);
        shortClick = false;
    }
    public void MouseReleased()
    {
        if (dragableClicked && !shortClick)
        {

            if (clickedObject.GetComponent<Playable>() != null)
            {
                if (mousePos.y > topPlayLine * Screen.height)
                {
                    Debug.Log(clickedObject + "top was played");
                    clickedObject.GetComponent<Card>().AttemptToPlayTop();
                }
                else if (mousePos.y > bottomPlayLine * Screen.height)
                {
                    Debug.Log(clickedObject + "bottom was played");
                    clickedObject.GetComponent<Card>().AttemptToPlayBottom();
                }
                else
                {
                    deckManager.UpdateHand();
                }
            }
            dragableClicked = false;
        }


        if (clickedObject.GetComponent<Tile>())
        {
            playerControler.clickedTile = clickedObject;
        }
        if (clickedObject.GetComponent<UIButton>())
        {
            clickedObject.GetComponent<UIButton>().Activate();
        }
        if (!dragableClicked)
        {
            clickedObject = null;
        }
    }
}

