using System.Collections.Generic;
using UnityEngine;

public class TestEnemy2 : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(MoveSet1);
        moveSets.Add(MoveSet2);
        moveSets.Add(MoveSet3);
        maxHealth = 10;
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public void MoveSet1()
    {
        Move(10);
        Attack(15);

    }
    public void MoveSet2()
    {
        Move(20);
        Block(10);
    }

    public void MoveSet3()
    {
        Move(25);
        Attack(5);
    }
}
