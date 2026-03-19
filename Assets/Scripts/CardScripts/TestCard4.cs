using System.Collections;
using UnityEngine;

public class TestCard4 : Card
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
        PrepareBlock(5);
    }

    public override void PrepareBottom()
    {
        PrepareMove(4,true);
    }
}
