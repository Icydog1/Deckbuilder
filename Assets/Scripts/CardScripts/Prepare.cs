using System;
using System.Collections;
using System.Collections.Generic;

public class Prepare : Card
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
        currentActions.Add(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainTopEnergy(2), () => playerControler.Draw(1) })));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainBottomEnergy(2), () => playerControler.Draw(1) })));


    }
}
