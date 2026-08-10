using System;
using System.Collections;
using System.Collections.Generic;

public class CommandingPresence : Card
{
    public CommandingPresence() : base(2, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 4, Var.infinityValue)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(20)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(2, new List<Action>() { new Action(() => playerControler.Command("enemy", 4, 1)), new Action((currentTarget) => currentTarget.Move(1, isVariable: true)) }), 4))));
        //currentActions.Add(new Action(() => playerControler.ApplyCondition(new GainAbility(new Ability(2, new List<Action>() { new Action(() => playerControler.Command(1, "enemy", 4)), new Action(() => playerControler.Move(1, isVariable: true)) }), 4))));

    }
}