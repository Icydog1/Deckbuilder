using UnityEngine;
using System.Collections;


public class DevouringFlame : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Destroy a card";
        relicName = "Devouring Flame";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(GameObject.Find("RewardManager").GetComponent<RewardManager>().RemoveCardInDeck()));
        yield return StartCoroutine(base.OnGain());

    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(GameObject.Find("RewardManager").GetComponent<RewardManager>().RemoveCardInDeck()));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
