using System.Collections;
using UnityEngine;

 public class QuickStab : Card
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


    public override IEnumerator PrepareTop()
    {
        currentActions.Add(() => playerControler.Attack(10));
    }

    public override IEnumerator PrepareBottom()
    {
        currentActions.Add(() => playerControler.Move(25));

    }
}
