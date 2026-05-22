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
            ,() => Attack(16, 5)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(28, 6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(22)
            ,() => Attack(18, 4)
        });
        maxHealth = 48;
        XPValue = 3;
        base.Awake();
    }
}

