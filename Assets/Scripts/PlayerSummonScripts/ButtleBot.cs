using System;
using System.Collections;
using System.Collections.Generic;
public class ButtleBot : PlayerSummon
{
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(4)
        });

        maxHealth = 6;
        base.Awake();
    }
}