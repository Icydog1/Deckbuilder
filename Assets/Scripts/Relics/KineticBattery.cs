using System.Collections;
using UnityEngine;

public class KineticBattery : Relic
{
    public override void Awake()
    {
        relicDesription = "Every " + Variables.kineticBatterySpaces + " spaces you move gain <color=#009f9f>1<color=white> Vigor for " + Variables.kineticBatteryVigorDuration + " turns";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.KineticBatteryCount++;
        yield return StartCoroutine(base.OnGain());
		if (relicDescriptionList != null && relicDescriptionList.Count > 0)
		{
            Debug.Log("Come Back to this");

            //relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
		}
	}
    public override IEnumerator IncreaseCount()
    {
        playerControler.KineticBatteryCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

