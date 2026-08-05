using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Lootable;

public class RewardManager : MonoBehaviour
{
    private PlayerControler playerControler;
    private DeckManager deckManager;
    private GameManager gameManager;
    private GameObject rewardsLocation;
    private UIManager uIManager;
    private RelicManager relicManager;

    private GameObject[] allCardPrefabs;
    private GameObject[] hunterCards, tinkererCards, wizardCards;

    //[SerializeField]
    private List<GameObject> allCardRewards = new List<GameObject>();
    public List<GameObject> AllCardRewards { get { return allCardRewards; } }
    private List<GameObject> allCards = new List<GameObject>();
    public List<GameObject> AllCards { get { return allCards; } }
    private List<GameObject> allHunterCards, allTinkererCards, allWizardCards;
    public List<GameObject> AllHunterCards { get { return allHunterCards; } }
    public List<GameObject> AllTinkererCards { get { return allTinkererCards; } }
    public List<GameObject> AllWizardCards { get { return allWizardCards; } }


    private List<List<GameObject>> cardRewardsLists = new List<List<GameObject>>();
    private List<GameObject> customCardRewards = new List<GameObject>();
    private List<GameObject> commonCardRewards = new List<GameObject>();
    private List<GameObject> uncommonCardRewards = new List<GameObject>();
    private List<GameObject> rareCardRewards = new List<GameObject>();

    private GameObject[] allRelics = new GameObject[] { };
    private GameObject[] generalRelics, hunterRelics, tinkererRelics, wizardRelics;

    //[SerializeField]
    private List<GameObject> allRelicRewards = new List<GameObject>();
    public List<GameObject> AllRelicsRewards { get { return allRelicRewards; } }
    private List<GameObject> allHunterRelics, allTinkererRelics, allWizardRelics;
    public List<GameObject> AllHunterRelics { get { return allHunterRelics; } }
    public List<GameObject> AllTinkererRelics { get { return allTinkererRelics; } }
    public List<GameObject> AllWizardRelics { get { return allWizardRelics; } }

    private List<List<GameObject>> relicRewardsLists = new List<List<GameObject>>();
    private List<GameObject> customRelicRewards = new List<GameObject>();
    private List<GameObject> commonRelicRewards = new List<GameObject>();
    private List<GameObject> uncommonRelicRewards = new List<GameObject>();
    private List<GameObject> rareRelicRewards = new List<GameObject>();

    private List<GameObject> currentOptions = new List<GameObject>();
    private List<GameObject> currentOptionsPrefabs = new List<GameObject>();
    [SerializeField]
    private GameObject skipRewardButton;
    private TextMeshProUGUI skipRewardText;
    private Lootable tileScript;
    private int rewardRarity;


    private float relativeSpaceBetweenRewardCards = 0.5f;

    private bool isCardReward, isRelicReward;

    private bool isGettingReward;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerControler = RefrenceStorage.playerControler;
        deckManager = GameObject.Find("DeckManager").GetComponent<DeckManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        relicManager = GameObject.Find("RelicManager").GetComponent<RelicManager>();
        rewardsLocation = GameObject.Find("Rewards");

        //adds lists to big list of rewards
        cardRewardsLists.Add(customCardRewards);
        cardRewardsLists.Add(commonCardRewards);
        cardRewardsLists.Add(uncommonCardRewards);
        cardRewardsLists.Add(rareCardRewards);

        relicRewardsLists.Add(customRelicRewards);
        relicRewardsLists.Add(commonRelicRewards);
        relicRewardsLists.Add(uncommonRelicRewards);
        relicRewardsLists.Add(rareRelicRewards);

        LoadCards();

        LoadRelics();

        skipRewardText = skipRewardButton.transform.Find("SkipRewardText").GetComponent<TextMeshProUGUI>();

        GameManager.ResetGame += ResetState;
        GameManager.GameStartedFunctions += GenerateRewardPools;

        //GenerateRewardPools();

    }
    //loads cards and sorts them into the correct rarities
    public void LoadCards()
    {
        allCardPrefabs = Resources.LoadAll<GameObject>("Prefabs/Cards");
        hunterCards = Resources.LoadAll<GameObject>("Prefabs/Cards/HunterCards");
        tinkererCards = Resources.LoadAll<GameObject>("Prefabs/Cards/TinkererCards");
        wizardCards = Resources.LoadAll<GameObject>("Prefabs/Cards/WizardCards");

        allHunterCards = hunterCards.ToList();
        allHunterCards.RemoveAll(card => card.GetComponent<Card>() == null);
        allHunterCards.Sort((card1, card2) =>
        {
            Card card1Script = card1.GetComponent<Card>();
            Card card2Script = card2.GetComponent<Card>();
            // Compare primary value
            int result = card1Script.Rarity.CompareTo(card2Script.Rarity);
            if (result == 0)
            {
                result = card1Script.name.CompareTo(card2Script.name);
            }
            return result;
        });

        allTinkererCards = tinkererCards.ToList();
        allTinkererCards.RemoveAll(card => card.GetComponent<Card>() == null);
        allTinkererCards.Sort((card1, card2) =>
        {
            Card card1Script = card1.GetComponent<Card>();
            Card card2Script = card2.GetComponent<Card>();
            // Compare primary value
            int result = card1Script.Rarity.CompareTo(card2Script.Rarity);
            if (result == 0)
            {
                result = card1Script.name.CompareTo(card2Script.name);
            }
            return result;
        });
        allWizardCards = wizardCards.ToList();
        allWizardCards.RemoveAll(card => card.GetComponent<Card>() == null);
        allWizardCards.Sort((card1, card2) =>
        {
            Card card1Script = card1.GetComponent<Card>();
            Card card2Script = card2.GetComponent<Card>();
            // Compare primary value
            int result = card1Script.Rarity.CompareTo(card2Script.Rarity);
            if (result == 0)
            {
                result = card1Script.name.CompareTo(card2Script.name);
            }
            return result;
        });



    }
    //loads relics and sorts them into the correct rarities

    public void LoadRelics()
    {
        allRelics = Resources.LoadAll<GameObject>("Prefabs/Relics");
        generalRelics = Resources.LoadAll<GameObject>("Prefabs/Relics/GeneralRelics");
        hunterRelics = Resources.LoadAll<GameObject>("Prefabs/Relics/HunterRelics");
        tinkererRelics = Resources.LoadAll<GameObject>("Prefabs/Relics/TinkererRelics");
        wizardRelics = Resources.LoadAll<GameObject>("Prefabs/Relics/WizardRelics");
        allRelicRewards = allRelics.ToList();


        allHunterRelics = hunterRelics.ToList();
        allHunterRelics.AddRange(generalRelics);
        allHunterRelics.RemoveAll(relic => relic.GetComponent<Relic>() == null);
        allHunterRelics.Sort((relic1, relic2) =>
        {
            Relic relic1Script = relic1.GetComponent<Relic>();
            Relic relic2Script = relic2.GetComponent<Relic>();
            // Compare primary value
            int result = relic1Script.Rarity.CompareTo(relic2Script.Rarity);
            // If primary values are equal, compare secondary value
            if (result == 0)
            {
                result = relic1Script.name.CompareTo(relic2Script.name);
            }
            return result;
        });

        allTinkererRelics = tinkererRelics.ToList();
        allTinkererRelics.AddRange(generalRelics);
        allTinkererRelics.RemoveAll(relic => relic.GetComponent<Relic>() == null);
        allTinkererRelics.Sort((relic1, relic2) =>
        {
            Relic relic1Script = relic1.GetComponent<Relic>();
            Relic relic2Script = relic2.GetComponent<Relic>();
            // Compare primary value
            int result = relic1Script.Rarity.CompareTo(relic2Script.Rarity);
            // If primary values are equal, compare secondary value
            if (result == 0)
            {
                result = relic1Script.name.CompareTo(relic2Script.name);
            }
            return result;
        });

        allWizardRelics = wizardRelics.ToList();
        allWizardRelics.AddRange(generalRelics);
        allWizardRelics.RemoveAll(relic => relic.GetComponent<Relic>() == null);
        allWizardRelics.Sort((relic1, relic2) =>
        {
            Relic relic1Script = relic1.GetComponent<Relic>();
            Relic relic2Script = relic2.GetComponent<Relic>();
            // Compare primary value
            int result = relic1Script.Rarity.CompareTo(relic2Script.Rarity);
            // If primary values are equal, compare secondary value
            if (result == 0)
            {
                result = relic1Script.name.CompareTo(relic2Script.name);
            }
            return result;
        });


    }
    //reset to base when restarting game
    public void ResetState(GameManager gameManager)
    {
        if (isGettingReward)
        {
            ClearUnusedRewards();
        }
    }
    //generate reward pools based on what character is being player
    public void GenerateRewardPools(GameManager gameManager)
    {
        commonCardRewards.Clear();
        uncommonCardRewards.Clear();
        rareCardRewards.Clear();
        commonRelicRewards.Clear();
        uncommonRelicRewards.Clear();
        rareRelicRewards.Clear();
        switch (gameManager.CurrentCharacter.characterName)
        {
            case "Hunter": allCardRewards = allHunterCards; break;
            case "Tinkerer": allCardRewards = allTinkererCards; break;
            case "Wizard": allCardRewards = allWizardCards; break;
            default: Debug.Log("No class type assigned"); allCardRewards = allCards; break;
        }

        switch (gameManager.CurrentCharacter.characterName)
        {
            case "Hunter": allRelicRewards = allHunterRelics; break;
            case "Tinkerer": allRelicRewards = allTinkererRelics; break;
            case "Wizard": allRelicRewards = allWizardRelics; break;
            default: Debug.Log("No class type assigned"); allRelicRewards = allRelics.ToList(); break;
        }
        //allRelicRewards.AddRange(generalRelics);
        //allRelicRewards.RemoveAll(relic => relic.GetComponent<Relic>() == null);
        //allRelicRewards.Sort((relic1, relic2) => relic1.GetComponent<Relic>().Rarity.CompareTo(relic2.GetComponent<Relic>().Rarity));
        //allRelicRewards.Sort((relic1, relic2) =>
        //{
        //    Relic relic1Script = relic1.GetComponent<Relic>();
        //    Relic relic2Script = relic2.GetComponent<Relic>();
        //    // Compare primary value
        //    int result = relic1Script.Rarity.CompareTo(relic2Script.Rarity);
        //    // If primary values are equal, compare secondary value
        //    if (result == 0)
        //    {
        //        result = relic1Script.name.CompareTo(relic2Script.name);
        //    }
        //    return result;
        //});

        //allRelicRewards.RemoveAll(relic => relic.name == "BaseRelic");
        //allRelicRewards.RemoveAll(relic => relic.GetComponent<Relic>().Rarity == -1);
        //allRelicRewards.Sort((relic1, relic2) => relic1.GetComponent<Relic>().Rarity.CompareTo(relic2.GetComponent<Relic>().Rarity));
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
    //basic actions when any reward is generated
    public void AnyReward()
    {
        isGettingReward = true;
        playerControler.GettingReward = true;
        uIManager.IsGettingReward = true;
    }
    //display button for skipible rewards
    public void SkippableReward()
    {
        skipRewardButton.SetActive(true);
        if (isCardReward)
        {
            skipRewardText.text = "Scrap (1XP)";
        }
        else
        {
            skipRewardText.text = "Scrap (Destroy a card)";
        }
    }
    //basic actions when any reward is gained
    public void GainedReward()
    {
        isGettingReward = false;
        playerControler.GettingReward = false;
        uIManager.IsGettingReward = false;
        skipRewardButton.SetActive(false);
    }
    //Generates rewards from a chest
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
                GainHealingReward(reward.rewardAmount);
            }
            yield return new WaitUntil(() => isGettingReward == false);
        }
        tileScript = tile.GetComponent<Lootable>();
        tileScript.Looted();
    }
    //Generates rewards from completing a quest
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
                GainHealingReward(reward.rewardAmount);
            }
            yield return new WaitUntil(() => isGettingReward == false);
        }
    }
    //Generates rewards from leveling up
    public IEnumerator LevelUpReward()
    {
        AnyReward();
        yield return StartCoroutine(GenerateReward(5, false));
        yield return new WaitUntil(() => isGettingReward == false);
    }
    //Generates rewards from killing a boss
    public void BossReward()
    {
        AnyReward();
        //rewardRarity = tileScript.Raity;
        StartCoroutine(GenerateReward(3, false));
    }
    public IEnumerator CustomReward(bool isCard)
    {
        AnyReward();
        yield return StartCoroutine(GenerateReward(3, isCard));
        yield return new WaitUntil(() => isGettingReward == false);
    }
    //reward for healing
    public void GainHealingReward(int amount)
    {
        playerControler.HealDamage(amount);
        GainedReward();
    }
    public IEnumerator RemoveCardInDeck()
    {
        yield return StartCoroutine(deckManager.ChooseCard(deckManager.EntireDeck, (result) => deckManager.DestroyCard(result)));
    }
    //generates relic or card reward options
    public IEnumerator GenerateReward(int numberOfRewards, bool isCard = true)
    {
        isCardReward = isCard;
        isRelicReward = !isCard;
        SkippableReward();
        List<GameObject> potentialRewards = new List<GameObject>();
        for (int i = 0; i < numberOfRewards; i++)
        {
            float randomProbability = UnityEngine.Random.Range(0, 1f);
            //Debug.Log(randomProbability);
            List<GameObject> currentRewardPool = new List<GameObject>();
            if (isCard)
            {
                if (randomProbability <= Var.commonCardProbability)
                {
                    rewardRarity = 1;
                    currentRewardPool = new List<GameObject>(commonCardRewards);
                }
                else if (randomProbability <= Var.commonCardProbability + Var.uncommonCardProbability)
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
                if (randomProbability <= Var.commonRelicProbability)
                {
                    rewardRarity = 1;
                    currentRewardPool = new List<GameObject>(commonRelicRewards);

                }
                else if (randomProbability <= Var.commonRelicProbability + Var.uncommonRelicProbability)
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
    //when a reward is selected
    public IEnumerator RewardSelected(GameObject reward)
    {
        //Debug.Log(reward + " selected");
        Destroy(reward.GetComponent<IsReward>());
        if (isCardReward)
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
        ClearUnusedRewards();
    }
    //when a reward is skipped
    public IEnumerator RewardScrapped()
    {
        //Debug.Log("Scrapped");
        if (isCardReward)
        {
            playerControler.GainXP(Var.ScrappedCardXP);
        }
        else
        {
            yield return StartCoroutine(RemoveCardInDeck());

        }
        ClearUnusedRewards();
    }
    //removes exes reward options when finished
    public void ClearUnusedRewards()
    {
        foreach (GameObject unselectedReward in currentOptions)
        {
            Destroy(unselectedReward);
        }
        currentOptions.Clear();
        GainedReward();
    }

}
