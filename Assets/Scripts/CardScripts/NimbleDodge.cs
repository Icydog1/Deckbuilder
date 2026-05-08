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


    public override IEnumerator PrepareTop()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.Block(25));
    }

    public override IEnumerator PrepareBottom()
    {
        yield return "Temp";

        currentActions.Add(() => playerControler.Move(20, true));
    }
}
