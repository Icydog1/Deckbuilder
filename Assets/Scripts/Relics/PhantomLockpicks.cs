using System;
using System.Collections;
using System.Collections.Generic;

public class PhantomLockpicks : Relic
{
    protected override int rarity => 2;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: 1 <sprite name=Skill> for 2 <sprite name=Lockpick>(Max " + Variables.relicIncreaseableNumberColor + Variables.phantomLockpicksMaxTimes + "x</color>)";
        ability = new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Lockpick(2, true) }, Variables.phantomLockpicksMaxTimes);
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
        ability.MaxTimes = Variables.phantomLockpicksMaxTimes * count;
    }
}
