using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected DeckManager deckManager;
    protected CardEffectText topText, bottomText;
    protected VariableDisplayer topCostText, bottomCostText;

    protected GameObject topGlow, bottomGlow;
    public GameObject TopGlow { get { return topGlow; } }
    public GameObject BottomGlow { get { return bottomGlow; } }
    protected bool isCurrentCard;
    protected int topCost, bottomCost;
    protected bool isTopPlayed, isBottomPlayed;
    protected bool isPlaying;
    //protected int currentStep;
    protected bool nextAction;
    public bool NextAction { set { nextAction = value;}
    }

    protected bool isPreparingTop;

    protected List<System.Action> topActions = new List<System.Action>();
    protected List<System.Action> bottomActions = new List<System.Action>();
    protected List<System.Action> prepareTo = new List<System.Action>();

    protected List<string> topDescription = new List<string>();
    protected List<string> bottomDescription = new List<string>();
    protected List<string> currentDescription = new List<string>();
    protected string currentDescriptionString = "";

    [SerializeField]
    protected int rarity = 1;
    public int Rarity { get { return rarity; } }

    //private float baseAbsoluteSize = 1;
    //private float relativeSize;
    //public float RelativeSize { get { return relativeSize; } set { relativeSize = value; SetRelativeSize(); } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        topGlow = transform.Find("TopGlow").gameObject;
        bottomGlow = transform.Find("BottomGlow").gameObject;
        topText = transform.Find("TopEffects").GetComponent<CardEffectText>();
        bottomText = transform.Find("BottomEffects").GetComponent<CardEffectText>();
        topCostText = transform.Find("TopCost").GetComponent<VariableDisplayer>();
        bottomCostText = transform.Find("BottomCost").GetComponent<VariableDisplayer>();

        deckManager.SetRelativeCardSize(gameObject, 1);
        // base class code runs


        PrepareCard();
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }




    public void AttemptToPlayTop()
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
    public void AttemptToPlayBottom()
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
        playerControler.playedCard = gameObject;
        playerControler.playedCardScript = gameObject.GetComponent<Card>();
        playerControler.UpdatePlayer();
        //currentStep = 0;
        mouseManager.clickedObject = null;
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
        //Debug.Log("done playing");
        //currentStep = 0;
    }

    public IEnumerator PlayTop()
    {
        foreach (System.Action action in topActions)
        {
            action();
            yield return new WaitUntil(() => nextAction == true);
            nextAction = false;
        }
        DonePlaying();
    }

    public IEnumerator PlayBottom()
    {
        foreach (System.Action action in bottomActions)
        {
            action();
            //Debug.Log(action.Method.Name);
            yield return new WaitUntil(() => nextAction == true);
            nextAction = false;
        }
        DonePlaying();
    }

    public void PrepareCard()
    {
        prepareTo = topActions;
        isPreparingTop = true;
        currentDescription = topDescription;
        PrepareTop();
        topCostText.DisplayText(topCost);
        prepareTo = bottomActions;
        isPreparingTop = false;
        currentDescription = bottomDescription;
        PrepareBottom();
        bottomCostText.DisplayText(bottomCost);
        DisplayEffects();
    }

    public void DisplayEffects()
    {
        topText.DisplayText(topDescription);
        bottomText.DisplayText(bottomDescription);
        topDescription.Clear();
        bottomDescription.Clear();
    }


    public virtual void PrepareTop()
    {

    }

    public virtual void PrepareBottom()
    {

    }

    public void AddToDescription()
    {
        currentDescription.Add(currentDescriptionString);
        //Debug.Log(currentDescriptionString);
        currentDescriptionString = "";
    }

    public void PrepareAttack(int attackValue, int range = 1)
    {
        prepareTo.Add(() => playerControler.Attack(attackValue, range));
        currentDescriptionString = "Attack " + attackValue;
        if (range > 1)
        {
            currentDescriptionString += " range " + range;
        }
        AddToDescription();
    }

    public void PrepareMove(int moveValue, bool isJump = false)
    {
        prepareTo.Add(() => playerControler.Move(moveValue, isJump));
        currentDescriptionString = "Move " + moveValue;
        if (isJump)
        {
            currentDescriptionString += " Jump";
        }
        AddToDescription();
    }
    public void PrepareBlock(int blockValue)
    {
        prepareTo.Add(() => playerControler.Block(blockValue));
        currentDescriptionString = "Block " + blockValue;
        AddToDescription();
    }

}
