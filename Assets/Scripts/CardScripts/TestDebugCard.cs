using System.Collections;
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
        topActions.Add(() => playerControler.ApplyCondition(new Strength(1, 1)));
        topActions.Add(() => playerControler.ApplyCondition(new Dexterity(1, 1), "ally", 4, 1));


    }

    public override void PrepareBottom()
    {
        bottomActions.Add(() => playerControler.ApplyCondition(new Speed(10, -1), "enemy", 4, 1, true));
        bottomActions.Add(() => playerControler.ApplyConditions(new Condition[]{ new Finesse(5, 5)}, "self and ally", 2, 2));

    }
}
