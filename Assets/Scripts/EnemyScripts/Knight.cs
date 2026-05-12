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
            ,() => Block(13)
            ,() => ApplyCondition(new Strength(7, 2))
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(7)
            ,() => Attack(8)
            ,() => Block(16)
            ,() => ApplyCondition(new Strength(14, 2))

        });

        maxHealth = 68;
        base.Awake();
    }
}
