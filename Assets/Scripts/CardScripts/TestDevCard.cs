using System.Collections;
using UnityEngine;

public class TestDevCard : Card
{
    public override void Start()
    {
        topCost = 0;
        bottomCost = 0;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        playerControler.Block(1000);
        playerControler.Attack(1000,3);

    }

    public override void PrepareBottom()
    {
        playerControler.Move(1000, false);
    }
}
