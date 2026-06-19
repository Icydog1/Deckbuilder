using System;
using System.Collections;
using System.Collections.Generic;

public class ShieldedStrike : Card
{
    public ShieldedStrike() : base(2, 1, 1) { }
    public override void Start()
    {
        
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Block(10)));
        currentActions.Add( new Action(() => playerControler.Attack(10)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Ability(10)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Block(1, true) })))));
        
    }
}
