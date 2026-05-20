using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Lootable;

public class Quest : MonoBehaviour
{
    protected RefrenceStorage refrenceStorage;
    protected OverallStatistics overallStatistics;
    protected RewardManager rewardManager;
    protected ActionManager actionManager;
    protected PlayerControler playerControler;
    protected SpriteRenderer tileImage;
    //private PlayerControler playerControler;
    //private RewardManager rewardManager;
    private Interactable interactable;
    [SerializeField]
    protected List<Reward> rewards = new List<Reward>();
    protected bool isActive, isComplete;

    public virtual void Awake()
    {
        refrenceStorage = GameObject.Find("RefrenceStorage").GetComponent<RefrenceStorage>();
        overallStatistics = refrenceStorage.OverallStatistics;
        rewardManager = refrenceStorage.RewardManager;
        actionManager = refrenceStorage.ActionManager;
        playerControler = refrenceStorage.PlayerControler;
        tileImage = transform.Find("BaseTileImage").GetComponent<SpriteRenderer>();
        //playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        //rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();

        interactable = gameObject.GetComponent<Interactable>();

        interactable.InteractedWith.AddListener(InteractedWith);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InteractedWith()
    {
        //Debug.Log("Gain Level Function");
        AtemptToGainQuest();
    }
    public virtual void AtemptToGainQuest()
    {
        if (!isActive && !isComplete)
        {
            StartCoroutine(GainQuest());
        }
    }
    public virtual IEnumerator GainQuest()
    {
        yield break;
    }
    public virtual void CompleteQuest()
    {
        isActive = false;
        isComplete = true;
        tileImage.color = new Color(0.7f, 0.5f, 0.6f);
        StartCoroutine(Reward());
    }
    public virtual IEnumerator Reward()
    {
        actionManager.QueueAction(rewardManager.QuestReward(rewards));
        yield break;
    }
}

