using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Strike : Card
{
    public Strike() : base(0, 1, 1) { }

    public override void Awake()
    {
        cardName = "Strike";
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add( new Action(() => playerControler.Attack(10)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add( new Action(() => playerControler.Move(10)));
    }
}
