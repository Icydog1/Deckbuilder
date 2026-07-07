using System.Collections;
using System.Text.RegularExpressions;

public class ToxicTentacle : Relic
{
    protected override int rarity => 2;
    public override void Awake()
    {
        relicDesription = "At the end of your turn apply " + Variables.relicIncreaseableNumberColor + Variables.toxicTentaclePoison + "</color> poison all" + Variables.targetSprite + " 1" + Variables.rangeSprite;
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        playerControler.PlayerTurnEnded += EndOfTurnPoison;
        yield return StartCoroutine(base.OnGain());

    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());

    }
    public IEnumerator EndOfTurnPoison(PlayerControler playerControler)
    {
        yield return StartCoroutine(playerControler.ApplyCondition(new Poison(count * Variables.toxicTentaclePoison), "enemy", 1, Variables.gameInfinityValue, false, true));
    }

    public void OnDestroy()
    {
        playerControler.PlayerTurnEnded -= EndOfTurnPoison;
    }
}
