using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class Strike : Card
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

        yield return StartCoroutine(actionManager.PreformAction(playerControler.Attack(10)));

    }

    public override IEnumerator PrepareBottom()
    {

        yield return StartCoroutine(actionManager.PreformAction(playerControler.Move(10)));

    }
}
