using System;
using System.Collections;
using System.Collections.Generic;
public class Pirate : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13,true)
            ,() => Attack(10)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(8,true)
            ,() => Attack(12)
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(17,true)
            ,() => Attack(8)
        });
        initialMoves = new List<int>() { };

        //movesSetOrder = new List<int>() { 0, 1 };
        maxHealth = 17;
        XPValue = 2;
        base.Awake();
    }
}