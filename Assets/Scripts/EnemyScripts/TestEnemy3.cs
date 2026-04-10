using System.Collections.Generic;
using UnityEngine;
public class TestEnemy3 : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(10)
            ,() => Attack(10, 10)
        });

        maxHealth = 50;
        base.Start();
    }
}
