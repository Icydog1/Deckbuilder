using System.Collections;
using UnityEngine;

public class Defend : Card
{
    public Defend() : base(0, 1, 1) { }

    public override void Start()
    {
        
        base.Start();
    }

    public override void PrepareTop()
    {
        //yield return StartCoroutine(actionManager.PreformAction(playerControler.Block(10)));
        currentActions.Add( new Action(() => playerControler.Block(10)));
    }

    public override void PrepareBottom()
    {
        //QueueAction(playerControler.Ability(10));

        currentActions.Add( new Action(() => playerControler.Ability(10)));
    }
}