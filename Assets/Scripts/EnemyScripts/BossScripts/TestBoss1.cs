using System.Collections.Generic;
using UnityEngine;

public class TestBoss1 : Boss
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(15, false)
            ,() => Attack(15, 2)
        });
        moveSets.Add(new List<System.Action> {

            () => Move(20)
            ,() => Attack(10, 2)
        });
        moveSets.Add(new List<System.Action> {

            () => Block(25)
            ,() => ApplyCondition(new Strength(3, -1))
        });

        maxHealth = 500;
        base.Start();
    }

}
