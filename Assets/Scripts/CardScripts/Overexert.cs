using System;
using System.Collections;
using UnityEngine;
public class Overexert : Card
{
    public Overexert() : base(1, 1, 1) { }

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
        currentActions.Add( new Action(() => playerControler.Attack(47)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnTopEnergy(-2)), "Next turn lose 2 top energy"));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(32)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurnBottomEnergy(-1)), "Next turn lose 1 bottom energy"));
    }
}
