using System.Collections;
using System.Text.RegularExpressions;

public class ShatteredSword : Relic
{
    protected override int rarity => 1;
    public override void Awake()
    {
        relicDesription = "At the start of your turn the closest enemy loses " + Var.relicIncreaseableNumberColor + Var.shatteredSwordStrengthLoss + "</color> strength this turn";
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.PlayerTurnStarted += StartOfTurnWeaken;
        yield return StartCoroutine(base.OnGain());

    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());

    }
    public IEnumerator StartOfTurnWeaken(PlayerControler playerControler)
    {
        yield return StartCoroutine(playerControler.ApplyCondition(new Strength(-count * Var.shatteredSwordStrengthLoss, 1), "enemy", -1, 1, false, true));
    }

    public void OnDestroy()
    {
        playerControler.PlayerTurnStarted -= StartOfTurnWeaken;
    }
}
