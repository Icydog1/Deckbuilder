using System.Collections;
public class PerfectCrystal : Relic
{
    protected override int rarity => 1;
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new ManaCapacity(5, -1,false)), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.ApplyCondition(new ManaCapacity(5, -1, false)), relicDescriptionList));
        yield return StartCoroutine(base.IncreaseCount());
    }
}