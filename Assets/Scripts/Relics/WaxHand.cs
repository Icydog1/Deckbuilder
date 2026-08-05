using System.Collections;
using UnityEngine;

public class WaxHand : Relic
{
    protected override int rarity => 2;
    public override void Awake()
    {
        relicDesription = "Keep up to " + Var.relicIncreaseableNumberColor + Var.waxHandRetainedBlock + "</color> block between turns";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.WaxHandCount++;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        playerControler.WaxHandCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

