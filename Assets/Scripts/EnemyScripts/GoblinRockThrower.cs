using System;
using System.Collections;
using System.Collections.Generic;
public class GoblinRockThrower : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(12)
            ,() => Attack(12,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(8)
            ,() => Attack(15,3)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(19)
            ,() => Attack(9,4)
        });
        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 17;
        XPValue = 1;
        base.Awake();
    }
}