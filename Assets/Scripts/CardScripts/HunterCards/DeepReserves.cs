using System.Collections;
using UnityEngine;

 public class DeepReserves : Card
{
    public DeepReserves() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.GainTopEnergy(3)));
        currentActions.Add(new Action(() => playerControler.Exhausting(3)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainBottomEnergy(3)));
        currentActions.Add(new Action(() => playerControler.Exhausting(3)));
    }
}
