using System;
using System.Collections;
using System.Collections.Generic;

public class Prepare : Card
{
    public Prepare() : base(1, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnTopEnergy(2)), "Next turn gain 2 top energy"));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnBottomEnergy(2)), "Next turn gain 2 bottom energy"));
    }
}


