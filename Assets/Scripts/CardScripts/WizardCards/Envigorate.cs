using System.Collections;
using UnityEngine;

 public class Envigorate : WizardCard
{
    public Envigorate() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command(1, "enemy", 5)));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Vigor(18))));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Burst(16))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Burst(12,2))));
        currentActions.Add(new Action(() => playerControler.Draw(2)));

    }
}