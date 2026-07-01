using System;
using System.Collections;
using System.Collections.Generic;
public class ClockworkOwl : PlayerSummon
{
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(21)
            ,() => Attack(12)
            ,() => Block(7)
        });

        maxHealth = 25;
        base.Awake();
    }
    public override IEnumerator LoadFigure()
    {
        yield return StartCoroutine(actionManager.PreformAction(GainCondition(new Flight())));
        yield return StartCoroutine(base.LoadFigure());
    }
}
