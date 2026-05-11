using System;
using System.Collections;
using System.Collections.Generic;

public class Scout : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(10)
            ,() => Attack(15, 5)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(25, 6)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(20)
            ,() => Attack(20, 4)
        });
        maxHealth = 50;
        base.Awake();
    }
}

