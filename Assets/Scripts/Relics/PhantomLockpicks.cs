using System;
using System.Collections;
using System.Collections.Generic;

public class PhantomLockpicks : Relic
{
    public override void Awake()
    {
        isUnique = true;
        base.Awake();
    }
    public override IEnumerator OnGain()
    {
        yield return StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Lockpick(2, true) })), relicDescriptionList));
        yield return StartCoroutine(base.OnGain());

    }
}
