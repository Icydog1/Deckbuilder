using System.Collections;
using UnityEngine;

 public class TestCard2 : Card
{    public override void Start()
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
        PrepareAttack(3,4);
    }

    public override void PrepareBottom()
    {
        PrepareMove(3);
    }
}
