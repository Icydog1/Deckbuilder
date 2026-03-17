using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private PlayerControler playerControler;

    private List<GameObject> relicRewards;

    private List<GameObject> allCardRewards;
    private List<GameObject> commonCardRewards;
    private List<GameObject> uncommonCardRewards;
    private List<GameObject> rareCardRewards;


    private Lootable tileScript;
    private int rewardRarity;
    float commonProbability = 0.8f;
    float uncommonProbability = 0.15f;
    float rareProbability = 0.05f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AnyReward()
    {
        playerControler.GettingReward = true;
        playerControler.UpdatePlayer();
    }

    public void TileReward(GameObject tile)
    {
        AnyReward();
        tileScript = tile.GetComponent<Lootable>();
        //rewardRarity = tileScript.Raity;

        
    }

    private void GenerateReward()
    {
        float randomProbability = Random.Range(0, 1);
        if (randomProbability < commonProbability)
        {
            rewardRarity = 1;
        }
        else if (randomProbability < commonProbability + uncommonProbability)
        {
            rewardRarity = 2;
        }
        else
        {
            rewardRarity = 3;
        }


    }
    public void RewardSelected(GameObject reward)
    {

    }
}
