using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

	protected List<Func<IEnumerator>> topActions = new List<Func<IEnumerator>>();
	protected List<Func<IEnumerator>> bottomActions = new List<Func<IEnumerator>>();
	protected List<Func<IEnumerator>> currentActions = new List<Func<IEnumerator>>();
    //protected List<IEnumerator> topActions = new List<IEnumerator>();
    //protected List<IEnumerator> bottomActions = new List<IEnumerator>();
    //protected List<IEnumerator> currentActions = new List<IEnumerator>();

    protected List<Action> topDescription = new List<Action>();
    protected List<Action> bottomDescription = new List<Action>();
    protected List<Action> currentDescription = new List<Action>();
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

        deckManager.SetRelativeCardSize(gameObject, 1);


        currentActions = topActions;
        PrepareTop();
        currentActions = bottomActions;
        PrepareBottom();
        //StartCoroutine(PrepareCardDiscription());
    }

    public IEnumerator FirstSpawned()
    {
        yield return StartCoroutine(PrepareCardDiscription());
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
            StartCoroutine(SetPlayed());
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

            StartCoroutine(SetPlayed());

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
		StartCoroutine(deckManager.UpdateHand());
    }

    public IEnumerator SetPlayed()
    {
        //Debug.Log("started playing");
        yield return StartCoroutine(deckManager.PlayCard(gameObject));
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
            yield return StartCoroutine(PlayTop());
        }
        if (isBottomPlayed)
        {
            bottomGlow.SetActive(true);
            yield return StartCoroutine(PlayBottom());
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
		StartCoroutine(deckManager.DiscardCard(gameObject));
        mouseManager.MouseOffObject(gameObject);
        stopPlaying = false;
        //Debug.Log("done playing");
        //currentStep = 0;
    }

    public IEnumerator PlayTop()
    {
        playerControler.ActionsRemaining = new List<Action>(topDescription);
        //playerControler.NextAction = false;
        foreach (Func<IEnumerator> action in topActions)
        {
            if (stopPlaying == false)
            {
                yield return StartCoroutine(actionManager.PreformAction(action()));

                //yield return new WaitUntil(() => playerControler.NextAction == true);
                //playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public IEnumerator PlayBottom()
    {
        playerControler.ActionsRemaining = new List<Action>(bottomDescription);
        //playerControler.NextAction = false;
        foreach (Func<IEnumerator> action in bottomActions)
        {
            if (stopPlaying == false)
            {
                yield return StartCoroutine(actionManager.PreformAction(action()));

                //yield return new WaitUntil(() => playerControler.NextAction == true);
                //playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public IEnumerator PrepareCardDiscription()
    {
        //Debug.Log("updated entire card");
		topDescription.Clear();
        bottomDescription.Clear();

        playerControler.UnmodifiedAction = false;
        foreach (Func<IEnumerator> action in topActions)
        {
            yield return StartCoroutine(actionManager.PreformAction(action(), topDescription));
        }

		foreach (Func<IEnumerator> action in bottomActions)
        {

            yield return StartCoroutine(actionManager.PreformAction(action(), bottomDescription));
        }

        topCostText.DisplayString("<color=red>" + topCost);
        bottomCostText.DisplayString("<color=#008000>" + bottomCost);
        if (additionalTopDescription != null)
        {
            List<Action> newDescription = new List<Action>(topDescription);
            newDescription.Insert(0, new Action("???", new List<ActionModifier>() { new ActionModifier(playerControler, additionalBottomDescription) }));

            topText.DisplayDescription(newDescription);
        }
        else
        {
            topText.DisplayDescription(topDescription);
        }
        if (additionalBottomDescription != null)
        {
            List<Action> newDescription = new List<Action>(bottomDescription);
            newDescription.Insert(0, new Action("???", new List<ActionModifier>() { new ActionModifier(playerControler, additionalBottomDescription) }));

            bottomText.DisplayDescription(newDescription);
        }
        else
        {
            bottomText.DisplayDescription(bottomDescription);
        }

    }
    public IEnumerator UpdateCardDiscription(string modifiedAction)
    {
        //Debug.Log("changed card description " + modifiedAction);
        playerControler.UnmodifiedAction = false;
        string actionName = null;
        int modifierNum = 0;
        switch (modifiedAction)
        {
            case "BlockValue":
                actionName = "Block";
                modifierNum = 0;
                break;
            case "AttackValue":
                //Debug.Log("changed attack");
                actionName = "Attack";
                modifierNum = 0;
                break;
            case "MoveValue":
                //Debug.Log("changed Move");

                actionName = "Move";
                modifierNum = 0;
                break;
            case "AbilityValue":
                actionName = "Ability";
                modifierNum = 0;
                break;
            default:
                Debug.Log("Default");
                modifierNum = 0;
                break;
        }
        foreach (Action action in topDescription)
        {
            if (action.ActionName == actionName)
            {
                action.ActionModifiers[modifierNum].UpdateValue();
            }
        }
        topText.DisplayDescription(topDescription);
        foreach (Action action in bottomDescription)
        {
            if (action.ActionName == actionName)
            {
                action.ActionModifiers[modifierNum].UpdateValue();
            }
        }
        bottomText.DisplayDescription(bottomDescription);

        yield break;
    }

    public virtual void PrepareTop()
    {

    }

    public virtual void PrepareBottom()
    {

    }
}
