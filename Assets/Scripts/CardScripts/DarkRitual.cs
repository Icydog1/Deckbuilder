using System.Collections;
using UnityEngine;

public class DarkRitual : Card
{
    public DarkRitual() : base(2, 1, 1) { }

    [SerializeField]
    private GameObject summon;

    public override void Awake()
    {
        //summon = Resources.Load<GameObject>("Prefabs/AllFigures/PlayerSummons/Zombie");
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Summon(summon)));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnTopEnergy(-1, Variables.gameInfinityValue))));
        currentActions.Add(new Action(() => playerControler.LoseHealth(3)));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.GainBottomEnergy(2)));
        currentActions.Add(new Action(() => playerControler.GainTopEnergy(2)));
        currentActions.Add(new Action(() => playerControler.LoseHealth(2)));
    }
}