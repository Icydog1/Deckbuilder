using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
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

            () => Move(25)
        });
        //moveSets.Add(MoveSet1);
        //moveSets.Add(MoveSet2);
        //moveSets.Add(MoveSet3);
        maxHealth = 50;
        base.Start(); // runs the code from the base
                      // add your additional code here
    }

    public void MoveSet1()
    {
        Move(15, false);
        Attack(15, 2);

    }
    public void MoveSet2()
    {
        Move(20);
        Attack(10, 2);
    }

    public void MoveSet3()
    {
        Move(25);
    }
}
