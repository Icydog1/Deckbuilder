using System.Collections;
using UnityEngine;

public class FlameShield : WizardCard
{
    public FlameShield() : base(2, 1, 1, 15, 9) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(25)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Thorns(12,1))));


    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Attack(14,1,Var.infinityValue)));
    }
}