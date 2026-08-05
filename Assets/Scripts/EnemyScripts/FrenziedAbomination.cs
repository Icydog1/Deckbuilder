using System;
using System.Collections;
using System.Collections.Generic;
public class FrenziedAbomination : Enemy
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(5)
            ,() => ApplyCondition(new Strength(4))
        });
        moveSets.Add(new List<Func<IEnumerator>> {
            () => Move(13)
            ,() => Attack(5)
        });
        initialMoves = new List<int>() { 0, 0, 0, 0, 0 };
        movesSetOrder = new List<int>() { 1 };

        maxHealth = 104;
        XPValue = 8;

        base.Awake();
    }
}
