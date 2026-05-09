using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Strike : Card
{
    public override void Start()
    {
        topCost = 1;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        currentActions.Add(() => playerControler.Attack(10));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Move(10));
    }
}
