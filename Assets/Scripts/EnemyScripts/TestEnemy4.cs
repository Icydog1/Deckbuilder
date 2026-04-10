using System.Collections.Generic;
using UnityEngine;
public class TestEnemy4 : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(5)
            ,() => Attack(5)
            ,() => ApplyCondition(new Strength(2))
            ,() => ApplyCondition(new Speed(2))
        });

        maxHealth = 50;
        base.Start();
    }
}
