using System.Collections;
using UnityEngine;

 public class Convert : WizardCard
{
    public Convert() : base(1, 0, 2, 20) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.GainTopEnergy(2)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainMana(27)));

    }
}