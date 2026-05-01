using System.Collections.Generic;
using UnityEngine;
public class FrenziedAbomination : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(5)
            ,() => Attack(5)
            ,() => ApplyCondition(new Strength(4, 5))
            ,() => ApplyCondition(new Speed(4, 5))
        });

        maxHealth = 100;
        base.Start();
    }
}
