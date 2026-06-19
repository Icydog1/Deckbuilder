using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private GameObject[] allCards = new GameObject[] { };

    [SerializeField]
    private List<GameObject> allCardRewards = new List<GameObject>();
    public List<GameObject> AllCards { get { return allCardRewards; } }

    private List<List<GameObject>> cardRewardsLists = new List<List<GameObject>>();
    private List<GameObject> customCardRewards = new List<GameObject>();
    private List<GameObject> commonCardRewards = new List<GameObject>();
    private List<GameObject> uncommonCardRewards = new List<GameObject>();
    private List<GameObject> rareCardRewards = new List<GameObject>();

    private GameObject[] allRelics = new GameObject[] { };

    [SerializeField]
    private List<GameObject> allRelicRewards = new List<GameObject>();
    public List<GameObject> AllRelics{ get { return allRelicRewards; } }

    private List<List<GameObject>> relicRewardsLists = new List<List<GameObject>>();
    private List<GameObject> customRelicRewards = new List<GameObject>();
    private List<GameObject> commonRelicRewards = new List<GameObject>();
    private List<GameObject> uncommonRelicRewards = new List<GameObject>();
    private List<GameObject> rareRelicRewards = new List<GameObject>();

    private List<GameObject> currentOptions = new List<GameObject>();
    private List<GameObject> currentOptionsPrefabs = new List<GameObject>();

    private Lootable tileScript;
    private int rewardRarity;


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

        allCards = Resources.LoadAll<GameObject>("Prefabs/Cards");
        allCardRewards = allCards.ToList();
        allCardRewards.RemoveAll(card => card.name == "BaseCard");
        allCardRewards.Sort((card1, card2) => card1.GetComponent<Card>().Rarity.CompareTo(card2.GetComponent<Card>().Rarity));

        cardRewardsLists.Add(customCardRewards);
        cardRewardsLists.Add(commonCardRewards);
        cardRewardsLists.Add(uncommonCardRewards);
        cardRewardsLists.Add(rareCardRewards);

        allRelics = Resources.LoadAll<GameObject>("Prefabs/Relics");
        allRelicRewards = allRelics.ToList();
        allRelicRewards.RemoveAll(relic => relic.name == "BaseRelic");
        allRelicRewards.Sort((relic1, relic2) => relic1.GetComponent<Relic>().Rarity.CompareTo(relic2.GetComponent<Relic>().Rarity));

        relicRewardsLists.Add(customRelicRewards);
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

        //if (commonProbability + uncommonProbability + rareProbability != 1f)
        //{
        //    Debug.Log("reward probabilitys dont add to 1");
        //}
        foreach (GameObject card in allCardRewards)
        {
            int cardRarity = card.GetComponent<Card>().Rarity;
            if (cardRarity > 0)
            {
                cardRewardsLists[cardRarity].Add(card);
            }
            else
            {
                customCardRewards.Add(card);
            }
        }
        //if (testCardRewards.Count > 0)
        //{
        //    commonCardRewards = testCardRewards;
        //}
        foreach (GameObject relic in allRelicRewards)
        {
            int relicRarity = relic.GetComponent<Relic>().Rarity;
            if (relicRarity > 0)
            {
                relicRewardsLists[relicRarity].Add(relic);
            }
            else
            {
                customRelicRewards.Add(relic);
            }

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


    public IEnumerator TileReward(GameObject tile, List<Reward> rewards)
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
    public IEnumerator QuestReward(List<Reward> rewards)
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
            float randomProbability = UnityEngine.Random.Range(0, 1f);
            //Debug.Log(randomProbability);
            List<GameObject> currentRewardPool = new List<GameObject>();
            if (isCard)
            {
                if (randomProbability <= Variables.commonCardProbability)
                {
                    rewardRarity = 1;
                    currentRewardPool = new List<GameObject>(commonCardRewards);
                }
                else if (randomProbability <= Variables.commonCardProbability + Variables.uncommonCardProbability)
                {
                    rewardRarity = 2;
                    currentRewardPool = new List<GameObject>(uncommonCardRewards);

                }
                else
                {
                    rewardRarity = 3;
                    currentRewardPool = new List<GameObject>(rareCardRewards);
                }
            }
            else
            {
                if (randomProbability <= Variables.commonRelicProbability)
                {
                    rewardRarity = 1;
                    currentRewardPool = new List<GameObject>(commonRelicRewards);

                }
                else if (randomProbability <= Variables.commonRelicProbability + Variables.uncommonRelicProbability)
                {
                    rewardRarity = 2;
                    currentRewardPool = new List<GameObject>(uncommonRelicRewards);

                }
                else
                {
                    rewardRarity = 3;
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
                relicRewardsLists[rewardScript.Rarity].Remove(currentOptionsPrefabs[currentOptions.IndexOf(reward)]);

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
    public void RewardSkiped()
    {
        //Debug.Log(reward + " selected");
        foreach (GameObject unselectedReward in currentOptions)
        {
            Destroy(unselectedReward);
        }
        currentOptions.Clear();
        GainedReward();
    }
}
