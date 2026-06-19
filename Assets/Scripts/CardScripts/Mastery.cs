using System;
using System.Collections;
using System.Collections.Generic;

public class Mastery : Card
{
    public Mastery() : base(1, 1, 2) { }

    public override void Start()
    {
        
        base.Start();
    }
    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Attack(1, isVariable: true) }), 3))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Ability(40)));
    }
}