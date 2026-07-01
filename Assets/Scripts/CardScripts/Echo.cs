using System;
using System.Collections;

public class Echo : Card
{
    public Echo() : base(2, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(7)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.Attack(7) }))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(7)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.Move(7) }))));
    }
}