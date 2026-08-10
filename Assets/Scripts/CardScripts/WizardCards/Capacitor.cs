using System.Collections;
using UnityEngine;

 public class Capacitor : WizardCard
{
    public Capacitor() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(14)));
        currentActions.Add(new Action(() => playerControler.Draw(1)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new ManaCapacity(8, 3))));
    }
}