using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WindscarSandals : Relic
{
    public override void Awake()
    {
        relicName = "SpeedRelic";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
		yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Speed(2, -1)), relicDescriptionList));

		yield return StartCoroutine(base.OnGain());
		if (relicDescriptionList != null && relicDescriptionList.Count > 0)
		{
			relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
		}
	}
    public override IEnumerator IncreaseCount()
    {
		yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Speed(2, -1)), relicDescriptionList));

		yield return StartCoroutine(base.IncreaseCount());
    }
}
