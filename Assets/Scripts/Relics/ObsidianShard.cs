using System;
using System.Collections;
using System.Collections.Generic;

public class ObsidianShard : Relic
{
    public override void Awake()
    {
        relicName = "Obsidian Shard";
        base.Awake();
    }
    public override void OnGain()
    {
        
        StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.ApplyCondition(new Strength(2, -1)) })), relicDescriptionList));

        base.OnGain();

    }
    public override void IncreaseCount()
    {
        StartCoroutine(actionManager.PreformAction(playerControler.GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.ApplyCondition(new Strength(2, -1)) })), relicDescriptionList));
        base.IncreaseCount();
    }
}
