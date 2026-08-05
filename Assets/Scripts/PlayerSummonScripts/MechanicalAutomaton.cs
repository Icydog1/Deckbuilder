using System;
using System.Collections;
using System.Collections.Generic;

public class MechanicalAutomaton : PlayerSummon
{
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(16)
            ,() => Attack(12)
            ,() => Block(8)
        });

        maxHealth = 73;
        base.Awake();
    }
}

