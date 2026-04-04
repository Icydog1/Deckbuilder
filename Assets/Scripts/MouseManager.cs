using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MouseManager : MonoBehaviour
{
    private bool mouseDown, shortClick, dragableClicked;
    private float bottomPlayLine = 0.3f, topPlayLine = 0.6f;
    private Vector2 mousePos, worldMousePos;
    public GameObject selectedObject, clickedObject;
    public GameObject SelectedObject { get { return selectedObject; } }
    public GameObject ClickedObject { get { return clickedObject; } }


    private float selectedHeight = -Mathf.Infinity;
    private List<GameObject> mouseOver = new List<GameObject>();
    private List<GameObject> mouseOverList = new List<GameObject>();
    private List<float> mouseOverHeights = new List<float>();
    private MapManager mapManager;
    private GameManager gameManager;
    private PlayerControler playerControler;
    private CameraScript cameraScript;
    private DeckManager deckManager;
    private RewardManager rewardManager;
    public float selectedCardHeightIncrease = 0.25f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();


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
        if (selectedObject && selectedObject.GetComponent<Card>())
        {
            deckManager.SelectCard(selectedObject);
        }
        if (clickedObject && clickedObject.GetComponent<Dragable>() && playerControler.CanPlayCards == true && clickedObject.GetComponent<Card>() != null)
        {
            //Debug.Log(worldMousePos);
            clickedObject.transform.position = new Vector3(worldMousePos.x, worldMousePos.y, clickedObject.transform.position.z);
            if (mousePos.y > topPlayLine * Screen.height)
            {
                clickedObject.GetComponent<Card>().TopGlow.SetActive(true);
                clickedObject.GetComponent<Card>().BottomGlow.SetActive(false);
            }
            else if (mousePos.y > bottomPlayLine * Screen.height)
            {
                clickedObject.GetComponent<Card>().BottomGlow.SetActive(true);
                clickedObject.GetComponent<Card>().TopGlow.SetActive(false);

            }
            else
            {
                clickedObject.GetComponent<Card>().TopGlow.SetActive(false);
                clickedObject.GetComponent<Card>().BottomGlow.SetActive(false);
            }
        }


    }
    public void MouseOnObject(GameObject newObject)
    {
        float newheight = transform.position.z;

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
            if (selectedObject.GetComponent<Selectable>().IsUI || selectedObject.GetComponent<UIButton>())
            {
                GameObject border = selectedObject.transform.Find("Border").gameObject;
                border.GetComponent<Image>().color = Color.green;
            }
            else
            {
                GameObject border = selectedObject.transform.Find("Border").gameObject;
                border.GetComponent<SpriteRenderer>().color = Color.green;
            }

        }
    }
    public void MouseOffObject(GameObject newObject)
    {
        if (mouseOver.Contains(newObject))
        {
            if (selectedObject.GetComponent<Selectable>())
            {
                if (selectedObject.GetComponent<Selectable>().IsUI || selectedObject.GetComponent<UIButton>())
                {
                    GameObject border = selectedObject.transform.Find("Border").gameObject;
                    border.GetComponent<Image>().color = Color.black;
                }
                else
                {
                    GameObject border = selectedObject.transform.Find("Border").gameObject;
                    border.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            if (newObject.GetComponent<Card>())
            {
                deckManager.DeSelectCard(newObject);
            }
            mouseOver.Remove(newObject);
            float newheight = transform.position.z;
            mouseOverHeights.Remove(newheight);
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
    }
    public void MouseClicked()
    {

        clickedObject = selectedObject;
        if (clickedObject && clickedObject.GetComponent<UIButton>())
        {
            if (clickedObject.GetComponent<UIButton>())
            {
                GameObject image = clickedObject.transform.Find("Image").gameObject;
                image.GetComponent<Image>().color = clickedObject.GetComponent<UIButton>().ClickedColor;
            }
        }
        if (clickedObject != null && clickedObject.GetComponent<Dragable>() != null && !dragableClicked && playerControler.CanPlayCards == true)
        {
            dragableClicked = true;
            deckManager.hand.transform.SetAsLastSibling();
            clickedObject.transform.SetAsLastSibling();
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
        if (clickedObject && clickedObject.GetComponent<UIButton>())
        {
            if (clickedObject.GetComponent<UIButton>())
            {
                GameObject image = clickedObject.transform.Find("Image").gameObject;
                image.GetComponent<Image>().color = clickedObject.GetComponent<UIButton>().BaseColor;
            }
        }
        if (dragableClicked && !shortClick)
        {

            if (clickedObject.GetComponent<Card>() != null)
            {
                if (mousePos.y > topPlayLine * Screen.height)
                {
                    //Debug.Log(clickedObject + "top was played");
                    clickedObject.GetComponent<Card>().AttemptToPlayTop();
                }
                else if (mousePos.y > bottomPlayLine * Screen.height)
                {
                    //Debug.Log(clickedObject + "bottom was played");
                    clickedObject.GetComponent<Card>().AttemptToPlayBottom();
                }
                else
                {
                    deckManager.UpdateHand();
                }
                deckManager.hand.transform.SetAsFirstSibling();
            }
            dragableClicked = false;
        }
        if (clickedObject != null && clickedObject == selectedObject)
        {
            if (clickedObject.GetComponent<Tile>())
            {
                playerControler.TileClicked(clickedObject);
            }
            if (clickedObject.GetComponent<UIButton>())
            {
                clickedObject.GetComponent<UIButton>().Activate();
            }
            if (clickedObject.GetComponent<Figure>())
            {
                playerControler.FigureClicked(clickedObject);
            }
            if (clickedObject.GetComponent<IsReward>())
            {
                rewardManager.RewardSelected(clickedObject);
            }
        }
        if (!dragableClicked)
        {
            clickedObject = null;
        }

    }
}

