using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(MoveSet1);
        moveSets.Add(MoveSet2);
        //moveSets.Add(MoveSet3);
        maxHealth = 10;
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); // runs the code from the base
    }

    public void MoveSet1()
    {
        Move(3, false);
        Attack(3, 2);

    }
    public void MoveSet2()
    {
        Move(4);
        Attack(2, 2);
    }

    public void MoveSet3()
    {
        Move(4);
    }
}
