using System.Collections;
using UnityEngine;

public class Defend : Card
{
    public Defend() : base(0, 1, 1) { }

    public override void Awake()
    {
        cardName = "Defend";
        base.Awake();
    }


    public override void PrepareTop()
    {
        //yield return StartCoroutine(actionManager.PreformAction(playerControler.Block(10)));
        currentActions.Add( new Action(() => playerControler.Block(10)));
    }

    public override void PrepareBottom()
    {
        //QueueAction(playerControler.Skill(10));

        currentActions.Add( new Action(() => playerControler.Skill(10)));
    }
}