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


    public override void PrepareTop()
    {
        //yield return StartCoroutine(actionManager.PreformAction(playerControler.Block(10)));
        currentActions.Add(playerControler.Block(10));
    }

    public override void PrepareBottom()
    {
        //QueueAction(playerControler.Ability(10));

        currentActions.Add(playerControler.Ability(10));
    }
}