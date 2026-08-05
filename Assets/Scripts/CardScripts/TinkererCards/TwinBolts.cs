using System.Collections;
using UnityEngine;

 public class TwinBolts : Card
{
    public TwinBolts() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(8, 4, 1, 2)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(11)));
        currentActions.Add(new Action(() => playerControler.Move(11)));
    }
}