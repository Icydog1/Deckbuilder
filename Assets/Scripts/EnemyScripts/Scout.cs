using System.Collections.Generic;
using UnityEngine;

public class Scout : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(10)
            ,() => Attack(15, 5)
        });
        moveSets.Add(new List<System.Action> {
            () => Attack(25, 6)
        });
        moveSets.Add(new List<System.Action> {
            () => Move(20)
            ,() => Attack(20, 4)
        });
        maxHealth = 50;
        base.Start();
    }
}

