using System.Collections;
using UnityEngine;

public class MechanicalProwess : Card
{
    public MechanicalProwess() : base(3, 2, 2) { }

    [SerializeField]
    private GameObject mechanicalAutomaton;

    public override void Awake()
    {
        //clockworkOwl = Resources.Load<GameObject>("Prefabs/AllFigures/PlayerSummons/ClockworkOwl");
        base.Awake();
    }

    public override void PrepareTop()
    {
        currentActions.Add(new Action(() => playerControler.Summon(mechanicalAutomaton)));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnCards(-1,Var.infinityValue))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnBottomEnergy(-1))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnTopEnergy(-1))));
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeeps(new Condition[] { new NextTurnTopEnergy(-1,Var.infinityValue), new NextTurnBottomEnergy(-1, Var.infinityValue) })));

        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Speed(-2))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Dexterity(-2))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Strength(-2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.AddKeyword("Augment")));
        currentActions.Add(new Action((currentTarget) => currentTarget.ApplyConditions(new Condition[] { new Strength(5), new Speed(5), new Dexterity(5) } )));
        //currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Speed(5))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.ApplyCondition(new Dexterity(5))));
        
        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeeps(new Condition[] { new Strength(-2), new Speed(-2), new Dexterity(-2), new Finesse(-2) })));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Strength(-2))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Speed(-2))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Dexterity(-2))));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Finesse(-2))));
    }
}