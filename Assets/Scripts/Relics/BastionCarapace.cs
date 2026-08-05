using System.Collections;
public class BastionCarapace : Relic
{
    protected override int rarity => 1;
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Dexterity(2, -1)), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new Dexterity(2, -1)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}
