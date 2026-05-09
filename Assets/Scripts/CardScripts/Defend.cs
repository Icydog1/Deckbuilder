using System.Collections;
using UnityEngine;

public class Defend : Card
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
        yield return StartCoroutine(actionManager.PreformAction(playerControler.Block(10)));
        //currentActions.Add(() => playerControler.Block(10));
    }

    public override IEnumerator PrepareBottom()
    {
        actionManager.QueueAction(playerControler.Ability(10));

        currentActions.Add(() => playerControler.Ability(10));
    }
}