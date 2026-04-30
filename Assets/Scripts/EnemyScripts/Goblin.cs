using System.Collections.Generic;
using UnityEngine;
public class Goblin : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        moveSets.Add(new List<System.Action> {
            () => Move(20)
            ,() => Attack(10)
        });
        moveSets.Add(new List<System.Action> {
            () => Attack(25)
        });
        moveSets.Add(new List<System.Action> {
            () => Move(10)
            ,() => Attack(15)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 50;
        base.Start();
    }
}