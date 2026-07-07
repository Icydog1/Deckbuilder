using System;
using System.Collections;
using System.Collections.Generic;

public class TwoTailedNewt : Relic
{
    protected override int rarity => 3;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: 1" + Variables.skillSprite + " for 2" + Variables.skillSprite + "(Max " + Variables.relicIncreaseableNumberColor + Variables.twoTailedNewtMaxTimes + "x</color>)";
        ability = new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Skill(2, true) }, Variables.twoTailedNewtMaxTimes);
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
        ability.MaxTimes = Variables.twoTailedNewtMaxTimes * count;
    }
}
