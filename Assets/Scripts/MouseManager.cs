using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float selectedCardHeightIncrease = 0.25f;



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
        if (selectedObject != null && selectedObject.GetComponent<Card>() != null && selectedObject.transform.localScale == Vector3.one)
        {
            selectedObject.transform.localScale = new Vector3 (2,2,1);
            selectedObject.transform.position = selectedObject.transform.position + new Vector3(0, selectedCardHeightIncrease, 0) * cameraScript.zoom;
        }
        if (clickedObject != null && clickedObject.GetComponent<Dragable>() != null && clickedObject.GetComponent<Card>() != null && playerControler.cardPlayed == false)
        {
            //Debug.Log(worldMousePos);
            clickedObject.transform.position = new Vector3(worldMousePos.x, worldMousePos.y, clickedObject.transform.position.z);
            if (mousePos.y > topPlayLine * Screen.height)
            {
                clickedObject.GetComponent<Card>().topGlow.SetActive(true);
                clickedObject.GetComponent<Card>().bottomGlow.SetActive(false);
            }
            else if (mousePos.y > bottomPlayLine * Screen.height)
            {
                clickedObject.GetComponent<Card>().bottomGlow.SetActive(true);
                clickedObject.GetComponent<Card>().topGlow.SetActive(false);

            }
            else
            {
                clickedObject.GetComponent<Card>().topGlow.SetActive(false);
                clickedObject.GetComponent<Card>().bottomGlow.SetActive(false);
            }
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
        if (selectedObject.GetComponent<Selectable>())
        {
            GameObject border = selectedObject.transform.Find("Border").gameObject;
            border.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }
    public void MouseOffObject(float newheight, GameObject newObject)
    {
        mouseOver.Remove(newObject);
        mouseOverHeights.Remove(newheight);
        if (selectedObject.GetComponent<Selectable>())
        {
            GameObject border = selectedObject.transform.Find("Border").gameObject;
            
            border.GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (newObject.GetComponent<Card>())
        {
            newObject.transform.localScale = new Vector3(1, 1, 1);
            newObject.transform.position = newObject.transform.position - new Vector3(0, selectedCardHeightIncrease, 0) * cameraScript.zoom;
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
        if (clickedObject != null && clickedObject.GetComponent<Dragable>() != null && !dragableClicked && playerControler.cardPlayed == false)
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
        if (clickedObject != null)
        {
            if (clickedObject.GetComponent<Tile>())
            {
                playerControler.TileClicked(clickedObject);
            }
            if (clickedObject.GetComponent<UIButton>())
            {
                clickedObject.GetComponent<UIButton>().Activate();
            }
            if (clickedObject.GetComponent<Enemy>())
            {
                playerControler.EnemyClicked(clickedObject);
            }
            if (!dragableClicked)
            {
                clickedObject = null;
            }
        }

    }
}

