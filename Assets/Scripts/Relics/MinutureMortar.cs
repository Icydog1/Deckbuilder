using System.Collections;
using UnityEngine;

public class MinutureMortar : Relic
{
    protected override int rarity => 3;
    public override void Awake()
    {
        relicDesription = "Attacks deal " + Variables.relicIncreaseableNumberColor + Variables.mortarDamageIncrease + "</color> extra damage for each space between you and the target";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.MortarCount++;
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        playerControler.MortarCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

