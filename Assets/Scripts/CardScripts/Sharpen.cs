using System.Collections;
using UnityEngine;

public class Sharpen : Card
{
    public Sharpen() : base(2, 1, 1) { }

    [SerializeField]
    private GameObject clockworkOwl;

    public override void Awake()
    {
        //clockworkOwl = Resources.Load<GameObject>("Prefabs/AllFigures/PlayerSummons/ClockworkOwl");
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Strength(5))));
        //currentActions.Add(new Action(() => playerControler.Augment(new Strength(5))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Vigor(20,1))));
    }
}