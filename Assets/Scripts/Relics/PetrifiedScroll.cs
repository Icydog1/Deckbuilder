using System;
using System.Collections;
using System.Collections.Generic;

public class PetrifiedScroll : Relic
{
    protected override int rarity => 2;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: " + Variables.petrifiedScrollCost  + " <sprite name=Ability> to draw 1 card(Max " + Variables.relicIncreaseableNumberColor + Variables.petrifiedScrollMaxTimes + "x</color>)";
        ability = new Ability(Variables.petrifiedScrollCost, new List<Func<IEnumerator>>() { () => playerControler.Draw(1, true) }, Variables.petrifiedScrollMaxTimes);
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(ability), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());
    }
    public override IEnumerator IncreaseCount()
    {
        yield return StartCoroutine(base.IncreaseCount());
        ability.MaxTimes = Variables.petrifiedScrollMaxTimes * count;
    }
}
