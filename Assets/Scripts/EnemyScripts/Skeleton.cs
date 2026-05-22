using System;
using System.Collections;
using System.Collections.Generic;
public class Skeleton : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(17)
            ,() => Attack(12)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(18)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            //does nothing the turn it is summoned
        });
        initialMoves = new List<int>() { 2 };

        movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 22;
        XPValue = 1;
        base.Awake();
    }
}