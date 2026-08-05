using System;
using System.Collections;
using System.Collections.Generic;
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
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new NextTurnCards(-1,Var.infinityValue))));

        currentActions.Add(new Action((currentTarget) => currentTarget.Upkeeps(new Condition[] { new Speed(-2), new Dexterity(-2) })));
        //currentActions.Add(new Action((currentTarget) => currentTarget.Upkeep(new Dexterity(-2))));
    }

    public override void PrepareBottom()
    {
        currentActions.Add(new Action(() => playerControler.ApplyCondition(new Flight(2))));
    }
}



//using System;
//using System.Collections;
//using System.Collections.Generic;
//public class MechanicalAutomaton : PlayerSummon
//{
//    public override void Awake()
//    {
//        moveSets.Add(new List<Func<IEnumerator>> {
//            () => Move(10)
//            ,() => Attack(10)
//            ,() => Block(10)
//        });

//        maxHealth = 72;
//        base.Awake();
//    }
//    //public override IEnumerator LoadFigure()
//    //{
//    //    yield return StartCoroutine(actionManager.PreformAction(GainCondition(new Flight())));
//    //    yield return StartCoroutine(base.LoadFigure());
//    //}
//}