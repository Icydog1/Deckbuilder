using System.Collections;
using UnityEngine;

 public class QuickStab : Card
{
    public QuickStab() : base(1, 0, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(10)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(25)));

    }
}
