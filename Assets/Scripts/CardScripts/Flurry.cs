using System;
using System.Collections;
using System.Collections.Generic;

public class Flurry : Card
{
    public Flurry() : base(3, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(3,1,1,4)));

    }

    public override void PrepareBottom()
    {
        for (int i = 0; i < 4; i++)
        {
            currentActions.Add( new Action(() => playerControler.Skill(3)));
        }
    }
}