using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Wish : WizardCard
{
    public Wish() : base(3, 1, 1) { }
    public override void PrepareTop()
    {

        currentActions.Add(new Action(() => playerControler.Attack(50,10,Var.infinityValue)));
        currentActions.Add(new Action(() => playerControler.TakeDamageAction(20)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainTopEnergy(5)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Strength(-100,1))));
    }
}