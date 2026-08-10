using System.Collections;
using UnityEngine;

 public class CorruptedPower : WizardCard
{
    public CorruptedPower() : base(2, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 3, 1)));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Strength(20, 1))));
        currentActions.Add(new Action((currentTarget) => currentTarget.LoseHealthAction(25)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainMana(30)));
        currentActions.Add(new Action(() => playerControler.Command("enemy", 10, Var.infinityValue)));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Strength(20, 1))));
    }
}