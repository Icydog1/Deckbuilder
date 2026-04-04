using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

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
    private List<GameObject> commonCardRewards = new List<GameObject>();
    private List<GameObject> uncommonCardRewards = new List<GameObject>();
    private List<GameObject> rareCardRewards = new List<GameObject>();


    [SerializeField]
    private List<GameObject> allRelicRewards = new List<GameObject>();
    private List<GameObject> commonRelicRewards = new List<GameObject>();
    private List<GameObject> uncommonRelicRewards = new List<GameObject>();
    private List<GameObject> rareRelicRewards = new List<GameObject>();



    private List<GameObject> currentOptions = new List<GameObject>();

    private Lootable tileScript;
    private int rewardRarity;
    private float commonProbability = 1f;// 0.8f;
    private float uncommonProbability = 0f; //0.15f;
    private float rareProbability = 0f; //0.05f;

    private float relativeSpaceBetweenRewardCards = 0.5f;

    private bool isRewardCard;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();

        rewardsLocation = GameObject.Find("Rewards");

        if (commonProbability + uncommonProbability + rareProbability != 1f)
        {
            Debug.Log("reward probabilitys dont add to 1");
        }

        foreach (GameObject card in allCardRewards)
        {
            int cardRarity = card.GetComponent<Card>().Rarity;
            if (cardRarity == 1)
            {
                commonCardRewards.Add(card);
            }
            else if (cardRarity == 2)
            {
                uncommonCardRewards.Add(card);
            }
            else
            {
                rareCardRewards.Add(card);
            }
        }
        foreach (GameObject relic in allRelicRewards)
        {
            int relicRarity = relic.GetComponent<Relic>().Rarity;
            if (relicRarity == 1)
            {
                commonRelicRewards.Add(relic);
            }
            else if (relicRarity == 2)
            {
                uncommonRelicRewards.Add(relic);
            }
            else
            {
                rareRelicRewards.Add(relic);
            }
        }

    }
    void Start()
    {
        //GameManager.GameStarted += InitialReward;

        isRewardCard = true;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AnyReward()
    {
        playerControler.GettingReward = true;
        uIManager.IsGettingReward = true;
        //playerControler.UpdatePlayer();
    }
    public void InitialReward(GameManager gameManager)
    {
        AnyReward();
        GenerateReward(3);

    }


    public void TileReward(GameObject tile)
    {
        AnyReward();
        tileScript = tile.GetComponent<Lootable>();
        tileScript.Looted();
        //rewardRarity = tileScript.Raity;

        GenerateReward(3);
    }
    public void BossReward()
    {
        AnyReward();
        //rewardRarity = tileScript.Raity;
        GenerateReward(3, false);
    }
    private void GenerateReward(int numberOfRewards, bool isCard = true)
    {
        isRewardCard = isCard;
        List<GameObject> potentialRewards = new List<GameObject>();
        for (int i = 0; i < numberOfRewards; i++)
        {
            float randomProbability = Random.Range(0, 1);
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
            foreach (GameObject reward in currentOptions)
            {
                if (currentRewardPool.Contains(reward))
                {
                    currentRewardPool.Remove(reward);
                }
            }
            potentialRewards.Add(currentRewardPool[Random.Range(0, currentRewardPool.Count)]);

        }
        foreach (GameObject reward in potentialRewards)
        {
            GameObject createdReward = Instantiate(reward, rewardsLocation.transform);
            createdReward.AddComponent<IsReward>();
            currentOptions.Add(createdReward);
        }
        deckManager.SeperateCards(currentOptions, rewardsLocation.transform.position, relativeSpaceBetweenRewardCards);

    }
    public void RewardSelected(GameObject reward)
    {
        Debug.Log(reward + " selected");
        Destroy(reward.GetComponent<IsReward>());
        if (isRewardCard)
        {
            deckManager.GainCard(reward);

        }
        else
        {
            relicManager.GainRelic(reward);
        }
        currentOptions.Remove(reward);
        foreach (GameObject unselectedReward in currentOptions)
        {
            Destroy(unselectedReward);
        }
        currentOptions.Clear();
        playerControler.GettingReward = false;
        uIManager.IsGettingReward = false;
    }
}
