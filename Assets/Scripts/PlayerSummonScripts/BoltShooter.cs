using System;
using System.Collections;
using System.Collections.Generic;
public class BoltShooter : PlayerSummon
{
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(6)
            ,() => Attack(12, 3)
        });

        maxHealth = 15;
        base.Awake();
    }
    //public override IEnumerator LoadFigure()
    //{
    //    yield return StartCoroutine(actionManager.PreformAction(GainCondition(new Flight())));
    //    yield return StartCoroutine(base.LoadFigure());
    //}
}
