using System.Collections;
using UnityEngine;

 public class PowerShot : Card
{
    public PowerShot() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(21, 3)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(24)));
    }
}
