using System;
using System.Collections;
using System.Collections.Generic;

public class PetrifiedScroll : Relic
{
    protected override int rarity => 2;
    private Ability ability;
    public override void Awake()
    {
        relicDesription = "Gain ability: <sprite name=Skill>" + Var.petrifiedScrollCost + " to draw 1 card(Max " + Var.relicIncreaseableNumberColor + Var.petrifiedScrollMaxTimes + "x</color>)";
        ability = new Ability(Var.petrifiedScrollCost, new List<Action>() { new Action(() => playerControler.Draw(1, true)) }, Var.petrifiedScrollMaxTimes);
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
        ability.MaxTimes = Var.petrifiedScrollMaxTimes * count;
    }
}
