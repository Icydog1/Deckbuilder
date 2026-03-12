using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected DeckManager deckManager;
    public GameObject topGlow, bottomGlow;
    protected bool isCurrentCard;
    protected int topCost, bottomCost;
    protected bool isTopPlayed, isBottomPlayed;
    protected bool isPlaying;
    public int currentStep;
    public bool nextAction;

    public List<System.Action> topActions = new List<System.Action>();
    public List<System.Action> bottomActions = new List<System.Action>();

    protected string topDescription;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        topGlow = transform.Find("Top Glow").gameObject;
        bottomGlow = transform.Find("Bottom Glow").gameObject;
        // base class code runs
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }


    public void AttemptToPlayTop()
    {
        if (playerControler.topEnergy >= topCost)
        {
            isTopPlayed = true;
            playerControler.topEnergy -= topCost;


            SetPlayed();
        }
        else
        {
            deckManager.UpdateHand();
        }
    }
    public void AttemptToPlayBottom()
    {
        if (playerControler.bottomEnergy >= bottomCost)
        {
            isBottomPlayed = true;
            playerControler.bottomEnergy -= bottomCost;

            SetPlayed();

        }
        else
        {
            deckManager.UpdateHand();
        }
    }

    public void SetPlayed()
    {
        deckManager.PlayCard(gameObject);
        isCurrentCard = true;
        playerControler.cardPlayed = true;
        playerControler.playedCard = gameObject;
        playerControler.playedCardScript = gameObject.GetComponent<Card>();
        currentStep = 0;
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
        playerControler.cardPlayed = false;
        topGlow.SetActive(false);
        bottomGlow.SetActive(false);
        deckManager.DiscardCard(gameObject);
        mouseManager.MouseOffObject(gameObject);
        //Debug.Log("done playing");
        currentStep = 0;
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
    /*
    public virtual void PlayTop()
    {
        Debug.Log("Warning Baseclass top played");
    }
    
    public virtual void PlayBottom()
    {
        Debug.Log("Warning Baseclass bottom played");

    }
    */



}
