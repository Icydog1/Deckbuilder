using System.Collections;
using UnityEngine;

public class TestStartCard2 : Card
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
        playerControler.Block(10);
    }

    public override void PrepareBottom()
    {
        playerControler.Lockpick(10);
    }
}