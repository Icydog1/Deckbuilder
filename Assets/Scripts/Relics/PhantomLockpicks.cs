using System;
using System.Collections;
using System.Collections.Generic;

public class PhantomLockpicks : Relic
{
    protected override int rarity => 2;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: <sprite name=Skill>1 for <sprite name=Lockpick>2(Max " + Var.relicIncreaseableNumberColor + Var.phantomLockpicksMaxTimes + "x</color>)";
        ability = new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Lockpick(2, true) }, Var.phantomLockpicksMaxTimes);
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
        ability.MaxTimes = Var.phantomLockpicksMaxTimes * count;
    }
}
