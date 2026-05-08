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
        yield return "Temp";

        currentActions.Add(() => playerControler.Attack(10));
    }

    public override IEnumerator PrepareBottom()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.Move(25));

    }
}
