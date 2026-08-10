using System.Collections;
using UnityEngine;

 public class Madness : WizardCard
{
    public Madness() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 3, 1)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Attack(14,1,Var.infinityValue, targetType: "friendly")));
    }

    public override void PrepareBottom()
    {
        //replace with summon eldrich monster thing
        currentActions.Add(new Action(() => playerControler.Skill(31)));
        currentActions.Add(new Action(() => playerControler.TakeDamageAction(8)));


    }
}