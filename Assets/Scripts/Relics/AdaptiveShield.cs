using System.Collections;
using System.Text.RegularExpressions;
using Unity.VisualScripting;

public class AdaptiveShield : Relic
{
    public override void Awake()
    {
        relicDesription = "Whenever you lose HP gain <color=#009f9f>" + Variables.adaptiveShieldBlock + "<color=white> block next turn";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.AdaptiveShieldCount++;
        yield return StartCoroutine(base.OnGain());
        if (relicDescriptionList != null && relicDescriptionList.Count > 0)
        {
            relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
        }
    }
    public override IEnumerator IncreaseCount()
    {
        playerControler.AdaptiveShieldCount++;
        yield return StartCoroutine(base.IncreaseCount());

    }
}

