using System;
using System.Collections;
using System.Collections.Generic;

public class DeadlyConcoction : Card
{
    public DeadlyConcoction() : base(2, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Skill(10)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(5, new List<Func<IEnumerator>>() { () => playerControler.ApplyCondition(new Poison(1),"enemy", isVariable: true) }), 2))));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Strength(-10,1), "enemy")));
    }

}