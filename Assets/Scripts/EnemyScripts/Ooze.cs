using System;
using System.Collections;
using System.Collections.Generic;
public class Ooze : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(7)
            ,() => Attack(6,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(10,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(7)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 16;
        XPValue = 1;
        base.Awake();
    }
}