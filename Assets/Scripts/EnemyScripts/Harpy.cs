using System;
using System.Collections;
using System.Collections.Generic;

public class Harpy : Enemy
{
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(25)
            ,() => Attack(8)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(10)
            ,() => Attack(6,1,1,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Block(23)
            ,() => ApplyCondition(new Vigor(10))

        });

        //canFly = true;
        maxHealth = 44;
        XPValue = 6;
        base.Awake();
    }
    public override IEnumerator LoadFigure()
    {
        yield return StartCoroutine(actionManager.PreformAction(GainCondition(new Flight())));
        yield return StartCoroutine(base.LoadFigure());
    }


}
