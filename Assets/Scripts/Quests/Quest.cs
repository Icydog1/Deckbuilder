using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Lootable;

public class Quest : MonoBehaviour
{
    //protected RefrenceStorage refrenceStorage;
    //protected OverallStatistics overallStatistics;
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
    int turnLimit = 0;
    int turnsLeft;
    [SerializeField]
    protected List<Reward> rewards = new List<Reward>();
    protected bool isActive, isFinished;
    protected string description, updatedDescrtiption;

    public virtual void Awake()
    {
        //refrenceStorage = GameObject.Find("RefrenceStorage").GetComponent<RefrenceStorage>();
        //overallStatistics = RefrenceStorage.overallStatistics;
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
        turnsLeft = turnLimit;
        contentsDisplay.DisplayString(LootDescription);
        SetDescription(description);
    }

    public void InteractedWith()
    {
        //Debug.Log("Gain Level Function");
        AtemptToGainQuest();
    }
    public virtual void AtemptToGainQuest()
    {
        if (!isActive && !isFinished)
        {
            StartCoroutine(GainQuest());
        }
    }
    public virtual IEnumerator GainQuest()
    {
        tileImage.color = new Color(0.7f, 0f, 0.2f);
        if (turnLimit != 0)
        {
            playerControler.PlayerTurnEndedFunc += DecreaseTimeLeft;
        }
        SetDescription(description);
        yield break;
    }
    public virtual void CompleteQuest()
    {
        isActive = false;
        isFinished = true;
        tileImage.color = new Color(0.4352941f, 0.4352941f, 0.4352941f);
        StartCoroutine(Reward());
        contentsDisplay.DisplayString("");
        SetDescription("");
    }
    public virtual void FailQuest()
    {
        isActive = false;
        isFinished = true;
        tileImage.color = new Color(0.4352941f, 0.4352941f, 0.4352941f);
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
        updatedDescrtiption = description;
        if (turnLimit != 0 && !isFinished)
        {
            description += " in " + turnsLeft + " turns";
        }
        questDescriptionDisplay.DisplayString(description);
    }
    public void DecreaseTimeLeft(PlayerControler playerControler)
    {
        turnsLeft--;
        if (turnsLeft == 0)
        {
            FailQuest();
            playerControler.PlayerTurnEndedFunc -= DecreaseTimeLeft;
        }
        SetDescription(updatedDescrtiption);
    }

}

