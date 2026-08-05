using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : Card
{
    public Slide() : base(0, 1, 1) { }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Block(7)));
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Speed(6, 1))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(16)));

    }
}