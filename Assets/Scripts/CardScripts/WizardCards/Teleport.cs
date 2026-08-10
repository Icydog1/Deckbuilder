using System.Collections;
using UnityEngine;

 public class Teleport : WizardCard
{
    public Teleport() : base(2, 1, 1, 10, 10) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Command("enemy", 4, 1)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Move(30,true)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(30,true)));
    }
}