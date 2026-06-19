using System;
using System.Collections;
using System.Collections.Generic;

public class Prepare : Card
{
    public Prepare() : base(2, 1, 1) { }
    public override void Start()
    {
        
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnTopEnergy(2)), "Next turn gain 2 top energy"));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnBottomEnergy(2)), "Next turn gain 2 bottom energy"));
    }
}


