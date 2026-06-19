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
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainTopEnergy(-2) }))));

    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(32)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.GainBottomEnergy(-1) }))));
    }
}
