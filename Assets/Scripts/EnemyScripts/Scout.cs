using System;
using System.Collections;
using System.Collections.Generic;

public class Scout : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(11, 5)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(14, 6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(22)
            ,() => Attack(7, 4)
        });
        maxHealth = 38;
        XPValue = 2;
        base.Awake();
    }
}

