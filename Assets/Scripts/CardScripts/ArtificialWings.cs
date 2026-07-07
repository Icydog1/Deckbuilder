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
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnCards(-1,Variables.gameInfinityValue))));

        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Speed(-2))));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Dexterity(-2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Flight(2))));
    }
}