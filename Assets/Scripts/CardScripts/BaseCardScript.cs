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
    protected GameManager gameManager;
    

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
    private int doingSomething;

    protected bool isPreparingTop;

    protected List<Figure> actingFigures = new List<Figure>();
    public List<Figure> ActingFigures { get { return actingFigures; } set { actingFigures = value; } }

    public class Action
    {
        public Func<IEnumerator> preformedAction;
        public Func<Figure, IEnumerator> preformedAction2;
        public string description = null;
        public bool multitarget;

        public Action(Func<Figure, IEnumerator> action, string descriptionOverride = null)
        {
            preformedAction2 = action;
            description = descriptionOverride;
            multitarget = true;
            //Debug.Log(targets.Count);
            //RefrenceStorage.playerControler.EffectedFigures.Add(RefrenceStorage.playerControler);
            //Debug.Log(targets.Count);
            //Debug.Log(targets[0]);
        }
        public Action(Func<IEnumerator> action, string descriptionOverride = null)
        {
            preformedAction = action;
            description = descriptionOverride;
            multitarget = false;
        }
    }

	//protected List<Func<IEnumerator>> topActions = new List<Func<IEnumerator>>();
	//protected List<Func<IEnumerator>> bottomActions = new List<Func<IEnumerator>>();
    //protected List<Func<IEnumerator>> currentActions = new List<Func<IEnumerator>>());
    protected List<Action> topActions = new List<Action>();
    protected List<Action> bottomActions = new List<Action>();
    protected List<Action> currentActions = new List<Action>();

    protected List<Action> topEndActions = new List<Action>();
    protected List<Action> bottomEndActions = new List<Action>();
    protected List<Action> currentEndActions = new List<Action>();

    //protected List<IEnumerator> topActions = new List<IEnumerator>();
    //protected List<IEnumerator> bottomActions = new List<IEnumerator>();
    //protected List<IEnumerator> currentActions = new List<IEnumerator>());

    protected List<ActionDescription> topDescription = new List<ActionDescription>();
    protected List<ActionDescription> bottomDescription = new List<ActionDescription>();
    protected List<ActionDescription> currentDescription = new List<ActionDescription>();
    protected string currentDescriptionString = "";
    protected string additionalTopDescription, additionalBottomDescription;
    //protected List<string> topKeywords = new List<string>();
    //protected List<string> bottomKeywords = new List<string>();
    protected Dictionary<string, int> topKeywords = new Dictionary<string, int>();
    protected Dictionary<string, int> bottomKeywords = new Dictionary<string, int>();
    protected Dictionary<string, int> currentKeywords = new Dictionary<string, int>();
    public Dictionary<string,int> CurrentKeywords { get { return currentKeywords; }}

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

    protected Func<bool> returnFromExhaustConditiion;
    protected GameObject returnToList;
    protected Figure currentTarget;
    //private float baseAbsoluteSize = 1;
    //private float relativeSize;
    //public float RelativeSize { get { return relativeSize; } set { relativeSize = value; SetRelativeSize(); } }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Awake()
    {
        playerControler = RefrenceStorage.playerControler;
        mouseManager = RefrenceStorage.mouseManager;
        deckManager = RefrenceStorage.deckManager;
        actionManager = RefrenceStorage.actionManager;
        gameManager = RefrenceStorage.gameManager;

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
        doingSomething++;
        //Debug.Log("started playing");
        yield return StartCoroutine(deckManager.PlayCard(gameObject));
        isCurrentCard = true;
        playerControler.CardPlayed = true;
        playerControler.PlayedCard = gameObject;
        playerControler.PlayedCardScript = this;
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
        doingSomething--;
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
        if (returnToList == null)
        {
            StartCoroutine(deckManager.DiscardCard(gameObject));

        }
        else
        {
            deckManager.StartCoroutine(ExhaustAfterPlayed());
        }
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
            yield return StartCoroutine(PreformAction(action));

            //if (stopPlaying == false)
            //{
            //    yield return StartCoroutine(actionManager.PreformAction(action.preformedAction()));

            //    //yield return new WaitUntil(() => playerControler.NextAction == true);
            //    //playerControler.NextAction = false;
            //}
        }
        DonePlaying();
    }

    public IEnumerator PlayBottom()
    {
        playerControler.ActionsRemaining = new List<ActionDescription>(bottomDescription);
        //playerControler.NextAction = false;
        foreach (Action action in bottomActions)
        {
            //Debug.Log("doing 1 action");
            yield return StartCoroutine(PreformAction(action));
            //if (action.targets == null)
            //{
            //    yield return StartCoroutine(actionManager.PreformAction(action.preformedAction()));
            //}
            //else
            //{
            //    foreach (Figure target in action.targets)
            //    {
            //        currentTarget
            //    }
            //}
            //if (stopPlaying == false)
            //{
            //    yield return StartCoroutine(actionManager.PreformAction(action.preformedAction()));

            //    //yield return new WaitUntil(() => playerControler.NextAction == true);
            //    //playerControler.NextAction = false;
            //}
        }
        DonePlaying();
    }
    public IEnumerator PreformAction(Action action, List<ActionDescription> planTo = null)
    {
        if (action.multitarget == false)
        {
            if (planTo != null)
            {
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAction(), planTo));
            }
            else
            {
                if (stopPlaying == false)
                {
                    //Debug.Log("started action");
                    playerControler.ActionEnded = false;
                    yield return StartCoroutine(actionManager.PreformAction(action.preformedAction()));
                    //Debug.Log("finished action");
                }
            }

        }
        else
        {
            if (planTo != null)
            {
                playerControler.UnmodifiedAction = true;
                yield return StartCoroutine(actionManager.PreformAction(action.preformedAction2(playerControler), planTo));
                playerControler.UnmodifiedAction = false;
            }
            else
            {
                //ActionDescription copyDescription = playerControler.ActionsRemaining[0];
                if (actingFigures.Count == 0)
                {
                    playerControler.ActionsRemaining.RemoveAt(0);
                }
                else if(actingFigures.Count > 1)
                {
                    for (int i = 1; i < actingFigures.Count; i++)
                    {
                        Debug.Log("not tested");
                        playerControler.ActionsRemaining.Insert(i, playerControler.ActionsRemaining[0]); //copyDescription.Clone()
                    }
                }
                playerControler.UpdatePlan();
                //Debug.Log(actingFigures.Count);
                foreach (Figure target in actingFigures)
                {
                    //Debug.Log(target);
                    if (stopPlaying == false)
                    {
                        playerControler.ActionEnded = false;
                        //currentTarget = target;
                        yield return StartCoroutine(actionManager.PreformAction(action.preformedAction2(target), planTo));
                        if (!playerControler.ActionEnded)
                        {
                            playerControler.EndAction();
                        }
                    }
                }
            }
        }
    }

    public void PrepareExhaustAfterPlayed(Func<bool> conditiion, GameObject list = null)
    {
        returnFromExhaustConditiion = conditiion;
        if (list == null)
        {
            list = deckManager.Deck;
        }
        returnToList = list;
    }
    public IEnumerator ExhaustAfterPlayed()
    {
        yield return deckManager.StartCoroutine(deckManager.ExhaustUntil(gameObject, returnFromExhaustConditiion, returnToList));
        returnToList = null;
    }
    public IEnumerator PrepareCardDiscription(bool unmodified = false)
    {
        doingSomething++;
        //Debug.Log("updated entire card");
        playerControler.PlayedCardScript = this;
        topDescription.Clear();
        bottomDescription.Clear();
        currentKeywords = topKeywords;
        playerControler.UnmodifiedAction = unmodified;
        foreach (Action action in topActions)
        {
            if (action.description == null)
            {
                yield return StartCoroutine(PreformAction(action, topDescription));
                //yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility(), topDescription));
            }
            else
            {
                topDescription.Add(new ActionDescription("OverrideDescription", new List<ActionModifier>() { new ActionModifier(playerControler, action.description) }));
            }
        }
        currentKeywords = bottomKeywords;

        foreach (Action action in bottomActions)
        {
            if (action.description == null)
            {
                yield return StartCoroutine(PreformAction(action, bottomDescription));

                //yield return StartCoroutine(actionManager.PreformAction(action.preformedAbility(), bottomDescription));
            }
            else
            {
                bottomDescription.Add(new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, action.description) }));
            }
        }

        PrepareCardKeywords(true);
        PrepareCardKeywords(false);
        topCostText.DisplayString("<color=red>" + topCost);
        bottomCostText.DisplayString("<color=#008000>" + bottomCost);

        if (additionalTopDescription != null)
        {
            List<ActionDescription> newDescription = new List<ActionDescription>(topDescription);
            newDescription.Insert(0, new ActionDescription("???", new List<ActionModifier>() { new ActionModifier(playerControler, additionalTopDescription) }));

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
        doingSomething--;
    }



    public void PrepareCardKeywords(bool isTop)
    {
        if (isTop)
        {
            currentKeywords = topKeywords;
            currentDescription = topDescription;
        }
        else
        {
            currentKeywords = bottomKeywords;
            currentDescription = bottomDescription;
        }
        var sortedByKey = currentKeywords.OrderBy(pair => pair.Key);
        foreach (var pair in sortedByKey)
        {
            if (pair.Value != 0)
            {
                //if (pair.Key == "Command")
                //{
                //    currentDescription.Insert(0, new ActionDescription("Command", new List<ActionModifier>() { new ActionModifier(playerControler, "Command") }));
                //}
                //if (pair.Key == "Augment")
                //{
                //    currentDescription.Insert(0, new ActionDescription("Augment", new List<ActionModifier>() { new ActionModifier(playerControler, "Augment") }));
                //}
                if (pair.Key == "Exhausting")
                {
                    currentDescription.Add(new ActionDescription("Exhausting", new List<ActionModifier>() { new ActionModifier(playerControler, "Exausting ", pair.Value) }));
                }
            }

        }
        //if (currentKeywords.TryGetValue("Augment", out int keywordValue) && keywordValue != 0)
        //{
        //    currentDescription.Insert(0, new ActionDescription("Augment", new List<ActionModifier>() { new ActionModifier(playerControler, "Augment") }));
        //}
        //if (currentKeywords.TryGetValue("Exhausting", out int keywordValue2) && keywordValue2 != 0)
        //{
        //    currentDescription.Add(new ActionDescription("Exhausting", new List<ActionModifier>() { new ActionModifier(playerControler, "Exausting ", keywordValue2) }));
        //}
    }

    public IEnumerator UpdateCardDiscription(string modifiedAction)
    {
        doingSomething++;
        playerControler.PlayedCardScript = this;
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
            case "SkillValue":
                actionName = "Skill";
                modifierNum = 0;
                break;
            case "RangeValue":
                actionName = "Range";
                modifierNum = 1;
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
        doingSomething--;
        yield break;
    }

    public virtual void PrepareTop()
    {

    }

    public virtual void PrepareBottom()
    {

    }

    public void AttemptToDestroy()
    {
        if (doingSomething == 0)
        {
            //Debug.Log("destroy immediatly");
            Destroy(gameObject);
        }
        else
        {
            transform.parent = RefrenceStorage.UI.transform;
            transform.localScale = new Vector3(0, 0, 1);
            //Debug.Log("Destroy wait");
            StartCoroutine(DestroyWhenReady());
        }
    }
    public IEnumerator DestroyWhenReady()
    {
        yield return new WaitUntil(() => doingSomething == 0);
        //Debug.Log("Destroy wait finished");
        Destroy(gameObject);
    }
    //disables a card if it isnt doing somthing otherwise waits until card is done and 
    public void AttemptToDisable()
    {
        //gameObject.SetActive(false);

        if (doingSomething == 0)
        {
            //Debug.Log("disabled immediatly");
            gameObject.SetActive(false);
        }
        else
        {
            //Debug.Log("disabled wait");
            //transform.parent = RefrenceStorage.UI.transform;
            //transform.localScale = new Vector3(0, 0, 1);
            StartCoroutine(DisableWhenReady());
        }
    }
    public IEnumerator DisableWhenReady()
    {
        yield return new WaitUntil(() => doingSomething == 0);
        //Debug.Log("disabled wait finished");
        gameObject.SetActive(false);
    }
}
