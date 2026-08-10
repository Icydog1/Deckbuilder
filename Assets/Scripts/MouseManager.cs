using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//runs the mouse and what is it seleceting
public class MouseManager : MonoBehaviour
{
    private bool mouseDown, shortClick, dragableClicked;
    public bool MouseDown { get { return mouseDown; } }


    private float bottomPlayLine = 0.3f, topPlayLine = 0.6f;
    private Vector2 mousePos, worldMousePos;
    private GameObject selectedObject, clickedObject;
    public GameObject SelectedObject { get { return selectedObject; } set { selectedObject = value; } }
    public GameObject ClickedObject { get { return clickedObject; } set { clickedObject = value; } }
    private GameObject hoveredObject;
    private IEnumerator heldButtonRoutine;

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
    private TooltipManager tooltipManager;
    private ActionManager actionManager;


    private float selectedCardHeightIncrease = 0.25f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mapManager = RefrenceStorage.mapManager;
        gameManager = RefrenceStorage.gameManager;
        playerControler = RefrenceStorage.playerControler;
        cameraScript = RefrenceStorage.cameraScript;
        deckManager = RefrenceStorage.deckManager;
        rewardManager = RefrenceStorage.rewardManager;
        tooltipManager = RefrenceStorage.tooltipManager;
        actionManager = RefrenceStorage.actionManager;

    }

    // Update is called once per frame
    void Update()
    {
        //find mouse position
        mousePos = Input.mousePosition;
        worldMousePos = new Vector2(Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0)).x, Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0)).y);

        //detect if the mouse is clicked
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
        //if there is a clicked card
        if (clickedObject && clickedObject.GetComponent<Dragable>() && playerControler.CanPlayCards == true && clickedObject.GetComponent<Card>() != null)
        {
            //move clicked card to mouse pos
            clickedObject.transform.position = new Vector3(worldMousePos.x, worldMousePos.y, clickedObject.transform.position.z);
            //update card glow
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
    //make the mouse count as being over a object
    public void MouseOnObject(GameObject newObject)
    {
        //do varius thing to select the object
        if (newObject == selectedObject)
        {
            Debug.Log("same object");
        }
        float newheight = transform.position.z;
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
        if (selectedObject.GetComponent<Card>())
        {
            deckManager.SelectCard(selectedObject);
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
        if (hoveredObject != selectedObject && selectedObject.GetComponent<Hoverable>())
        {
            hoveredObject = selectedObject;
            StartCoroutine(tooltipManager.StartHoveringOver(hoveredObject));
        }
        if (playerControler.PlanningMove && selectedObject.GetComponent<Tile>() && mouseDown)
        {
            RefrenceStorage.actionManager.ActiveFigure.PlanMove(selectedObject);
        }

    }
    //make the mouse stop counting a object as one it is on
    public void MouseOffObject(GameObject removedObject)
    {
        //if the mouse is achualy over the object
        if (mouseOver.Contains(removedObject))
        {
            //do varius things to make the bject deselect correctly
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
            if (removedObject.GetComponent<Hoverable>() || removedObject == RefrenceStorage.tooltip)
            {
                StartCoroutine(UpdateTooltip());
            }
            if (removedObject.GetComponent<Card>())
            {
                deckManager.DeSelectCard(removedObject);
            }
            mouseOver.Remove(removedObject);
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
            if (playerControler.PlanningMove && selectedObject == null && mouseDown)
            {
                actionManager.ActiveFigure.PlanMove(mapManager.GetTileAtHex(playerControler.HexPos));
            }

        }
    }
    //update tooltip based on what you are currently hovering over
    public IEnumerator UpdateTooltip()
    {
        yield return new WaitForEndOfFrame();
        if (!(selectedObject && (selectedObject.GetComponent<Hoverable>() || selectedObject == RefrenceStorage.tooltip)))
        {
            //Debug.Log(selectedObject);
            hoveredObject = null;
            tooltipManager.StopHoveringOver();
        }

    }

    //whenmouse button is clicked down
    public void MouseClicked()
    {
        clickedObject = selectedObject;
        if (clickedObject)
        {
            if (playerControler.PlanningMove && selectedObject.GetComponent<Tile>())
            {
                actionManager.ActiveFigure.PlanMove(selectedObject);
            }
            if (clickedObject.GetComponent<UIButton>())
            {
                GameObject image = clickedObject.transform.Find("Image").gameObject;
                image.GetComponent<Image>().color = clickedObject.GetComponent<UIButton>().ClickedColor;
                if (clickedObject.GetComponent<ChangeAbilityPower>())
                {
                    StartCoroutine(heldButtonRoutine = clickedObject.GetComponent<ChangeAbilityPower>().HoldClick());
                }
            }
            if (clickedObject.GetComponent<Dragable>() && !dragableClicked && playerControler.CanPlayCards == true)
            {
                dragableClicked = true;
                //Canvas canvas = clickedObject.AddComponent<Canvas>();
                //canvas.overrideSorting = true;
                //canvas.sortingLayerName = "Card";
                //deckManager.Hand.transform.SetAsLastSibling();
                //clickedObject.transform.SetAsLastSibling();
                StartCoroutine(ShortFirstClick());
            }
        }
    }
    //detect if player is holding down button or just clicked it
    private IEnumerator ShortFirstClick()
    {
        //Debug.Log("MechanicalAutomaton");
        shortClick = true;
        yield return new WaitForSeconds(0.25f);
        shortClick = false;
    }
    //when mouse button is released
    public void MouseReleased()
    {
        if (heldButtonRoutine != null)
        {
            StopCoroutine(heldButtonRoutine);
        }
        //if a object was clicked
        if (clickedObject)
        {
            //if the clicked object is a card play it or return it to hand if it wasnt a short click to pick the card up
            if (dragableClicked && !shortClick)
            {
                if (clickedObject.GetComponent<Card>() != null)
                {
                    if (mousePos.y > topPlayLine * Screen.height)
                    {
                        clickedObject.GetComponent<Card>().AttemptToPlayTop();
                    }
                    else if (mousePos.y > bottomPlayLine * Screen.height)
                    {
                        clickedObject.GetComponent<Card>().AttemptToPlayBottom();
                    }
                    else
                    {
                        MouseOffObject(clickedObject);
                        StartCoroutine(deckManager.UpdateHand());
                    }
                }
                dragableClicked = false;
            }
            //if the object clicked when the mouse went down is the same as the one that is currently selected
            if (clickedObject == selectedObject)
            {
                //do varius things depending on what you clicked
                if (clickedObject.GetComponent<Card>() && deckManager.IsChoosingCard)
                {
                    deckManager.SelectedCard = clickedObject;
                }
                if (clickedObject.GetComponent<UIButton>() && !clickedObject.GetComponent<ChangeAbilityPower>())
                {
                    clickedObject.GetComponent<UIButton>().Activate();
                }
                if (clickedObject.GetComponent<Figure>())
                {
                    StartCoroutine(playerControler.FigureClicked(clickedObject));
                }
                if (clickedObject.GetComponent<IsReward>())
                {
                    StartCoroutine(rewardManager.RewardSelected(clickedObject));
                }
            }
            //if the object is a ui button do return it to it's base color
            if (clickedObject.GetComponent<UIButton>())
            {
                GameObject image = clickedObject.transform.Find("Image").gameObject;
                image.GetComponent<Image>().color = clickedObject.GetComponent<UIButton>().BaseColor;
            }
        }

        if (selectedObject && selectedObject.GetComponent<Card>())
        {
            playerControler.CardClicked(clickedObject);
        }
        if (selectedObject && selectedObject.GetComponent<Tile>())
        {
            playerControler.TileClicked(selectedObject);
        }
        if (!dragableClicked)
        {
            clickedObject = null;
        }

    }
}

