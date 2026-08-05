using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class EternalPeach : Relic
{

    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "Gain " + Var.relicIncreaseableNumberColor + 10 + "</color> max health";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainMaxHealth(10), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainMaxHealth(10), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
