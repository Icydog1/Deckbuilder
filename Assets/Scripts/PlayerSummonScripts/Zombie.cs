using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Zombie : PlayerSummon
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(11)
            ,() => Attack(24)
        });

        maxHealth = 45;
        base.Awake();
    }
}
