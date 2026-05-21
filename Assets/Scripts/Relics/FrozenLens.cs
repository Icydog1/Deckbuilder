using System.Collections;
using System.Text.RegularExpressions;

public class FrozenLens : Relic
{
    public override void Awake()
    {
        relicDesription = "At the start of your turn the closest enemy loses <color=#009f9f>" + Variables.frozenLensSpeedLoss + "<color=white> speed";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Finesse(2, -1)), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
        if (relicDescriptionList != null && relicDescriptionList.Count > 0)
        {
            relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
        }

    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Finesse(2, -1)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());

    }
}
