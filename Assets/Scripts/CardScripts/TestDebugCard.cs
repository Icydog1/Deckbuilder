using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class TestDebugCard : Card
{
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
        currentActions.Add(() => playerControler.ApplyCondition(new Strength(1, 1)));
        currentActions.Add(() => playerControler.ApplyCondition(new Dexterity(1, 1), "ally", 4, 1));


    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.ApplyCondition(new Speed(10, -1), "enemy", 4, 1, true));
        currentActions.Add(() => playerControler.ApplyConditions(new Condition[]{ new Finesse(5, 5)}, "self and ally", 2, 2));
        currentActions.Add(() => playerControler.GainNewAbility(1, new List<System.Action>() { () => playerControler.Move(1, false, true) }));

    }
}
