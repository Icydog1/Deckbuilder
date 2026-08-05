using System;
using System.Collections;
using System.Collections.Generic;

public class EverlastingFlame : Relic
{
    protected override int rarity => 3;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: <sprite name=Skill>" + Var.everlastingFlameCost + " to gain 1 top energy(Max " + Var.relicIncreaseableNumberColor + Var.everlastingFlameMaxTimes + "x</color>)";
        ability = new Ability(Var.everlastingFlameCost, new List<Func<IEnumerator>>() { () => playerControler.GainTopEnergy(1, true) }, Var.everlastingFlameMaxTimes);
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
        ability.MaxTimes = Var.everlastingFlameMaxTimes * count;
    }
}
