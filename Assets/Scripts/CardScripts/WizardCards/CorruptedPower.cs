using System.Collections;
using UnityEngine;

 public class CorruptedPower : WizardCard
{
    public CorruptedPower() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command(1, "enemy", 3)));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Strength(20, 1))));
        currentActions.Add(new Action((currentTarget) => currentTarget.LoseHealthAction(25)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainMana(30)));
        currentActions.Add(new Action(() => playerControler.Command(Var.infinityValue, "enemy", 10)));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Strength(20, 1))));
    }
}