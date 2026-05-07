using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected DeckManager deckManager;
    protected CardEffectText topText, bottomText;
    protected VariableDisplayer topCostText, bottomCostText;
    protected ActionManager actionManager;

    protected GameObject topGlow, bottomGlow;
    public GameObject TopGlow { get { return topGlow; } }
    public GameObject BottomGlow { get { return bottomGlow; } }
    protected GameObject originalCard;
    public GameObject OriginalCard { get { return originalCard; } set { originalCard = value; } }

    protected bool isCurrentCard;
    protected int topCost, bottomCost;
    protected bool isTopPlayed, isBottomPlayed;
    protected bool isPlaying;
    //protected int currentStep;
    //protected bool nextAction;
    //public bool NextAction { set { nextAction = value;}}
    protected bool stopPlaying;
    public bool StopPlaying { set { stopPlaying = value; } }

    protected bool isPreparingTop;

    protected List<System.Action> topActions = new List<System.Action>();
    protected List<System.Action> bottomActions = new List<System.Action>();
    protected List<System.Action> currentActions = new List<System.Action>();

    protected List<string> topDescription = new List<string>();
    protected List<string> bottomDescription = new List<string>();
    protected List<string> currentDescription = new List<string>();
    protected string currentDescriptionString = "";
    protected string additionalTopDescription, additionalBottomDescription;

    [SerializeField]
    protected int rarity = 1;
    protected string cardName;

    public int Rarity { get { return rarity; } }

    //private float baseAbsoluteSize = 1;
    //private float relativeSize;
    //public float RelativeSize { get { return relativeSize; } set { relativeSize = value; SetRelativeSize(); } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        actionManager = GameObject.Find("ActionManager").GetComponent<ActionManager>();

        topGlow = transform.Find("TopGlow").gameObject;
        bottomGlow = transform.Find("BottomGlow").gameObject;
        topText = transform.Find("TopEffects").GetComponent<CardEffectText>();
        bottomText = transform.Find("BottomEffects").GetComponent<CardEffectText>();
        topCostText = transform.Find("TopCost").GetComponent<VariableDisplayer>();
        bottomCostText = transform.Find("BottomCost").GetComponent<VariableDisplayer>();
        cardName = this.name;
        cardName = cardName.Replace("(Clone)", "");
        cardName = Regex.Replace(cardName, "(.)([A-Z,0-9])", "$1 $2");
        transform.Find("CardName").gameObject.GetComponent<TextMeshProUGUI>().SetText(cardName);
    }
    public IEnumerator PrepareActions()
    {
        currentActions = topActions;
        yield return StartCoroutine(PrepareTop());
        currentActions = bottomActions;
        yield return StartCoroutine(PrepareBottom());
        deckManager.SetRelativeCardSize(gameObject, 1);
        PrepareCardDiscription();


    }

    public virtual void Start()
    {
        // base class code runs


    }

    // Update is called once per frame
    public virtual void Update()
    {

    }




    public virtual void AttemptToPlayTop()
    {
        if (playerControler.TopEnergy >= topCost)
        {
            isTopPlayed = true;
            playerControler.TopEnergy -= topCost;
            SetPlayed();
        }
        else
        {
            PlayFailed();
        }
    }
    public virtual void AttemptToPlayBottom()
    {
        if (playerControler.BottomEnergy >= bottomCost)
        {
            isBottomPlayed = true;
            playerControler.BottomEnergy -= bottomCost;

            SetPlayed();

        }
        else
        {
            PlayFailed();
        }
    }

    public void PlayFailed()
    {
        topGlow.SetActive(false);
        bottomGlow.SetActive(false);
        mouseManager.MouseOffObject(gameObject);
        deckManager.UpdateHand();
    }

    public void SetPlayed()
    {
        deckManager.PlayCard(gameObject);
        isCurrentCard = true;
        playerControler.CardPlayed = true;
        playerControler.PlayedCard = gameObject;
        playerControler.PlayedCardScript = gameObject.GetComponent<Card>();
        playerControler.UpdatePlayer();
        //currentStep = 0;
        mouseManager.ClickedObject = null;
        if (isTopPlayed)
        {
            topGlow.SetActive(true);
            StartCoroutine(PlayTop());
        }
        if (isBottomPlayed)
        {
            bottomGlow.SetActive(true);
            StartCoroutine(PlayBottom());
        }
    }

    public void DonePlaying()
    {
        isPlaying = false;
        isTopPlayed = false;
        isBottomPlayed = false;
        playerControler.CardPlayed = false;
        playerControler.UpdatePlayer();
        topGlow.SetActive(false);
        bottomGlow.SetActive(false);
        deckManager.DiscardCard(gameObject);
        mouseManager.MouseOffObject(gameObject);
        stopPlaying = false;
        //Debug.Log("done playing");
        //currentStep = 0;
    }

    public IEnumerator PlayTop()
    {
        playerControler.ActionsRemaining = new List<string>(topDescription);
        playerControler.NextAction = false;
        foreach (System.Action action in topActions)
        {
            if (stopPlaying == false)
            {
                action();
                yield return new WaitUntil(() => playerControler.NextAction == true);
                playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public IEnumerator PlayBottom()
    {
        playerControler.ActionsRemaining = new List<string>(bottomDescription);
        playerControler.NextAction = false;
        foreach (System.Action action in bottomActions)
        {
            if (stopPlaying == false)
            {
                action();
                yield return new WaitUntil(() => playerControler.NextAction == true);
                playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public void PrepareCardDiscription()
    {
        topDescription.Clear();
        bottomDescription.Clear();
        playerControler.IsPlanning = true;
        playerControler.PlanDescription = topDescription;
        Debug.Log("started planing");
        foreach (System.Action action in topActions)
        {
            action();
        }
        Debug.Log("finished planing");

        playerControler.PlanDescription = bottomDescription;
        foreach (System.Action action in bottomActions)
        {
            action();
        }
        topCostText.DisplayString("<color=red>" + topCost);
        bottomCostText.DisplayString("<color=#008000>" + bottomCost);
        if (additionalTopDescription != null)
        {
            List<string> newDescription = new List<string>(topDescription);
            newDescription.Insert(0,additionalTopDescription);
            topText.DisplayText(newDescription);
        }
        else
        {
            topText.DisplayText(topDescription);
        }
        if (additionalBottomDescription != null)
        {
            List<string> newDescription = new List<string>(bottomDescription);
            newDescription.Insert(0, additionalBottomDescription);
            bottomText.DisplayText(newDescription);
        }
        else
        {
            bottomText.DisplayText(bottomDescription);
        }

        playerControler.IsPlanning = false;
    }
    public virtual IEnumerator PrepareTop()
    {

    }

    public virtual IEnumerator PrepareBottom()
    {

    }
}
