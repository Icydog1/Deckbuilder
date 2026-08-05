using System.Collections;
using UnityEngine;

public class MinutureMortar : Relic
{
    protected override int rarity => 3;
    public override void Awake()
    {
        relicDesription = "Attacks deal " + Var.relicIncreaseableNumberColor + Var.mortarDamageIncrease + "</color> extra damage for each space between you and the target";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.MinutureMortarCount++;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        playerControler.MinutureMortarCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

