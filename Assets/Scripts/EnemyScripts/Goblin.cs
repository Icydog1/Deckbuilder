using System;
using System.Collections;
using System.Collections.Generic;
public class Goblin : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(28)
            ,() => Attack(11)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(27)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(17)
            ,() => Attack(14)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 49;
        XPValue = 2;
        base.Awake();
    }
}