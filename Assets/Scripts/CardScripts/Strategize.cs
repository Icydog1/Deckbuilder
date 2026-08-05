using System;
using System.Collections;
using System.Collections.Generic;

public class Strategize : Card
{
    public Strategize() : base(3, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.GainBottomEnergy(2)));
    }

    public override void PrepareBottom()
    {
        for (int i = 0; i < 4; i++)
        {
            currentActions.Add(new Action(() => playerControler.Skill(3)));
        }
    }
}