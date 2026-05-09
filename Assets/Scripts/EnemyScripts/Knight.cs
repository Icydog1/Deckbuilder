using System;
using System.Collections;
using System.Collections.Generic;

public class Knight : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(16)
            ,() => Attack(23)
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(17)
            ,() => Attack(14)
            ,() => Block(13)
            ,() => ApplyCondition(new Strength(12, 2))
        });
        moveSets.Add(new List<Func<IEnumerator>> {

            () => Move(7)
            ,() => Attack(8)
            ,() => Block(16)
            ,() => ApplyCondition(new Strength(18, 2))

        });

        maxHealth = 68;
        base.Start();
    }
}
