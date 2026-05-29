using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Lootable;

public class Quest : MonoBehaviour
{
    //protected RefrenceStorage refrenceStorage;
    protected OverallStatistics overallStatistics;
    protected RewardManager rewardManager;
    protected ActionManager actionManager;
    protected PlayerControler playerControler;
    protected SpriteRenderer tileImage;
    protected VariableDisplayer contentsDisplay;
    private VariableDisplayer questDescriptionDisplay;

    //private PlayerControler playerControler;
    //private RewardManager rewardManager;
    private Interactable interactable;
    [SerializeField]
    protected List<Reward> rewards = new List<Reward>();
    protected bool isActive, isComplete;
    protected string description;

    public virtual void Awake()
    {
        //refrenceStorage = GameObject.Find("RefrenceStorage").GetComponent<RefrenceStorage>();
        overallStatistics = RefrenceStorage.overallStatistics;
        rewardManager = RefrenceStorage.rewardManager;
        actionManager = RefrenceStorage.actionManager;
        playerControler = RefrenceStorage.playerControler;
        tileImage = transform.Find("BaseTileImage").GetComponent<SpriteRenderer>();
        //playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        //rewardManager = GameObject.Find("RewardManager").GetComponent<RewardManager>();
        contentsDisplay = transform.Find("TileUI").Find("ContentsText").GetComponent<VariableDisplayer>();
        questDescriptionDisplay = transform.Find("TileUI").Find("QuestDescriptionText").GetComponent<VariableDisplayer>();

        interactable = gameObject.GetComponent<Interactable>();

        interactable.InteractedWith.AddListener(InteractedWith);


        string LootDescription = "";
        foreach (Reward reward in rewards)
        {
            if (reward.rewardType == 1)
            {
                LootDescription += "<sprite name=CardReward> ";
            }
            else if (reward.rewardType == 2)
            {
                LootDescription += "<sprite name=RelicReward> ";
            }
            else if (reward.rewardType == 3)
            {
                LootDescription += "<sprite name=HealthReward> ";
            }
        }
        contentsDisplay.DisplayString(LootDescription);
        SetDescription(description);
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
        tileImage.color = new Color(0.7f, 0f, 0.2f);
        SetDescription(description);
        yield break;
    }
    public virtual void CompleteQuest()
    {
        isActive = false;
        isComplete = true;
        tileImage.color = new Color(0.4352941f, 0.4352941f, 0.4352941f);
        StartCoroutine(Reward());
        contentsDisplay.DisplayString("");
        SetDescription("");
    }
    public virtual IEnumerator Reward()
    {
        actionManager.QueueAction(rewardManager.QuestReward(rewards));
        yield break;
    }
    public void SetDescription(string description)
    {
        questDescriptionDisplay.DisplayString(description);
    }
}

