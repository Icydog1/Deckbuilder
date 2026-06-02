using System;
using System.Collections;
using System.Collections.Generic;

public class ShieldedStrike : Card
{
    protected override int rarity => 2;
    public override void Start()
    {
        topCost = 1;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add(() => playerControler.Block(10));
        currentActions.Add(() => playerControler.Attack(10));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Ability(10));
        currentActions.Add(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Block(1, true) }))));
        
    }
}
