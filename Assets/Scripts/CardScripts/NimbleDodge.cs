using System.Collections;
using UnityEngine;

public class NimbleDodge : Card
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
        currentActions.Add(() => playerControler.Block(25));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Move(20, true));
    }
}
