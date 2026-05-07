using UnityEngine;

public class DevouringFlame : Relic
{
    public override void Awake()
    {
        relicDesription = "Destroy a card";
        relicName = "Devouring Flame";
        base.Awake();
    }
    public override void OnGain()
    {
        StartCoroutine(GameObject.Find("RewardManager").GetComponent<RewardManager>().RemoveCardInDeck());
        base.OnGain();

    }
    public override void IncreaseCount()
    {
        StartCoroutine(GameObject.Find("RewardManager").GetComponent<RewardManager>().RemoveCardInDeck());
        base.IncreaseCount();
    }
}
