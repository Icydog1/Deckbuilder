using System;
using System.Collections;
using System.Collections.Generic;

public class UndeadHunter : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(14)
            ,() => Attack(17, 3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(8)
            ,() => Attack(22, 3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(23)
            ,() => Attack(12, 3)
        });
        maxHealth = 50;
        XPValue = 3;
        base.Awake();
    }
}

