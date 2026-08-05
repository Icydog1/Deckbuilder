using System.Collections;
using System.Text.RegularExpressions;

public class ToxicTentacle : Relic
{
    protected override int rarity => 2;
    public override void Awake()
    {
        relicDesription = "At the end of your turn apply " + Var.relicIncreaseableNumberColor + Var.toxicTentaclePoison + "</color> poison <sprite name=Target>all <sprite name=Range>1";
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
        yield return StartCoroutine(playerControler.ApplyCondition(new Poison(count * Var.toxicTentaclePoison), "enemy", 1, Var.infinityValue));
    }

    public void OnDestroy()
    {
        playerControler.PlayerTurnEnded -= EndOfTurnPoison;
    }
}
