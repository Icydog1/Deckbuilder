using System;
using System.Collections;
using System.Collections.Generic;

public class BetterLockpicks : Relic
{
    public BetterLockpicks()
    {
        isUnique = true;
    }
    public override void Awake()
    {
        relicName = "Better Lockpicks";
        base.Awake();
    }
    public override void OnGain()
    {
        StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Lockpick(2, true) })), relicDescriptionList));
        base.OnGain();

    }
}
