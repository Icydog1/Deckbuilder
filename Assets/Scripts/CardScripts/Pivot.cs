using System;
using System.Collections;
using UnityEngine;
public class Pivot : Card
{
    public Pivot() : base(1, 1, 1) { }



    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Draw(3)));
        currentActions.Add(new Action(() => playerControler.Discard(1)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(28,true)));
        currentActions.Add(new Action(() => playerControler.Exhausting(2)));


    }
}
