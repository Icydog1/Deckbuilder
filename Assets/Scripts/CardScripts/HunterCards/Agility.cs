using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agility : Card
{
    public Agility() : base(0, 0, 1) { }
    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Skill(6)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.Attack(10)));

    }
}