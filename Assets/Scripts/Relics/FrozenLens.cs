using System.Collections;
using System.Text.RegularExpressions;

public class FrozenLens : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "At the start of your turn the closest enemy loses " + Variables.relicIncreaseableNumberColor + Variables.frozenLensSpeedLoss + "</color> speed this turn";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        PlayerControler.PlayerTurnStarted += StartOfTurnSlow;

        //yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new StartOfTurnSlow(Variables.frozenLensSpeedLoss, -1)), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
        
        //if (relicDescriptionList != null && relicDescriptionList.Count > 0)
        //{
        //    relicDescriptionList[0] = Regex.Replace(relicDescriptionList[0], "(. )([0-9]+)( .)", "$1<color=#009f9f>$2<color=white>$3");
        //}

    }
    public override IEnumerator IncreaseCount()
    {
        //yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new StartOfTurnSlow(Variables.frozenLensSpeedLoss, -1)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());

    }
    public IEnumerator StartOfTurnSlow(PlayerControler playerControler)
    {
        yield return StartCoroutine(playerControler.ApplyCondition(new Speed(-count * Variables.frozenLensSpeedLoss, 1), "enemy", -1, 1, false, false));
    }

    public void OnDestroy()
    {
        PlayerControler.PlayerTurnStarted -= StartOfTurnSlow;
    }

}
