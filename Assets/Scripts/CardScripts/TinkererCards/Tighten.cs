using System;
using System.Collections;
using System.Collections.Generic;

public class Tighten : Card
{
    public Tighten() : base(2, 1, 2) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(27,2)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(5, new List<Action>() { new Action(() => playerControler.ApplyCondition(new Accuracy(1,1), isVariable: true)) }), 4))));
    }
}