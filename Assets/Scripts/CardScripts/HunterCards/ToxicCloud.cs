using System;
using System.Collections;

public class ToxicCloud : Card
{
    public ToxicCloud() : base(2, 2, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition( new Poison(11), "enemy",3,Var.infinityValue)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.ApplyCondition(new Poison(4), "enemy", 4, Var.infinityValue) },3))));
    }
}
