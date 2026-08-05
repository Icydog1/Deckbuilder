using System.Collections;
using UnityEngine;

public class BoneShard : Relic
{
    protected override int rarity => 3;
    public override void Awake()
    {
        relicDesription = "When you command gain " + Var.relicIncreaseableNumberColor + Var.boneShardBlock + "</color> block";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.CommandedEnemy += GainBlock;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
    }

    public void GainBlock(PlayerControler playerControler)
    {
        actionManager.QueueAction(playerControler.Block(Var.boneShardBlock * count));
    }
    public void OnDestroy()
    {
        playerControler.CommandedEnemy -= GainBlock;
    }
}