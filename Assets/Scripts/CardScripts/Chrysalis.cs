using System;
using System.Collections;
using System.Collections.Generic;

public class Chrysalis : Card
{
    public Chrysalis() : base(1, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Block(9)));
        currentActions.Add( new Action(() => playerControler.Skill(7)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(1, new List<Func<IEnumerator>>() { () => playerControler.Move(1, isVariable: true) }), 3))));
    }
}