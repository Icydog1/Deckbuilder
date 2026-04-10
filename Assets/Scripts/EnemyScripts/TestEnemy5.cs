using System.Collections.Generic;
using UnityEngine;
public class TestEnemy5 : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(25)
            ,() => Attack(5)
            ,() => Attack(5)
        });
        moveSets.Add(new List<System.Action> {
            () => ApplyCondition(new Strength(4))
        });

        movesSetOrder = new List<int>() {1,2 };
        maxHealth = 50;
        base.Start();
    }
}
