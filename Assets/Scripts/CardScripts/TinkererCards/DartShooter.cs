using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartShooter : Card
{
    public DartShooter() : base(0, 1, 0) { }
    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Attack(11,3)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Move(6)));

    }
}