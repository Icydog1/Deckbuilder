using System.Collections;
using System.Text.RegularExpressions;

public class KineticBattery : Relic
{
    public override void Awake()
    {
        relicDesription = "Every 3 spaces you move gain <color=#009f9f>1<color=white> Vigor";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.KineticBatteryCount++;
        yield return StartCoroutine(base.OnGain());
		if (relicDescriptionList != null && relicDescriptionList.Count > 0)
		{
			relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
		}
	}
    public override IEnumerator IncreaseCount()
    {
        playerControler.KineticBatteryCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

