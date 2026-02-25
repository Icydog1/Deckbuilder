using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    protected PlayerControler playerControler;
    protected MouseManager mouseManager;
    protected DeckManager deckManager;

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
                StartCoroutine(PlayTop());
                mouseManager.clickedObject = null;

            }
            if (isBottomPlayed && !isPlaying)
            {
                isPlaying = true;
                PlayBottom();
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
            IsPlayed();
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
            IsPlayed();
        }
        else
        {
            deckManager.UpdateHand();
        }
    }

    public void IsPlayed()
    {
        isCurrentCard = true;
        playerControler.playedCard = gameObject;
        playerControler.playedCardScript = gameObject.GetComponent<Card>();
    }



    public virtual IEnumerator PlayTop()
    {
        Debug.Log("Baseclass top played");
        yield return null;
    }

    public virtual IEnumerator PlayBottom()
    {
        Debug.Log("Baseclass bottom played");
        yield return null;

    }
    public void DonePlaying()
    {
        isPlaying = false;
        isTopPlayed = false;
        isBottomPlayed = false;
        deckManager.DiscardCard(gameObject);
        mouseManager.MouseOffObject(transform.position.z, gameObject);
        Debug.Log("done playing");
        currentStep = 0;
    }

    

}
