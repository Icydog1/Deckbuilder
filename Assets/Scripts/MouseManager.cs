using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
        //Debug.Log("new object " + newObject);
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
    public void MouseOffObject(GameObject removedObject)
    {
        //Debug.Log("old object " + removedObject);

        if (mouseOver.Contains(removedObject))
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
                actionManager.ActiveFigure.PlanMove(mapManager.GetTileAtHex(playerControler.OneToOnePos));
            }

        }
    }
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
    private IEnumerator ShortFirstClick()
    {
        //Debug.Log("MechanicalAutomaton");
        shortClick = true;
        yield return new WaitForSeconds(0.25f);
        shortClick = false;
    }
    public void MouseReleased()
    {
        if (heldButtonRoutine != null)
        {
            StopCoroutine(heldButtonRoutine);
        }
        if (clickedObject)
        {
            if (dragableClicked && !shortClick)
            {
                //Debug.Log("stoped draging");
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
                        MouseOffObject(clickedObject);
                        StartCoroutine(deckManager.UpdateHand());
                    }
                    //deckManager.DeSelectCard(clickedObject);
                    //deckManager.Hand.transform.SetAsFirstSibling();
                }
                dragableClicked = false;
            }
            if (clickedObject == selectedObject)
            {
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
        //if (selectedObject && selectedObject.GetComponent<Tile>() && playerControler.CanMove)
        //{
        //    playerControler.PlanMove(selectedObject);
        //    StartCoroutine(playerControler.MoveAlongPath());
        //}
        if (!dragableClicked)
        {
            clickedObject = null;
        }

    }
}

