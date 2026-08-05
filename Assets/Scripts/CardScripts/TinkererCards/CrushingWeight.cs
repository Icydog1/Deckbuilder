using System;
using System.Collections;
using System.Collections.Generic;

public class CrushingWeight : Card
{
    public CrushingWeight() : base(3, 1, 1) { }


    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(3,1,1,4)));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(20,true)));
        currentActions.Add(new Action(() => playerControler.Attack(5,1,Var.infinityValue)));
        //for (int i = 0; i < 4; i++)
        //{
        //    currentActions.Add( new Action(() => playerControler.Skill(3)));
        //}
    }
}