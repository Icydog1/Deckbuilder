using System;
using System.Collections;
using System.Collections.Generic;
public class Goblin : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(25)
            ,() => Attack(10)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(25)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(15)
            ,() => Attack(15)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 50;
        XPValue = 2;
        base.Awake();
    }
}