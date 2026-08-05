using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Archer : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(14)
            ,() => Attack(10, 10)
        });

        maxHealth = 43;
        XPValue = 6;

        base.Awake();
    }
}
