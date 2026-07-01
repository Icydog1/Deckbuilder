using System.Collections;
using UnityEngine;

public class ArtificialWings : Card
{
    public ArtificialWings() : base(2, 2, 1) { }

    [SerializeField]
    private GameObject clockworkOwl;

    public override void Awake()
    {
        //clockworkOwl = Resources.Load<GameObject>("Prefabs/AllFigures/PlayerSummons/ClockworkOwl");
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Summon(clockworkOwl)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Flight(2))));
    }
}