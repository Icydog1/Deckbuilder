using System;
using System.Collections;
using System.Collections.Generic;
public class Goblin : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(22)
            ,() => Attack(8)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Attack(17)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(12)
            ,() => Attack(13)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 29;
        XPValue = 1;
        base.Awake();
    }
}