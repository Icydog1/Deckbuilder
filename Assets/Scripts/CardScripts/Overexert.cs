using System;
using System.Collections;
using UnityEngine;
public class Overexert : Card
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
        currentActions.Add(() => playerControler.Attack(47));
        currentActions.Add(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainTopEnergy(-2) })));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Move(36));
        currentActions.Add(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainBottomEnergy(-1) })));
    }
}
