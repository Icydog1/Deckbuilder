using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class GlassEye : Relic
{
    protected override int rarity => 3;
    public override void Awake()
    {
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Accuracy(1, -1)), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Accuracy(1, -1)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
