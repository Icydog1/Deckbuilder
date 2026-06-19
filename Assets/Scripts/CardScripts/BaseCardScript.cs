using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public class Action
    {
        public Func<IEnumerator> preformedAbility;
        public string description = null;

        public Action(Func<IEnumerator> ability, string descriptionOverride = null)
        {
            preformedAbility = ability;
            description = descriptionOverride;
        }
    }

	//protected List<Func<IEnumerator>> topActions = new List<Func<IEnumerator>>();
	//protected List<Func<IEnumerator>> bottomActions = new List<Func<IEnumerator>>();
    //protected List<Func<IEnumerator>> currentActions = new List<Func<IEnumerator>>());
    protected List<Action> topActions = new List<Action>();
    protected List<Action> bottomActions = new List<Action>();
    protected List<Action> currentActions = new List<Action>();

    //protected List<IEnumerator> topActions = new List<IEnumerator>();
    //protected List<IEnumerator> bottomActions = new List<IEnumerator>();
    //protected List<IEnumerator> currentActions = new List<IEnumerator>());

    protected List<ActionDescription> topDescription = new List<ActionDescription>();
    protected List<ActionDescription> bottomDescription = new List<ActionDescription>();
    protected List<ActionDescription> currentDescription = new List<ActionDescription>();
    protected string currentDescriptionString = "";
    protected string additionalTopDescription, additionalBottomDescription;

    //[SerializeField]
    protected int rarity = 1;
    protected string cardName;
    public Card(int baseRarity, int initialTopCost, int initialBottomCost)
    {
        rarity = baseRarity;
        topCost = initialTopCost;
        bottomCost = initialBottomCost;
    }
    public int Rarity { get { return rarity; } }
    protected Image rarityGlow;

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

        rarityGlow = transform.Find("RarityGlow").gameObject.GetComponent<Image>();

        switch (rarity)
        {
            case 0:
                {
                    rarityGlow.color = new Color(0, 0, 0, 0f);
                    break;
                }
            case 1:
                {
                    rarityGlow.color = new Color(0, 0, 0, 0f);
                    break;
                }
            case 2:
                {
                    rarityGlow.color = new Color(0, 0, 1, 0.5f);
                    break;
                }
            case 3:
                {
                    rarityGlow.color = new Color(1, 0.8f, 0, 1);
                    break;
                }
            default:
                {
                    rarityGlow.color = new Color(0, 0, 0, 0f);
                    break;
                }
        }


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
        playerControler.ActionsRemaining = new List<ActionDescription>(topDescription);
        //playerControler.NextAction = false;
        foreach (Action action in topActions)
        {
            if (stopPlaying == false)
            {
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility()));

                //yield return new WaitUntil(() => playerControler.NextAction == true);
                //playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public IEnumerator PlayBottom()
    {
        playerControler.ActionsRemaining = new List<ActionDescription>(bottomDescription);
        //playerControler.NextAction = false;
        foreach (Action action in bottomActions)
        {
            if (stopPlaying == false)
            {
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility()));

                //yield return new WaitUntil(() => playerControler.NextAction == true);
                //playerControler.NextAction = false;
            }
        }
        DonePlaying();
    }

    public IEnumerator PrepareCardDiscription(bool unmodified = false)
    {
        //Debug.Log("updated entire card");
		topDescription.Clear();
        bottomDescription.Clear();

        playerControler.UnmodifiedAction = unmodified;
        foreach (Action action in topActions)
        {
            if (action.description == null)
            {
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility(), topDescription));
            }
            else
            {
                topDescription.Add(new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, action.description) }));
            }
        }

		foreach (Action action in bottomActions)
        {
            if (action.description == null)
            {
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility(), bottomDescription));
            }
            else
            {
                bottomDescription.Add(new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, action.description) }));
            }
        }

        topCostText.DisplayString("<color=red>" + topCost);
        bottomCostText.DisplayString("<color=#008000>" + bottomCost);
        if (additionalTopDescription != null)
        {
            List<ActionDescription> newDescription = new List<ActionDescription>(topDescription);
            newDescription.Insert(0, new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, additionalBottomDescription) }));

            topText.DisplayDescription(newDescription);
        }
        else
        {
            topText.DisplayDescription(topDescription);
        }
        if (additionalBottomDescription != null)
        {
            List<ActionDescription> newDescription = new List<ActionDescription>(bottomDescription);
            newDescription.Insert(0, new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, additionalBottomDescription) }));

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
        foreach (ActionDescription action in topDescription)
        {
            if (action.ActionName == actionName)
            {
                action.ActionModifiers[modifierNum].UpdateValue();
            }
        }
        topText.DisplayDescription(topDescription);
        foreach (ActionDescription action in bottomDescription)
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
