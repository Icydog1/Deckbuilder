using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(20)
            ,() => Attack(20)
        });
        moveSets.Add(new List<System.Action> {

            () => Move(15)
            ,() => Attack(10)
            ,() => ApplyCondition(new Strength(5))
        });
        moveSets.Add(new List<System.Action> {

            () => Move(10)
            ,() => Attack(5)
            ,() => Block(10)
            ,() => ApplyCondition(new Speed(5))

        });

        maxHealth = 50;
        base.Start();
    }
}
