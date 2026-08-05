using System.Collections;
using UnityEngine;

public class MagicMissle : WizardCard
{
    public MagicMissle() : base(1, 1, 1, 7) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(5, 3)));
        currentActions.Add(new Action(() => playerControler.Attack(5, 3)));
        currentActions.Add(new Action(() => playerControler.Attack(5, 3)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(10,true)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Accuracy(2,1))));
    }
}