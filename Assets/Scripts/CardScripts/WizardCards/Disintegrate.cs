using System.Collections;
using UnityEngine;

public class Disintegrate : WizardCard
{
    public Disintegrate() : base(3, 2, 1, 30) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(100, 5)));


    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainMana(21)));
    }
}