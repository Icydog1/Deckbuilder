using System;
using System.Collections;

public class Echo : Card
{
    public Echo() : base(2, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(8)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.Attack(8) }))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(8)));
        currentActions.Add( new Action(() => playerControler.ApplyCondition(new NextTurns(new Func<IEnumerator>[] { () => playerControler.Move(8) }))));
    }
}