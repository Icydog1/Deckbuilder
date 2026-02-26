using System.Collections;
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
        if (isCurrentCard)
        {
            if (playerControler.actionDone)
            {

            }
            if (isTopPlayed && !isPlaying)
            {
                isPlaying = true;
                currentStep = 0;
                topGlow.SetActive(true);
                StartCoroutine(PlayTop());
                mouseManager.clickedObject = null;

            }
            if (isBottomPlayed && !isPlaying)
            {
                isPlaying = true;
                currentStep = 0;
                bottomGlow.SetActive(true);

                StartCoroutine(PlayBottom());
                mouseManager.clickedObject = null;

            }
        }
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
        mouseManager.MouseOffObject(transform.position.z, gameObject);
        //Debug.Log("done playing");
        currentStep = 0;
    }

    public virtual IEnumerator PlayTop()
    {
        Debug.Log("Warning Baseclass top played");
        yield return null;
    }

    public virtual IEnumerator PlayBottom()
    {
        Debug.Log("Warning Baseclass bottom played");
        yield return null;

    }


    

}
