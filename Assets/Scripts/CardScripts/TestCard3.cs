using System.Collections;
using UnityEngine;

 public class TestCard3 : Card
{
    public override void Start()
    {
        topCost = 0;
        bottomCost = 1;
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }


    public override void PrepareTop()
    {
        PrepareAttack(2);
    }

    public override void PrepareBottom()
    {
        PrepareMove(5);
    }
}
