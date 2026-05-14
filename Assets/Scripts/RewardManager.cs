using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Lootable;
using static UnityEngine.GraphicsBuffer;

public class RewardManager : MonoBehaviour
{
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameManager gameManager;
    private GameObject rewardsLocation;
    private UIManager uIManager;
    private RelicManager relicManager;


    [SerializeField]
    private List<GameObject> allCardRewards = new List<GameObject>();
    private List<List<GameObject>> cardRewardsLists = new List<List<GameObject>>();
    private List<GameObject> commonCardRewards = new List<GameObject>();
    private List<GameObject> uncommonCardRewards = new List<GameObject>();
    private List<GameObject> rareCardRewards = new List<GameObject>();


    [SerializeField]
    private List<GameObject> allRelicRewards = new List<GameObject>();
    private List<List<GameObject>> relicRewardsLists = new List<List<GameObject>>();
    public List<GameObject> commonRelicRewards = new List<GameObject>();
    private List<GameObject> uncommonRelicRewards = new List<GameObject>();
    private List<GameObject> rareRelicRewards = new List<GameObject>();

    [SerializeField]
    private List<GameObject> testCardRewards = new List<GameObject>();

    private List<GameObject> currentOptions = new List<GameObject>();
    private List<GameObject> currentOptionsPrefabs = new List<GameObject>();

    private Lootable tileScript;
    private int rewardRarity;
    private float commonProbability = 1f;// 0.8f;
    private float uncommonProbability = 0f; //0.15f;
    private float rareProbability = 0f; //0.05f;

    private float relativeSpaceBetweenRewardCards = 0.5f;

    private bool isRewardCard;

    private bool isGettingReward;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        rewardsLocation = GameObject.Find("Rewards");
        cardRewardsLists.Add(commonCardRewards);
        cardRewardsLists.Add(uncommonCardRewards);
        cardRewardsLists.Add(rareCardRewards);
        relicRewardsLists.Add(commonRelicRewards);
        relicRewardsLists.Add(uncommonRelicRewards);
        relicRewardsLists.Add(rareRelicRewards);
        GameManager.GameStartedFunctions += GenerateRewardPools;
        //GenerateRewardPools();


    }
    public void GenerateRewardPools(GameManager gameManager)
    {
        commonCardRewards.Clear();
        uncommonCardRewards.Clear();
        rareCardRewards.Clear();
        commonRelicRewards.Clear();
        uncommonRelicRewards.Clear();
        rareRelicRewards.Clear();

        if (commonProbability + uncommonProbability + rareProbability != 1f)
        {
            Debug.Log("reward probabilitys dont add to 1");
        }
        foreach (GameObject card in allCardRewards)
        {
            int cardRarity = card.GetComponent<Card>().Rarity;
            cardRewardsLists[cardRarity-1].Add(card);
            //if (cardRarity == 1)
            //{
            //    commonCardRewards.Add(card);
            //}
            //else if (cardRarity == 2)
            //{
            //    uncommonCardRewards.Add(card);
            //}
            //else
            //{
            //    rareCardRewards.Add(card);
            //}
        }
        if (testCardRewards.Count > 0)
        {
            commonCardRewards = testCardRewards;
        }
        foreach (GameObject relic in allRelicRewards)
        {
            int relicRarity = relic.GetComponent<Relic>().Rarity;
            relicRewardsLists[relicRarity - 1].Add(relic);
            //if (relicRarity == 1)
            //{
            //    commonRelicRewards.Add(relic);
            //}
            //else if (relicRarity == 2)
            //{
            //    uncommonRelicRewards.Add(relic);
            //}
            //else
            //{
            //    rareRelicRewards.Add(relic);
            //}
        }
    }
    void Start()
    {
        //GameManager.GameStartedFunctions += InitialReward;

        isRewardCard = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AnyReward()
    {
        isGettingReward = true;
        playerControler.GettingReward = true;
        uIManager.IsGettingReward = true;
    }
    public void GainedReward()
    {
        isGettingReward = false;
        playerControler.GettingReward = false;
        uIManager.IsGettingReward = false;
    }
    //public void InitialReward(GameManager gameManager)
    //{
    //    AnyReward();
    //    GenerateReward(3);

    //}


    public IEnumerator TileReward(GameObject tile, List<Reward> rewards, bool isCard,bool isHealing)
    {
        foreach (Reward reward in rewards)
        {
            AnyReward();
            if (reward.rewardType == 1)
            {
                yield return StartCoroutine(GenerateReward(3, true));
            }
            else if (reward.rewardType == 2)
            {
                yield return StartCoroutine(GenerateReward(3, false));
            }
            else if (reward.rewardType == 3)
            {
                GainHealing(reward.rewardAmount);
            }
            yield return new WaitUntil(() => isGettingReward == false);
        }
        tileScript = tile.GetComponent<Lootable>();
        tileScript.Looted();
    }
    public IEnumerator LevelUpReward()
    {
        AnyReward();
        yield return StartCoroutine(GenerateReward(5, false));
        yield return new WaitUntil(() => isGettingReward == false);
    }
    public void BossReward()
    {
        AnyReward();
        //rewardRarity = tileScript.Raity;
        StartCoroutine(GenerateReward(3, false));
    }
    public void GainHealing(int amount)
    {
        playerControler.Heal(amount);
        GainedReward();
    }
    public IEnumerator RemoveCardInDeck()
    {
        yield return StartCoroutine(deckManager.ChooseCard(deckManager.EntireDeck, (result) => deckManager.DestroyCard(result)));
    }

    private IEnumerator GenerateReward(int numberOfRewards, bool isCard = true)
    {
        isRewardCard = isCard;
        List<GameObject> potentialRewards = new List<GameObject>();
        for (int i = 0; i < numberOfRewards; i++)
        {
            float randomProbability = UnityEngine.Random.Range(0, 1);
            List<GameObject> currentRewardPool = new List<GameObject>();
            if (randomProbability <= commonProbability)
            {
                rewardRarity = 1;
                if (isCard)
                {
                    currentRewardPool = new List<GameObject>(commonCardRewards);
                }
                else
                {
                    currentRewardPool = new List<GameObject>(commonRelicRewards);
                }
            }
            else if (randomProbability <= commonProbability + uncommonProbability)
            {
                rewardRarity = 2;
                if (isCard)
                {
                    currentRewardPool = new List<GameObject>(uncommonCardRewards);
                }
                else
                {
                    currentRewardPool = new List<GameObject>(uncommonRelicRewards);
                }
            }
            else
            {
                rewardRarity = 3;
                if (isCard)
                {
                    currentRewardPool = new List<GameObject>(rareCardRewards);
                }
                else
                {
                    currentRewardPool = new List<GameObject>(rareRelicRewards);
                }

            }
            foreach (GameObject reward in potentialRewards)
            {
                if (currentRewardPool.Contains(reward))
                {
                    currentRewardPool.Remove(reward);
                }
            }
            potentialRewards.Add(currentRewardPool[UnityEngine.Random.Range(0, currentRewardPool.Count)]);

        }
        currentOptionsPrefabs = potentialRewards;
        foreach (GameObject reward in potentialRewards)
        {
            GameObject createdReward = Instantiate(reward, rewardsLocation.transform);
            createdReward.AddComponent<IsReward>();
            currentOptions.Add(createdReward);
            if (isCard)
            {
                yield return StartCoroutine(createdReward.GetComponent<Card>().PrepareCardDiscription());

                //yield return StartCoroutine(createdReward.GetComponent<Card>().FirstSpawned());
            }
        }
        deckManager.SeperateCards(currentOptions, rewardsLocation.transform.position, relativeSpaceBetweenRewardCards);
        yield return null;
    }
    public IEnumerator RewardSelected(GameObject reward)
    {
        //Debug.Log(reward + " selected");
        Destroy(reward.GetComponent<IsReward>());
        if (isRewardCard)
        {
            yield return StartCoroutine(deckManager.GainCard(reward));
        }
        else
        {
            if (reward.GetComponent<Relic>().IsUnique)
            {
                Relic rewardScript = reward.GetComponent<Relic>();
                //GameObject originalRelic = relicRewardsLists[rewardScript.Rarity - 1].Find(obj => obj.GetComponent<Relic>().RelicName == rewardScript.RelicName);
                relicRewardsLists[rewardScript.Rarity - 1].Remove(currentOptionsPrefabs[currentOptions.IndexOf(reward)]);

                //if (originalRelic != null)
                //{
                //}
            }
            yield return StartCoroutine(relicManager.GainRelic(reward));
        }
        currentOptions.Remove(reward);
        foreach (GameObject unselectedReward in currentOptions)
        {
            Destroy(unselectedReward);
        }
        currentOptions.Clear();
        GainedReward();
    }
}
