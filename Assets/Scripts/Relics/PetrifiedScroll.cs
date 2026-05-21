using System;
using System.Collections;
using System.Collections.Generic;

public class PetrifiedScroll : Relic
{
    public override void Awake()
    {
        isUnique = true;
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(new Ability(9, new List<Func<IEnumerator>>() { () => playerControler.Draw(1, true) })), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());

    }
}
