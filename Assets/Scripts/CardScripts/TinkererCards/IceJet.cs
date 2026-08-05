using System.Collections;
using UnityEngine;

 public class IceJet : Card
{
    public IceJet() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Speed(-14,1), "enemy",5)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(14)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Speed(8, 1), "any", 7, Var.infinityValue)));
    }
}