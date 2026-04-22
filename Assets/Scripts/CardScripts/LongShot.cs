using System.Collections;
using UnityEngine;

 public class LongShot : Card
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
        currentActions.Add(() => playerControler.Attack(15, 4));

    }

    public override void PrepareBottom()
    {
        currentActions.Add(() => playerControler.Ability(20));
    }
}
