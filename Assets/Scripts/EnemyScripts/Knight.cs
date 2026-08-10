using System;
using System.Collections;
using System.Collections.Generic;

public class Knight : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(16)
            ,() => Attack(18)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(13)
            ,() => Attack(13)
            ,() => Block(6)
            ,() => ApplyCondition(new Vigor(7))
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(7)
            ,() => Attack(8)
            ,() => Block(8)
            ,() => ApplyCondition(new Vigor(14))

        });

        maxHealth = 68;
        XPValue = 10;
        base.Awake();
    }
}
